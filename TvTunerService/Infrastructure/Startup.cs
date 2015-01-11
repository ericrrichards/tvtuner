using System.Net;
using System.Reflection;
using log4net;
using Nancy;
using Nancy.Conventions;
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
    public class Bootstrapper : DefaultNancyBootstrapper {
        protected override void ConfigureConventions(NancyConventions nancyConventions) {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("/videos")
            );
        }
    }
}