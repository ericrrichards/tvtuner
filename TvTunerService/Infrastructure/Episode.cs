using System;
using System.Linq;
using System.Xml.Linq;

namespace TvTunerService.Infrastructure {
    public class Episode {
        private static int _nextID;
        public int ID { get; set; }
        public string Title { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string Filename { get; set; }
        public string Summary { get; set; }
        public string ThumbFilePath { get; set; }
        public Show Show { get; set; }

        private Episode(){}

        public Episode(Show s) {
            Show = s;
            ID = _nextID++;
        }

        public static Episode Parse(XElement xElement) {
            var summaryElem = xElement.Descendants("summary").FirstOrDefault();
            var thumbElem = xElement.Descendants("thumb").FirstOrDefault();
            var ret = new Episode {
                ID = Convert.ToInt32(xElement.Descendants("id").First().Value),
                Title = xElement.Descendants("title").First().Value,
                SeasonNumber = Convert.ToInt32(xElement.Descendants("season").First().Value),
                EpisodeNumber = Convert.ToInt32(xElement.Descendants("episodeNumber").First().Value),
                Filename = xElement.Descendants("filename").First().Value,
                Summary = summaryElem!= null ? summaryElem.Value : "",
                ThumbFilePath = thumbElem!=null ? thumbElem.Value : ""
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
                new XElement("filename", Filename),
                new XElement("summary", Summary),
                new XElement("thumb", ThumbFilePath)
                );
        }
    }
}