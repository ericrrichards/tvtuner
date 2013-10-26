namespace TvTuner.Controllers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class MoviesController : Controller {
        public ActionResult Index() {
            var db = new TvTunerDataContext();
            var movies = db.Movies.ToList();

            var moviesModel = new MoviesIndex() {
                movies = movies
            };
            return View(moviesModel);
        }
        public ActionResult GetMovieBanner(int id) {
            using (var db = new TvTunerDataContext()) {
                var movie= db.Movies.FirstOrDefault(s => s.MovieID == id);
                if (movie != null) {
                    return File(movie.Thumbnail.ToArray(), "img/jpeg");
                }
            }
            return null;
        }
        public ActionResult Watch(int id) {
            var db = new TvTunerDataContext();
            var movies = db.Movies.FirstOrDefault(e => e.MovieID == id);
            var home = new Uri(Server.MapPath(Request.Url.AbsolutePath));
            movies.VideoPath = home.MakeRelativeUri(new Uri(movies.VideoPath)).ToString();
            return View(movies);

        }
    }
        
    public class MoviesIndex {
        public List<Movies> movies { get; set; }
    }
}