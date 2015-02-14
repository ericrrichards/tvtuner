using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using log4net;
using Nancy;

namespace TvTunerService.Modules {
    public class TvDbModule : NancyModule{
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public const string ApiKey = "645AAFA61BE26C3C";
        private const string ShowBannerDir = "Content/images/showBanners";
        private const string EpisodeThumbDir = "Content/images/episodeThumbs";
        private const string ShowXmlCacheDir = "ShowXml";

        public TvDbModule() {
            if (!Directory.Exists(ShowBannerDir)) {
                Directory.CreateDirectory(ShowBannerDir);
            }
            if (!Directory.Exists(ShowXmlCacheDir)) {
                Directory.CreateDirectory(ShowXmlCacheDir);
            }
            if (!Directory.Exists(EpisodeThumbDir)) {
                Directory.CreateDirectory(EpisodeThumbDir);
            }

            Get["/TvDb/LookupShow"] = LookupShow;
            Get["/TvDb/ShowInformation/{id}"] = ShowInformation;
            Get["/TvDB/LookupEpisode"] = LookupEpisode;
        }

        private dynamic LookupShow(dynamic parameters) {
            try {
                var search = Request.Query["search"];
                List<Tuple<string, string>> results = DoShowLookup(search);
                return NancyUtils.JsonResponse(results);
            } catch (Exception ex) {
                Log.Error("Exception in " + ex.TargetSite.Name, ex);
                return NancyUtils.JsonResponse(ex.Message);
            }
        }

        private static List<Tuple<string, string>> DoShowLookup(string search) {
            List<Tuple<string, string>> results;
            using (var wc = new WebClient()) {
                var address = string.Format("http://thetvdb.com/api/GetSeries.php?seriesname={0}", GetSearchTerms(search));
                XDocument searchResults = XDocument.Parse(wc.DownloadString(address));
                var series = searchResults.Descendants("Series");
                results = new List<Tuple<string, string>>();
                foreach (var seri in series) {
                    var id = seri.Descendants("seriesid").First().Value;
                    var name = seri.Descendants("SeriesName").First().Value;
                    results.Add(new Tuple<string, string>(name, id));
                }
            }
            return results;
        }

        private static string GetSearchTerms(string text) {
            return text.Split(new[] { ' ' }).Aggregate((i, j) => i + "+" + j);
        }

        private dynamic ShowInformation(dynamic parameters) {
            var id = parameters.id;
            using (var wc = new WebClient()) {
                XDocument xml = GetShowXml(id);

                var series = xml.Descendants("Series").FirstOrDefault();
                if (series != null) {
                    string bannerPath = null;
                    string bannerFile = null;
                    string name = null;
                    string summary = null;
                    var nameElem = series.Descendants("SeriesName").FirstOrDefault();

                    if (nameElem != null) {
                        name = nameElem.Value;
                    }
                    var bannerElem = series.Descendants("banner").FirstOrDefault();

                    if (bannerElem != null && !string.IsNullOrWhiteSpace(bannerElem.Value)) {
                        bannerFile = bannerElem.Value;

                    }
                    var summaryElem = series.Descendants("Overview").FirstOrDefault();
                    if (summaryElem != null) {
                        summary = summaryElem.Value;
                    }

                    bannerPath = Path.Combine("http://thetvdb.com/banners", bannerFile);
                    var extension = Path.GetExtension(bannerPath);
                    if (string.IsNullOrWhiteSpace(extension)) {
                        extension = ".jpg";
                    }
                    var imgDlPath = Path.Combine(ShowBannerDir, name + extension);
                    imgDlPath = imgDlPath.Replace(':', '-');
                    if (!File.Exists(imgDlPath) && !string.IsNullOrWhiteSpace(bannerPath)) {
                        wc.DownloadFile(bannerPath, imgDlPath);
                    } else if (string.IsNullOrWhiteSpace(bannerPath)) {
                        var bmp = new Bitmap(758, 140);
                        var g = Graphics.FromImage(bmp);
                        var font = new Font("Impact", 24);
                        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                        g.FillRectangle(Brushes.Black, rect);
                        var sf = new StringFormat() {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
                        g.DrawString(name, font, Brushes.White, rect, sf);

                        bmp.Save(imgDlPath);
                    }

                    var s = new { Name = name, Summary = summary, BannerPath=imgDlPath };

                    
                    return NancyUtils.JsonResponse(s);
                }
                return NancyUtils.JsonResponse("No result found");
            }
        }

        

        private dynamic LookupEpisode(dynamic parameters) {
            string showName = null;
            int seasonNum = 0; int episodeNum = 0;
            try {
                showName = Request.Query["showname"];

                seasonNum = Request.Query["seasonNum"];

                episodeNum = Request.Query["episodeNum"];
            } catch (Exception ex) {
                Log.Error("Exception in " + ex.TargetSite.Name, ex);
            }
            var results = DoShowLookup(showName);
            var id = results.First().Item2;
            var xml = GetShowXml(id);

            var episodes = xml.Descendants("Episode").ToList();
            XElement foundEpisode = null;
            foreach (var ep  in episodes) {
                var epSeason = Convert.ToInt32(ep.Descendants("SeasonNumber").First().Value);
                var epEpNum = Convert.ToInt32(ep.Descendants("EpisodeNumber").First().Value);
                if (epSeason == seasonNum && epEpNum == episodeNum) {
                    foundEpisode = ep;
                    break;
                }
            }
            if (foundEpisode != null) {
                var ret = AquireEpisodeInfo(foundEpisode);
                return NancyUtils.JsonResponse(ret);
            }
            else {
                var eps = episodes.Select(e => new {
                    title = e.Descendants("EpisodeName").First().Value,
                    season = Convert.ToInt32(e.Descendants("SeasonNumber").First().Value),
                    episodeNumber = Convert.ToInt32(e.Descendants("EpisodeNumber").First().Value)
                });

                return NancyUtils.JsonResponse(eps.ToArray());
            }
        }

        public static TvDbShowInfo LookupUploadedEpisode(string showName, int seasonNum, int episodeNum) {
            var results = DoShowLookup(showName);
            var id = results.First().Item2;
            var xml = GetShowXml(id);

            var episodes = xml.Descendants("Episode").ToList();
            XElement foundEpisode = null;
            foreach (var ep in episodes) {
                var epSeason = Convert.ToInt32(ep.Descendants("SeasonNumber").First().Value);
                var epEpNum = Convert.ToInt32(ep.Descendants("EpisodeNumber").First().Value);
                if (epSeason == seasonNum && epEpNum == episodeNum) {
                    foundEpisode = ep;
                    break;
                }
            }
            if (foundEpisode != null) {
                var ret = AquireEpisodeInfo(foundEpisode);
                return ret;
            }
            return null;
        }

        private static TvDbShowInfo AquireEpisodeInfo(XElement foundEpisode) {
            var title = foundEpisode.Descendants("EpisodeName").First().Value;
            var summary = foundEpisode.Descendants("Overview").First().Value;
            if (string.IsNullOrWhiteSpace(summary)) {
                summary = "No summary";
            }
            var thumbFile = foundEpisode.Descendants("filename").First().Value;
            var thumbpath = Path.Combine("http://thetvdb.com/banners", thumbFile);
            var extension = Path.GetExtension(thumbpath);
            if (string.IsNullOrWhiteSpace(extension)) {
                extension = ".jpg";
            }
            var imgDlPath = Path.Combine(EpisodeThumbDir, title.Replace("/", "-").Replace("\\", "-") + extension);
            imgDlPath = imgDlPath.Replace(':', '-').Replace(" ", "_");
            using (var wc = new WebClient()) {
                if (!File.Exists(imgDlPath) && !string.IsNullOrWhiteSpace(thumbFile)) {
                    wc.DownloadFile(thumbpath, imgDlPath);
                } else if (string.IsNullOrWhiteSpace(thumbFile)) {
                    var bmp = new Bitmap(400, 225);
                    var g = Graphics.FromImage(bmp);
                    var font = new Font("Impact", 24);
                    var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    g.FillRectangle(Brushes.Black, rect);
                    var sf = new StringFormat() {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(title, font, Brushes.White, rect, sf);

                    bmp.Save(imgDlPath);
                }
            }
            var ret = new TvDbShowInfo(title, summary, imgDlPath);
            return ret;
        }

        private static XDocument GetShowXml(string id) {
            using (var wc = new WebClient()) {
                var tempPath = Path.Combine(ShowXmlCacheDir, id + ".zip");
                if (!File.Exists(tempPath) || File.GetLastWriteTime(tempPath).Date != DateTime.Now.Date) {
                    var address = string.Format("http://thetvdb.com/api/{0}/series/{1}/all/en.zip", ApiKey, id);
                    wc.DownloadFile(address, tempPath);
                }
                var zip = ZipFile.Open(tempPath, ZipArchiveMode.Read);

                var seriesXml = zip.GetEntry("en.xml");
                XDocument xml = XDocument.Load(seriesXml.Open());
                return xml;
            }
        }
    }
    public class TvDbShowInfo {
        private readonly string _title;
        private readonly string _summary;
        private readonly string _imgDlPath;
        public string title { get { return _title; } }
        public string summary { get { return _summary; } }
        public string imgDlPath { get { return _imgDlPath; } }

        public TvDbShowInfo(string title, string summary, string imgDlPath) {
            _title = title;
            _summary = summary;
            _imgDlPath = imgDlPath;
        }
    }
}
