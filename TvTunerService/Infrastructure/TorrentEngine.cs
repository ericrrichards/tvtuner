using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using log4net;
using MonoTorrent;

namespace TvTunerService.Infrastructure {
    public class TorrentEngine {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #region Singleton boilerplate
        private static readonly TorrentEngine _instance = new TorrentEngine();
        public static TorrentEngine Instance {
            get { return _instance; }
        }
        static TorrentEngine() { }
        #endregion

        private readonly string _downloadBasePath;


        private TorrentEngine() {
            _downloadBasePath = ConfigurationManager.AppSettings["VideoDirectory"];
            
        }

        public string AddMagnetLink(string magnet, string downloadPath) {

            downloadPath = Path.Combine(_downloadBasePath, downloadPath);
            if (!Directory.Exists(downloadPath)) {
                Directory.CreateDirectory(downloadPath);
            }
            var magnetLink = new MagnetLink(magnet);


            var ariaPath = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "aria2c.exe");
            var args = new ProcessStartInfo(ariaPath, string.Format("-d {0} --seed-time=0 \"{1}\"", downloadPath, magnet )) {
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var proc = new Process { StartInfo = args };
            proc.Exited += (sender, e) => Log.Info("Finished downloading " + magnetLink.Name);
            proc.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrWhiteSpace(e.Data)) Log.Debug(e.Data);
            };
            proc.EnableRaisingEvents = true;

            proc.Start();

            proc.BeginOutputReadLine();

            Log.InfoFormat("Starting torrent {0}, downloading to {1}", magnetLink.Name, downloadPath);
            return Path.Combine(downloadPath, magnetLink.Name);
        }

    }
}
