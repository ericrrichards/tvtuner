using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TvTuner.Controllers {
    using System.ComponentModel;
    using System.Text;

    using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

    public class SeriesData {
        public SeriesData(int id, string name, string desc) {
            SeriesId = id;
            Name = name;
            Description = desc;
        }

        public int SeriesId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class MyApiController : Controller {
        public ActionResult GetShows() {

            using (var db = new TvTunerDataContext()) {
                var series = db.Series.OrderBy(s => s.Name);

                var dict = db.Categories.ToDictionary(k => k.CategoryName, v => new List<SeriesData>());
                dict["Miscellaneous"] = new List<SeriesData>();
                foreach (var s in series) {
                    var ds = new SeriesData(s.SeriesID, s.Name, s.Summary);

                    if (s.Category != null) {
                        dict[s.Category.CategoryName].Add(ds);
                    } else {
                        dict["Miscellaneous"].Add(ds);
                    }
                }
                return new JsonResult() { Data = new { showCategories = dict.ToArray() }, ContentEncoding = Encoding.UTF8, ContentType = "application/json", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
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

        public ActionResult GetEpisodes(int id) {
            using (var db = new TvTunerDataContext()) {
                var eps = db.Episodes.Where(e => e.SeriesID == id).OrderBy(e => e.Season).ThenBy(e => e.EpisodeNumber);
                var ret = eps.Select(e=>new {
                    e.EpisodeID,
                    e.Season,
                    e.EpisodeNumber,
                    e.Name,
                    e.Summary,
                }).ToArray();

                return new JsonResult() { Data = ret, ContentEncoding = Encoding.UTF8, ContentType = "application/json", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        public ActionResult GetStream(int id) {
            using (var db = new TvTunerDataContext()) {
                var episode = db.Episodes.FirstOrDefault(e => e.EpisodeID == id);
                var home = new Uri(Server.MapPath(Request.Url.AbsolutePath));
                var path = home.MakeRelativeUri(new Uri(episode.VideoPath)).ToString();
                return new JsonResult() { Data = path, ContentEncoding = Encoding.UTF8, ContentType = "application/json", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
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

    }

}
