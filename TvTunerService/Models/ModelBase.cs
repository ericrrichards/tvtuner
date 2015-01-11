using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using TvTunerService.Infrastructure;

namespace TvTunerService.Models {
    public class ModelBase {
        protected ModelBase() { }
        public ModelBase(NancyContext context) {
            if (context != null) {
                var env = ((IDictionary<string, object>)context.Items[Nancy.Owin.NancyMiddleware.RequestEnvironmentKey]);
                User = (IPrincipal)env["server.User"];

            }
        }
        public IPrincipal User { get; private set; }
    }

    public class ShowIndexModel : ModelBase {
        public List<Show> Shows { get; set; } 
        public ShowIndexModel(NancyContext context, List<Show> shows) : base(context) {
            Shows = shows;
        }
    }
}
