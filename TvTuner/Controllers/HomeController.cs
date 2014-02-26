using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TvTuner.Controllers {
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    using DatabaseStuffer;

    using Episode = TvTuner.Episode;
    using Series = TvTuner.Series;

    public class HomeController : Controller {
        public ActionResult Index() {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            var db = new TvTunerDataContext();
            var series = db.Series.OrderBy(s=>s.Name).ToList();

            var dict = db.Categories.ToDictionary(k => k.CategoryName, v => new List<Series>());
            dict["Miscellaneous"] = new List<Series>();
            foreach (var s in series) {
                if (s.Category != null) {
                    dict[s.Category.CategoryName].Add(s);
                } else {
                    dict["Miscellaneous"].Add(s);
                }
            }

            var indexModel = new IndexModel {
                Series = dict,
            };


            return View(indexModel);
        }

        public ActionResult GetSeriesBanner(int id) {
            using (var db = new TvTunerDataContext()) {
                var show = db.Series.FirstOrDefault(s => s.SeriesID == id);
                if (show != null) {
                    return File(show.BannerImg.ToArray(), "img/jpeg", id + ".jpeg");
                }
            }
            return null;
        }

        public ActionResult Show(int id) {
            using (var db = new TvTunerDataContext()) {
                var show = db.Series.FirstOrDefault(s => s.SeriesID == id);
                var model = new ShowModel() { Series = show };
                model.Episodes = show.Episodes.OrderBy(e => e.Season).ThenBy(e=>e.EpisodeNumber).ToList();
                return View(model);
            }
        }

        public ActionResult WatchChannel(int id) {
            using (var db = new TvTunerDataContext()) {
                var channel = db.Channels.FirstOrDefault(c => c.ChannelID == id);
                var rand = new Random();
                var series = channel.ChannelSeries.Select(cs => cs.Series).ToList();
                var pickedShow = series.ElementAt(rand.Next(series.Count()));
                var pickedEpisode = pickedShow.Episodes.ElementAt(rand.Next(pickedShow.Episodes.Count));

                ViewBag.ChannelID = id;
                ViewBag.ChannelName = channel.Name;
                var home = new Uri(Server.MapPath(Request.Url.AbsolutePath));
                pickedEpisode.VideoPath = home.MakeRelativeUri(new Uri(pickedEpisode.VideoPath)).ToString();
                return View(pickedEpisode);
            }
        }

        public ActionResult Watch(int id, bool random = false) {
            var db = new TvTunerDataContext();
            var episode = db.Episodes.FirstOrDefault(e => e.EpisodeID == id);
            var home = new Uri(Server.MapPath(Request.Url.AbsolutePath));
            episode.VideoPath = home.MakeRelativeUri(new Uri(episode.VideoPath)).ToString();
            ViewBag.Random = random;
            return View(episode);

        }


        public ActionResult About() {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetEpisodeThumb(int id) {
            using (var db = new TvTunerDataContext()) {
                var episode = db.Episodes.FirstOrDefault(e => e.EpisodeID == id);
                if (episode != null) {
                    return File(episode.Thumbnail.ToArray(), "img/jpeg");
                }
            }
            return null;
        }

        public ActionResult RandomEpisode(int id) {
            using (var db = new TvTunerDataContext()) {
                var show = db.Series.FirstOrDefault(s => s.SeriesID == id);

                var r = new Random();

                var episodes = show.Episodes.ToArray();
                var next = r.Next(episodes.Length);
                ViewBag.Random = true;

                return RedirectToAction("Watch", new { id = episodes[next].EpisodeID, random = true });
            }
        }

        public ActionResult NextEpisode(int id) {
            using (var db = new TvTunerDataContext()) {
                var episode = db.Episodes.FirstOrDefault(e => e.EpisodeID == id);
                var firstTry = db.Episodes.FirstOrDefault(e => e.SeriesID == episode.SeriesID && e.Season == episode.Season && e.EpisodeNumber == episode.EpisodeNumber + 1);
                if (firstTry != null) {
                    return RedirectToAction("Watch", new { id = firstTry.EpisodeID });
                }

                var secondTry = db.Episodes.FirstOrDefault(e => e.SeriesID == episode.SeriesID && e.Season == episode.Season + 1 && e.EpisodeNumber == 1);
                if (secondTry != null) {
                    return RedirectToAction("Watch", new { id = secondTry.EpisodeID });
                }
                var thirdTry = db.Episodes.FirstOrDefault(e => e.SeriesID == episode.SeriesID && e.Season == 1 && e.EpisodeNumber == 1);
                if (thirdTry != null) {
                    return RedirectToAction("Watch", new { id = thirdTry.EpisodeID });
                }
                return RandomEpisode(episode.SeriesID);
            }

        }

        public ActionResult Add(string msg=null) {
            using (var db = new TvTunerDataContext()) {
                var series = db.Series.OrderBy(s=>s.Name).ToList();
                ViewBag.Msg = msg;
                return View(series);
            }
        }
        [HttpPost]
        public ActionResult Add(string[] paths, int seriesID) {



            using (var db = new TvTunerDataContext()) {

                var show = db.Series.First(s => s.SeriesID == seriesID);


                var name = show.Name.Trim(new[]{ '!', '@', '#', '$', '%', '^', '&', '*', '`', '~'});

                var contentRoot = Server.MapPath("~/Content/Video");

                var dirs = Directory.GetDirectories(contentRoot);
                var bestChance = dirs.FirstOrDefault(d => d.Contains(name)|| name.Contains(d));
                if (bestChance == null) {
                    var directories = Directory.GetDirectories(Path.Combine(contentRoot, "HC"));

                    var bestCount = 0;
                    foreach (var directory in directories) {
                        var count = Path.GetFileNameWithoutExtension(directory).Split(new[] { ' ' }).Intersect(name.Split(new[] { ' ' })).Count();
                        if (count > bestCount) {
                            bestCount = count;
                            bestChance = directory;
                        }
                    }
                }

                var files = Directory.GetFiles(bestChance);
                if (files.Any()) {
                    for (var i = 0; i < paths.Length; i++) {
                        paths[i] = Path.Combine(bestChance, paths[i]);
                    }
                }

                if (name == "Archer") {
                    name = name + " (2009)";
                }
                string id;
                using (var wc = new WebClient()) {
                    var address = string.Format("http://thetvdb.com/api/GetSeries.php?seriesname={0}", GetSearchTerms(name));
                    var searchResults = XDocument.Parse(wc.DownloadString(address));

                    var series = searchResults.Descendants("Series").First();
                    id = series.Descendants("seriesid").First().Value;
                    address = string.Format("http://thetvdb.com/api/{0}/series/{1}/all/en.zip", TVDBKey.ApiKey, id);
                    var tempPath = Path.GetTempFileName();
                    wc.DownloadFile(address, tempPath);

                    var zip = ZipFile.Open(tempPath, ZipArchiveMode.Read);

                    var seriesXml = zip.GetEntry("en.xml");
                    var xmlStream = seriesXml.Open();
                    var xml = XDocument.Load(xmlStream);

                    var eps = xml.Descendants("Episode").ToList();
                    foreach (var path in paths) {


                        var regex = new Regex(@"[Ss](\d\d).*[Ee](\d\d)");
                        int season = 0, episode = 0;
                        var m = regex.Match(path);
                        if (m.Success) {
                            season = Convert.ToInt32(m.Groups[1].Value);
                            episode = Convert.ToInt32(m.Groups[2].Value);
                        }

                        var epsXml = eps.First(e => Convert.ToInt32(e.Descendants("SeasonNumber").First().Value) == season && Convert.ToInt32(e.Descendants("EpisodeNumber").First().Value) == episode);

                        var ep = new Episode { Season = season, 
                            EpisodeNumber = episode, 
                            Name = epsXml.Descendants("EpisodeName").First().Value, 
                            VideoPath = path, 
                            Summary = epsXml.Descendants("Overview").First().Value };
                        if (string.IsNullOrEmpty(ep.Summary)) {
                            ep.Summary = "no summary";
                        }
                        if (db.Episodes.Any(e => e.SeriesID == seriesID && e.Name == ep.Name)) {
                            continue;
                        }
                        var thumbPath = Path.Combine("http://thetvdb.com/banners", epsXml.Descendants("filename").First().Value);
                        ep.Thumbnail = wc.DownloadData(thumbPath).ToArray();
                        ep.Series = show;
                        db.Episodes.InsertOnSubmit(ep);
                    }
                    db.SubmitChanges();
                }
            }

            var msg = "";
            foreach (var path in paths) {
                msg += "Successfully added " + Path.GetFileName(path) + "\n";
            }
            
            
            return RedirectToAction("Add", new{msg=msg});
        }
        private static string GetSearchTerms(string text) {
            return text.Split(new[] { ' ' }).Aggregate((i, j) => i + "+" + j);
        }

        [HttpPost]
        public ActionResult AddSeries(string name) {
            using (var wc = new WebClient()) {
                var address = string.Format("http://thetvdb.com/api/GetSeries.php?seriesname={0}", GetSearchTerms(name));
                var searchResults = XDocument.Parse(wc.DownloadString(address));

                var series = searchResults.Descendants("Series").First();
                var id = series.Descendants("seriesid").First().Value;
                address = string.Format("http://thetvdb.com/api/{0}/series/{1}/all/en.zip", TVDBKey.ApiKey, id);
                var tempPath = Path.GetTempFileName();
                wc.DownloadFile(address, tempPath);

                var zip = ZipFile.Open(tempPath, ZipArchiveMode.Read);

                var seriesXml = zip.GetEntry("en.xml");
                var xmlStream = seriesXml.Open();
                var xml = XDocument.Load(xmlStream);

                var newSeries = GetSeries(xml);

                using (var db = new TvTunerDataContext()) {
                    if (db.Series.Any(s => s.Name == newSeries.Name)) {
                        return RedirectToAction("Add", new { msg = "Series " + newSeries.Name + " has already been added" });
                    }
                    db.Series.InsertOnSubmit(newSeries);
                    db.SubmitChanges();
                    return RedirectToAction("Add", new { msg = "Added series " + newSeries.Name });
                }

            }

            
        }
        private static Series GetSeries(XDocument xml) {
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
                var s = new Series { Name = name, Summary = summary };

                var wc = new WebClient();
                s.BannerImg = wc.DownloadData(bannerPath);
                return s;
            }
            return null;
        }

        public ActionResult Channels() {
            using (var db = new TvTunerDataContext()) {
                return View(db.Channels.Select(c => 
                    new ChannelModel(){
                        ChannelID = c.ChannelID, 
                        Name = c.Name,
                        SeriesIds = c.ChannelSeries.Select(cs=>cs.SeriesID).ToList()
                    }
                ).ToList());
            }
        }

        
    }

    public class ChannelModel {
        public int ChannelID { get; set; }
        public string Name { get; set; }
        public List<int> SeriesIds { get; set; } 
    }

    public class ShowModel {
        public Series Series { get; set; }
        public List<Episode> Episodes { get; set; }
    }

    public class IndexModel {
        public Dictionary<string, List<Series>> Series { get; set; }

    }
}
