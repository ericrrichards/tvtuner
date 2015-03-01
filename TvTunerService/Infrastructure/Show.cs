using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TvTunerService.Infrastructure {
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

        public IEnumerable<Episode> Season(int seasonNumber) {
            return Episodes.Where(e => e.SeasonNumber == seasonNumber).OrderBy(e => e.EpisodeNumber);
        }
        public IEnumerable<int> Seasons { get { return Episodes.Select(e => e.SeasonNumber).Distinct().OrderBy(a=>a); } }

        public Episode FirstEpisode {
            get { return Season(Seasons.Min()).FirstOrDefault(); }
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
}