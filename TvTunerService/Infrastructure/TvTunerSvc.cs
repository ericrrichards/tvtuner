using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using Microsoft.Owin.Hosting;
using TvTunerService.Infrastructure;

namespace TvTunerService {
    public partial class TvTunerSvc : ServiceBase {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IDisposable _webhost;
        private const string Url = "http://+/TvTuner";
        private static bool _running;

        public TvTunerSvc() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            Start();
        }

        protected override void OnStop() {
            End();
        }
        public void Start() {
            _running = true;
            log4net.Config.XmlConfigurator.Configure();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            _webhost = WebApp.Start<Startup>(Url);
            Log.Debug("Started on " + Url);
            TorrentEngine.Instance.StartEngine();
        }
        public void End() {
            TorrentEngine.Instance.StopEngine();
            Log.DebugFormat("Killing webhost on {0}", Url);
            _webhost.Dispose();
            _running = false;
        }

        
    }
}
