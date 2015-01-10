using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using MonoTorrent;
using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;
using MonoTorrent.Dht;
using MonoTorrent.Dht.Listeners;

namespace TvTunerService.Infrastructure {
    public class TorrentEngine {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ClientEngine _engine;
        private readonly List<TorrentManager> _torrents;
        private bool _running;

        private readonly string _downloadBasePath;
        private readonly string _dhtNodesFile;
        private readonly string _fastResumeFile;
        private readonly string _torrentPath;

        private readonly Top10Listener _listener;
        private Thread _loopThread;

        #region Singleton boilerplate
        private static readonly TorrentEngine _instance = new TorrentEngine();
        private TorrentSettings _torrentDefaults;
        private BEncodedDictionary _fastResume;

        public static TorrentEngine Instance {
            get { return _instance; }
        }
        static TorrentEngine() { }
        #endregion

        private TorrentEngine() {
            _downloadBasePath = ConfigurationManager.AppSettings["VideoDirectory"];
            _dhtNodesFile = Path.Combine(_downloadBasePath, "DhtNodes");
            _fastResumeFile = Path.Combine(_downloadBasePath, "fastresume.data");
            _torrentPath = _downloadBasePath;

            _torrents = new List<TorrentManager>();
            _listener = new Top10Listener(10);
            _torrentDefaults = new TorrentSettings(4, 150, 0, 0);
        }

        public void StartEngine() {
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["torrentPort"]);

            var engineSettings = new EngineSettings(_downloadBasePath, port) {
                PreferEncryption = false,
                AllowedEncryption = EncryptionTypes.All,
                GlobalMaxUploadSpeed = 3 * 1024
            };

            _engine = new ClientEngine(engineSettings);
            _engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, port));
            byte[] nodes = null;
            try {
                nodes = File.ReadAllBytes(_dhtNodesFile);
            } catch {
                Log.Warn("No existing dht nodes could be loaded");
            }

            var dhtListener = new DhtListener(new IPEndPoint(IPAddress.Any, port));
            var dht = new DhtEngine(dhtListener);
            _engine.RegisterDht(dht);
            dhtListener.Start();
            _engine.DhtEngine.Start(nodes);

            if (!Directory.Exists(_engine.Settings.SavePath)) {
                Directory.CreateDirectory(_engine.Settings.SavePath);
            }

            try {
                _fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(_fastResumeFile));
            } catch {
                _fastResume = new BEncodedDictionary();
            }
            _running = true;
            _loopThread = new Thread(ProcessLoop);
            _loopThread.Start();
        }

        public void StopEngine() {
            _running = false;
            var fastResume = new BEncodedDictionary();
            foreach (var t in _torrents) {
                t.Stop();
                while (t.State != TorrentState.Stopped) {
                    Log.DebugFormat("{0} is {1}", t.Torrent.Name, t.State);
                    Thread.Sleep(250);
                }

                fastResume.Add(t.Torrent.InfoHash.ToHex(), t.SaveFastResume().Encode());
            }
            _loopThread.Join();
            File.WriteAllBytes(_fastResumeFile, fastResume.Encode());
            _engine.Dispose();
            foreach (TraceListener lst in Debug.Listeners) {
                lst.Flush();
                lst.Close();
            }

            Thread.Sleep(2000);
        }

        public void AddMagnetLink(string magnet, string downloadPath) {
            downloadPath = Path.Combine(_downloadBasePath, downloadPath);
            if (!Directory.Exists(downloadPath)) {
                Directory.CreateDirectory(downloadPath);
            }
            var magnetLink = new MagnetLink(magnet);
            var manager = new TorrentManager(magnetLink, downloadPath, _torrentDefaults, _torrentPath);
            if (_fastResume.ContainsKey(magnetLink.InfoHash.ToHex())) {
                manager.LoadFastResume(new FastResume(((BEncodedDictionary)_fastResume[magnetLink.InfoHash.ToHex()])));
            }
            _engine.Register(manager);

            _torrents.Add(manager);
            manager.PeersFound += manager_PeersFound;

            // Every time a piece is hashed, this is fired.
            manager.PieceHashed += delegate(object o, PieceHashedEventArgs e) {
                lock (_listener)
                    _listener.WriteLine(string.Format("Piece Hashed: {0} - {1}", e.PieceIndex, e.HashPassed ? "Pass" : "Fail"));
            };

            // Every time the state changes (Stopped -> Seeding -> Downloading -> Hashing) this is fired
            manager.TorrentStateChanged += delegate(object o, TorrentStateChangedEventArgs e) {
                lock (_listener)
                    _listener.WriteLine("OldState: " + e.OldState + " NewState: " + e.NewState);
            };

            // Every time the tracker's state changes, this is fired
            foreach (var tier in manager.TrackerManager) {
                foreach (var t in tier.GetTrackers()) {
                    t.AnnounceComplete += (sender, e) => _listener.WriteLine(string.Format("{0}: {1}", e.Successful, e.Tracker.ToString()));
                }
            }
            // Start the torrentmanager. The file will then hash (if required) and begin downloading/seeding
            manager.Start();
        }

        private void ProcessLoop() {
            var i = 0;
            var sb = new StringBuilder(1024);
            var sw = new StringWriter();
            while (_running) {
                if ((i++) % 10 == 0) {
                    sb.Clear();

                    AppendFormat(sb, "Total Download Rate: {0:0.00}kB/sec", _engine.TotalDownloadSpeed / 1024.0);
                    AppendFormat(sb, "Total Upload Rate:   {0:0.00}kB/sec", _engine.TotalUploadSpeed / 1024.0);
                    AppendFormat(sb, "Disk Read Rate:      {0:0.00} kB/s", _engine.DiskManager.ReadRate / 1024.0);
                    AppendFormat(sb, "Disk Write Rate:     {0:0.00} kB/s", _engine.DiskManager.WriteRate / 1024.0);
                    AppendFormat(sb, "Total Read:         {0:0.00} kB", _engine.DiskManager.TotalRead / 1024.0);
                    AppendFormat(sb, "Total Written:      {0:0.00} kB", _engine.DiskManager.TotalWritten / 1024.0);
                    AppendFormat(sb, "Open Connections:    {0}", _engine.ConnectionManager.OpenConnections);

                    foreach (var manager in _torrents) {
                        AppendSeperator(sb);
                        AppendFormat(sb, "State:           {0}", manager.State);
                        AppendFormat(sb, "Name:            {0}", manager.Torrent == null ? "MetaDataMode" : manager.Torrent.Name);
                        AppendFormat(sb, "Progress:           {0:0.00}", manager.Progress);
                        AppendFormat(sb, "Download Speed:     {0:0.00} kB/s", manager.Monitor.DownloadSpeed / 1024.0);
                        AppendFormat(sb, "Upload Speed:       {0:0.00} kB/s", manager.Monitor.UploadSpeed / 1024.0);
                        AppendFormat(sb, "Total Downloaded:   {0:0.00} MB", manager.Monitor.DataBytesDownloaded / (1024.0 * 1024.0));
                        AppendFormat(sb, "Total Uploaded:     {0:0.00} MB", manager.Monitor.DataBytesUploaded / (1024.0 * 1024.0));
                        var tracker = manager.TrackerManager.CurrentTracker;
                        //AppendFormat(sb, "Tracker Status:     {0}", tracker == null ? "<no tracker>" : tracker.State.ToString());
                        AppendFormat(sb, "Warning Message:    {0}", tracker == null ? "<no tracker>" : tracker.WarningMessage);
                        AppendFormat(sb, "Failure Message:    {0}", tracker == null ? "<no tracker>" : tracker.FailureMessage);

                        AppendFormat(sb, "", null);
                        if (manager.Torrent == null) { continue; }
                        foreach (var file in manager.Torrent.Files)
                            AppendFormat(sb, "{1:0.00}% - {0}", file.Path, file.BitField.PercentComplete);
                    }
                    Log.Debug(sb.ToString());
                    _listener.ExportTo(sw);
                    Log.Debug(sw.GetStringBuilder());
                    sw.GetStringBuilder().Clear();
                }
                Thread.Sleep(500);
            }
        }
        private static void AppendFormat(StringBuilder sb, string str, params object[] formatting) {
            if (formatting != null)
                sb.AppendFormat(str, formatting);
            else
                sb.Append(str);
            sb.AppendLine();
        }
        private static void AppendSeperator(StringBuilder sb) {
            AppendFormat(sb, "", null);
            AppendFormat(sb, "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - -", null);
            AppendFormat(sb, "", null);
        }
        void manager_PeersFound(object sender, PeersAddedEventArgs e) {
            lock (_listener)
                _listener.WriteLine(string.Format("Found {0} new peers and {1} existing peers", e.NewPeers, e.ExistingPeers));//throw new Exception("The method or operation is not implemented.");
        }
    }
}
