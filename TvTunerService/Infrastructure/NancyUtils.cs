using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
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

        public static Response FromPartialFile(this IResponseFormatter f,
                                                 Request req, string path,
                                                 string contentType) {
            return f.FromPartialStream(req, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), contentType);
        }

        public static Response FromPartialStream(this IResponseFormatter f,
                                                 Request req, Stream stream,
                                                 string contentType) {
            // Store the len
            var len = stream.Length;

            // Create the response now
            var res = f.FromStream(stream, contentType).
                		WithHeader("connection", "keep-alive").
                WithHeader("accept-ranges", "bytes");

            // Use the partial status code
            res.StatusCode = HttpStatusCode.PartialContent;

            long startI = 0;
            foreach (var s in req.Headers["Range"]) {
                var start = s.Split('=')[1];

                var m = Regex.Match(start, @"(\d+)-(\d+)?");

                start = m.Groups[1].Value;
                var end = len-1;
                if (m.Groups[2] != null && !string.IsNullOrWhiteSpace(m.Groups[2].Value)) {
                    end = Convert.ToInt64(m.Groups[2].Value);
                }
                

                startI = Convert.ToInt64(start);
                var length = len - startI;
                res.WithHeader("content-range","bytes " + start + "-" + end + "/" + len);
                res.WithHeader("content-length", length.ToString(CultureInfo.InvariantCulture));
            }

            stream.Seek(startI, SeekOrigin.Begin);
            return res;
        }

    }
}