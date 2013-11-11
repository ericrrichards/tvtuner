namespace TvTuner.Controllers {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class MoviesController : Controller {
        public ActionResult Index(string genre = "") {
            var db = new TvTunerDataContext();
            List<Movies> movies;
            if (genre == "") {
                movies = db.Movies.ToList();
            } else {
                movies = db.Movies.Where(m=>m.Genre == genre).ToList();
            }
        

        var moviesModel = new MoviesIndex() {
                movies = movies,
                Genres = db.Movies.Select(m => m.Genre).Distinct().ToList()
                
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

        public ActionResult Search(string fragment) {
            var db = new TvTunerDataContext();
            var results = db.Movies.Where(m => m.Title.Contains(fragment));
            ViewBag.SearchTerm = fragment;
            return View(results);
        }
    }
        
    public class MoviesIndex {
        public List<Movies> movies { get; set; }
        public List<string> Genres { get; set; }

        public MoviesIndex() {
            Genres = new List<string>();
            Genres.Add("foo");
            Genres.Add("bar");
        }
    }
}