using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TvTuner.Controllers {
    using System.IO;

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

                return RedirectToAction("Watch", new { id = episodes[next].EpisodeID, random=true });
            }
        }

        public ActionResult NextEpisode(int id) {
            using (var db = new TvTunerDataContext()) {
                var episode = db.Episodes.FirstOrDefault(e => e.EpisodeID == id);
                var firstTry = db.Episodes.FirstOrDefault(e => e.SeriesID == episode.SeriesID && e.Season == episode.Season && e.EpisodeNumber == episode.EpisodeNumber + 1);
                if (firstTry != null) {
                    return RedirectToAction("Watch", new { id = firstTry.EpisodeID });
                }

                var secondTry = db.Episodes.FirstOrDefault(e => e.SeriesID == episode.SeriesID && e.Season == episode.Season+1 && e.EpisodeNumber ==  1);
                if (secondTry != null) {
                    return RedirectToAction("Watch", new { id = secondTry.EpisodeID });
                }
                var thirdTry = db.Episodes.FirstOrDefault(e => e.SeriesID == episode.SeriesID && e.Season ==  1 && e.EpisodeNumber == 1);
                if (thirdTry != null) {
                    return RedirectToAction("Watch", new { id = thirdTry.EpisodeID });
                }
                return RandomEpisode(episode.SeriesID);
            }

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
