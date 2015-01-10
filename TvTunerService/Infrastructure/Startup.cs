using System.Net;
using System.Reflection;
using log4net;
using Owin;

namespace TvTunerService {
    public class Startup {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void Configuration(IAppBuilder app) {

            var listener = (HttpListener)app.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;
            
            app.UseNancy();
        }
    }
}