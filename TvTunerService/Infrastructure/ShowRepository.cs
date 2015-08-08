using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;

namespace TvTunerService.Infrastructure {
    class ShowRepository {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal List<Show> Shows { get; set; }

        public List<Episode> Episodes {
            get { return Shows.SelectMany(s => s.Episodes).ToList(); }
        }

        public List<Movie> Movies { get; set; } 

        private static readonly ShowRepository _instance = new ShowRepository();
        public static ShowRepository Instance { get { return _instance; } }

        private ShowRepository() {
            Shows = new List<Show>();
            Movies = new List<Movie>();
            if (!File.Exists("showData.xml")) {
                var file = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("shows"));
                file.Save("showData.xml");
            }
            var doc = XDocument.Load("showData.xml");
            var shows = doc.Descendants("show");
            foreach (var show in shows) {
                Shows.Add(Show.Parse(show));
            }
            try {
                var movies = doc.Descendants("movie");
                foreach (var movie in movies) {
                    Movies.Add(Movie.Parse(movie));
                }
            } catch (Exception ex) {
                Log.Error("Exception in " + ex.TargetSite.Name, ex);
            }
        }

        public void SaveData() {
            var file = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), 
                new XElement("root",
                    new XElement("shows", Shows.Select(s=>s.ToXml())),
                    new XElement("movies", Movies.Select(m=>m.ToXml()))
                )
            );
            file.Save("showData.xml");
        }

        public Show this[string name] {
            get { return Shows.FirstOrDefault(s => s.Name == name); }
        }
        public Show this[int id] {
            get { return Shows.FirstOrDefault(s => s.ID == id); }
        }
        public void AddShow(Show s) {
            Shows.Add(s);
        }
    }
}
