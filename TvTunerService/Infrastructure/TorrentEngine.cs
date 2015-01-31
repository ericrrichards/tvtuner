using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using MediaToolkit;
using MediaToolkit.Model;
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
            proc.Exited += (sender, e) => {
                Log.Info("Finished downloading " + magnetLink.Name);
                var file = GetBestMatch(Directory.GetFiles(downloadPath), magnetLink.Name);

                var newfileName = Path.Combine(Path.GetDirectoryName(file), magnetLink.Name + Path.GetExtension(file));
                if (newfileName != file) {
                    File.Move(file, newfileName);
                }
                if (!string.IsNullOrEmpty(file) && Path.GetExtension(file) != ".mp4") {
                    Log.Debug("Needs conversion");
                    Transcoder.ConvertToMp4(newfileName);
                } else {
                    Log.Debug("Format ok");
                }
            };
            proc.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrWhiteSpace(e.Data)) Log.Debug(e.Data);
            };
            proc.EnableRaisingEvents = true;

            proc.Start();

            proc.BeginOutputReadLine();

            Log.InfoFormat("Starting torrent {0}, downloading to {1}", magnetLink.Name, downloadPath);
            return Path.Combine(downloadPath, magnetLink.Name);
        }

        private string GetBestMatch(string[] files, string name) {
            var firstTry = files.FirstOrDefault(f => f.Contains(name));
            if (firstTry != null) {
                return firstTry;
            }
            string bestmatch = null;
            int bestCount = 0;
            var nameParts = name.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var file in files) {
                var fileParts = file.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                var count = nameParts.Intersect(fileParts).Count();
                if (count > bestCount) {
                    bestCount = count;
                    bestmatch = file;
                }
            }
            return bestmatch;
        }
    }

    public static class Transcoder {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void ConvertToMp4(string original) {
            var converted = Path.GetDirectoryName(original) + "/" + Path.GetFileNameWithoutExtension(original) + ".mp4";

            ConvertToMp4(original, converted);
        }

        public static void ConvertToMp4(string original, string converted) {
            var proc = new ProcessStartInfo("HandBrakeCLI.exe") {
                Arguments = string.Format("-i \"{0}\" -o \"{1}\" --preset=\"Normal\"", original, converted),
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            var p = new Process {StartInfo = proc, EnableRaisingEvents = true};
            p.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrWhiteSpace(e.Data)) Log.Debug(e.Data);
            };
            p.Exited += (sender, e) => {
                Log.InfoFormat("Finished transcoding {0} to {1}", original, converted);
                File.Delete(original);
            };
            
            p.Start();

            p.BeginOutputReadLine();
        }
    }
}
