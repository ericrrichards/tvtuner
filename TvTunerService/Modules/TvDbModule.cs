using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Nancy;

namespace TvTunerService.Modules {
    public class TvDbModule : NancyModule{
        public const string ApiKey = "645AAFA61BE26C3C";
        private const string ShowBannerDir = "Content/images/showBanners";

        public TvDbModule() {
            if (!Directory.Exists(ShowBannerDir)) {
                Directory.CreateDirectory(ShowBannerDir);
            }
            Get["/TvDb/LookupShow"] = LookupShow;
            Get["/TvDb/ShowInformation/{id}"] = ShowInformation;
        }

        private dynamic LookupShow(dynamic parameters) {
            var search = Request.Query["search"];
            using (var wc = new WebClient()) {
                var address = string.Format("http://thetvdb.com/api/GetSeries.php?seriesname={0}", GetSearchTerms(search));
                XDocument searchResults = XDocument.Parse(wc.DownloadString(address));
                var series = searchResults.Descendants("Series");
                var results = new List<Tuple<string, string>> ();
                foreach (var seri in series) {
                    var id = seri.Descendants("seriesid").First().Value;
                    var name = seri.Descendants("SeriesName").First().Value;
                    results.Add(new Tuple<string, string>(name, id));
                }
                return NancyUtils.JsonResponse(results);
            }
        }
        private static string GetSearchTerms(string text) {
            return text.Split(new[] { ' ' }).Aggregate((i, j) => i + "+" + j);
        }

        private dynamic ShowInformation(dynamic parameters) {
            var id = parameters.id;
            using (var wc = new WebClient()) {
                var address = string.Format("http://thetvdb.com/api/{0}/series/{1}/all/en.zip", ApiKey, id);
                var tempPath = Path.GetTempFileName();

                wc.DownloadFile(address, tempPath);

                var zip = ZipFile.Open(tempPath, ZipArchiveMode.Read);

                var seriesXml = zip.GetEntry("en.xml");
                var xml = XDocument.Load(seriesXml.Open());

                var series = xml.Descendants("Series").FirstOrDefault();
                if (series != null) {
                    string bannerPath = null;
                    string name = null;
                    string summary = null;
                    var nameElem = series.Descendants("SeriesName").FirstOrDefault();

                    if (nameElem != null) {
                        name = nameElem.Value;
                    }
                    var bannerElem = series.Descendants("banner").FirstOrDefault();

                    if (bannerElem != null) {
                        bannerPath = Path.Combine("http://thetvdb.com/banners", bannerElem.Value);
                    }
                    var summaryElem = series.Descendants("Overview").FirstOrDefault();
                    if (summaryElem != null) {
                        summary = summaryElem.Value;
                    }


                    var imgDlPath = Path.Combine(ShowBannerDir, name + Path.GetExtension(bannerPath));
                    if (!File.Exists(imgDlPath)) {
                        wc.DownloadFile(bannerPath, imgDlPath);
                    }
                    var s = new { Name = name, Summary = summary, BannerPath=imgDlPath };

                    
                    return NancyUtils.JsonResponse(s);
                }
                return NancyUtils.JsonResponse("No result found");
            }
        }
    }
}
