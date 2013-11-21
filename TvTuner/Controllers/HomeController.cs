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
            var series = db.Series.ToList();

            var indexModel = new IndexModel {
                Series = series,
            };


            return View(indexModel);
        }

        public ActionResult GetSeriesBanner(int id) {
            using (var db = new TvTunerDataContext()) {
                var show = db.Series.FirstOrDefault(s => s.SeriesID == id);
                if (show != null) {
                    return File(show.BannerImg.ToArray(), "img/jpeg");
                }
            }
            return null;
        }

        public ActionResult Show(int id) {
            using (var db = new TvTunerDataContext()) {
                var show = db.Series.FirstOrDefault(s => s.SeriesID == id);
                var model = new ShowModel() { Series = show };
                model.Episodes = show.Episodes.OrderBy(e => e.Season).ToList();
                return View(model);
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
                var series = db.Series.ToList();
                ViewBag.Msg = msg;
                return View(series);
            }
        }
        [HttpPost]
        public ActionResult Add(string path, int seriesID) {



            using (var db = new TvTunerDataContext()) {

                var show = db.Series.First(s => s.SeriesID == seriesID);


                var name = show.Name;

                var contentRoot = Server.MapPath("~/Content/Video");

                var dirs = Directory.GetDirectories(contentRoot);
                var bestChance = dirs.First(d => d.Contains(name));

                var files = Directory.GetFiles(bestChance, path);
                if (files.Count() == 1) {
                    path = Path.GetFullPath(files[0]);
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

                    var eps = xml.Descendants("Episode");
                    var regex = new Regex(@"[Ss](\d\d).*[Ee](\d\d)");
                    int season = 0, episode = 0;
                    var m = regex.Match(path);
                    if (m.Success) {
                        season = Convert.ToInt32(m.Groups[1].Value);
                        episode = Convert.ToInt32(m.Groups[2].Value);
                    }

                    var epsXml = eps.First(e => Convert.ToInt32(e.Descendants("SeasonNumber").First().Value) == season && 
                        Convert.ToInt32(e.Descendants("EpisodeNumber").First().Value) == episode);

                    var ep = new Episode();
                    ep.Season = season;
                    ep.EpisodeNumber = episode;
                    ep.Name = epsXml.Descendants("EpisodeName").First().Value;
                    ep.VideoPath = path;
                    ep.Summary = epsXml.Descendants("Overview").First().Value;
                    if (string.IsNullOrEmpty(ep.Summary)) {
                        ep.Summary = "no summary";
                    }
                    var thumbPath = Path.Combine("http://thetvdb.com/banners", epsXml.Descendants("filename").First().Value);
                    ep.Thumbnail = wc.DownloadData(thumbPath).ToArray();
                    ep.Series = show;
                    db.Episodes.InsertOnSubmit(ep);
                    db.SubmitChanges();
                }
            }





            return RedirectToAction("Add", new{msg="Successfully added " + Path.GetFileName(path)});
        }
        private static string GetSearchTerms(string text) {
            return text.Split(new[] { ' ' }).Aggregate((i, j) => i + "+" + j);
        }
    }

    public class ShowModel {
        public Series Series { get; set; }
        public List<Episode> Episodes { get; set; }
    }

    public class IndexModel {
        public List<Series> Series { get; set; }
    }
}
