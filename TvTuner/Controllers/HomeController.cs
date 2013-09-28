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
            var home = new Uri(Server.MapPath(Url.Content("~")));
            var indexModel = new IndexModel {
                Series = series, ShowEpisodes = series.ToDictionary(k => k.Name, v => v.Episodes.ToList())
            };

            foreach (var showEpisode in indexModel.ShowEpisodes) {
                foreach (var episode in showEpisode.Value) {
                    episode.VideoPath = home.MakeRelativeUri(new Uri(episode.VideoPath)).ToString();
                }
            }

            return View(indexModel);
        }
        
        public ActionResult About() {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
    public class IndexModel {
        public List<Series> Series { get; set; }
        public Dictionary<string, List<Episode>> ShowEpisodes { get; set; } 
    }
}
