using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using log4net;
using Nancy;
using Nancy.Owin;
using Nancy.Responses;
using Newtonsoft.Json;

namespace TvTunerService {
    public static class NancyUtils {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string GetUsername(NancyContext context) {
            if (context != null) {
                try {
                    var env = ((IDictionary<string, object>)context.Items[NancyMiddleware.RequestEnvironmentKey]);
                    var user = (IPrincipal)env["server.User"];
                    return user.Identity.Name;
                } catch (Exception ex) {
                    Log.Error("Exception in " + ex.TargetSite.Name, ex);
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Equivalent to JsonResult from ASP.Net MVC
        /// </summary>
        /// <param name="data">data object to serialize to JSON</param>
        /// <returns>json Response</returns>
        public static Response JsonResponse(object data) {
            var ret = (Response)JsonConvert.SerializeObject(data);
            ret.ContentType = "application/json";
            return ret;
        }

        public static Response CsvResponse(string csvData, string downloadFilename) {
            var response = new StreamResponse(() => new MemoryStream(new UTF8Encoding().GetBytes(csvData)), "text/csv");
            response.Headers.Add("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
            return response;
        }

        public static Response TextFileResponse(byte[] data, string downloadFilename) {
            var response = new StreamResponse(() => new MemoryStream(data), "text/plain");
            response.Headers.Add("Content-Disposition", "attachment; filename=\"" + downloadFilename + "\"");
            return response;
        }

    }
}