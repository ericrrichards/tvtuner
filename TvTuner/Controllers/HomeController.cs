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
        public ActionResult Watch(int id) {
            var db = new TvTunerDataContext();
                var episode = db.Episodes.FirstOrDefault(e => e.EpisodeID == id);
                var home = new Uri(Server.MapPath(Request.Url.AbsolutePath));
                episode.VideoPath = home.MakeRelativeUri(new Uri(episode.VideoPath)).ToString();
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

                return RedirectToAction("Watch", new { id = episodes[next].EpisodeID });
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
