using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TvTunerService.Infrastructure {
    class ShowRepository {
        internal List<Show> Shows { get; set; }

        public List<Episode> Episodes {
            get { return Shows.SelectMany(s => s.Episodes).ToList(); }
        }

        private static readonly ShowRepository _instance = new ShowRepository();
        public static ShowRepository Instance { get { return _instance; } }

        private ShowRepository() {
            Shows = new List<Show>();
            if (!File.Exists("showData.xml")) {
                var file = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("shows"));
                file.Save("showData.xml");
            }
            var doc = XDocument.Load("showData.xml");
            var shows = doc.Descendants("show");
            foreach (var show in shows) {
                Shows.Add(Show.Parse(show));
            }
        }

        public void SaveData() {
            var file = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), 
                new XElement("shows", Shows.Select(s=>s.ToXml()))
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

    public class Show {
        private static int _nextID;

        public int ID { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string BannerImg { get; set; }
        public List<Episode> Episodes { get; set; }

        private Show() {
            Episodes = new List<Episode>();
        }

        public Show(string name) :this() {
            Name = name;
            ID = _nextID++;
        }

        public bool HasEpisode(string filename) {
            return Episodes.Any(e => e.Filename == filename);
        }

        public static Show Parse(XElement show) {
            var summaryElem = show.Descendants("summary").FirstOrDefault();
            var bannerElem = show.Descendants("bannerImg").FirstOrDefault();
            var ret = new Show {
                ID = Convert.ToInt32(show.Descendants("id").First().Value),
                Name = show.Descendants("name").First().Value,
                Summary = summaryElem != null ? summaryElem.Value : "",
                BannerImg = bannerElem != null ? bannerElem.Value : "",
                Episodes = show.Descendants("episode").Select(Episode.Parse).ToList()
            };
            foreach (var episode in ret.Episodes) {
                episode.Show = ret;
            }

            if (ret.ID >= _nextID) {
                _nextID = ret.ID + 1;
            }
            return ret;
        }

        public XElement ToXml() {
            return new XElement("show", 
                new XElement("id", ID),
                new XElement("name", Name),
                new XElement("summary", Summary),
                new XElement("bannerImg", BannerImg),
                new XElement("episodes", Episodes.Select(e=>e.ToXml()))
            );
        }
    }

    public class Episode {
        private static int _nextID;
        public int ID { get; set; }
        public string Title { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string Filename { get; set; }
        public Show Show { get; set; }

        private Episode(){}

        public Episode(Show s) {
            Show = s;
            ID = _nextID++;
        }

        public static Episode Parse(XElement xElement) {
            var ret = new Episode {
                ID = Convert.ToInt32(xElement.Descendants("id").First().Value),
                Title = xElement.Descendants("title").First().Value,
                SeasonNumber = Convert.ToInt32(xElement.Descendants("season").First().Value),
                EpisodeNumber = Convert.ToInt32(xElement.Descendants("episodeNumber").First().Value),
                Filename = xElement.Descendants("filename").First().Value
            };
            if (ret.ID >= _nextID) {
                _nextID = ret.ID + 1;
            }
            return ret;
        }

        public XElement ToXml() {
            return new XElement("episode",
                new XElement("id", ID),
                new XElement("title", Title),
                new XElement("season", SeasonNumber),
                new XElement("episodeNumber", EpisodeNumber),
                new XElement("filename", Filename)
            );
        }
    }
}
