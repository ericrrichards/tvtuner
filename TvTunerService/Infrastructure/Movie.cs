using System;
using System.Linq;
using System.Xml.Linq;

namespace TvTunerService.Infrastructure {
    public class Movie {
        private static int _nextID;

        public int ID { get; set; }
        public string Title { get; set; }
        public string Filename { get; set; }

        private Movie() { }

        public Movie(string title) {
            ID = _nextID++;
            Title = title;
        }

        public static Movie Parse(XElement xElement) {
            var ret = new Movie() {
                ID = Convert.ToInt32(xElement.Descendants("id").First().Value),
                Title = xElement.Descendants("title").First().Value,
                Filename = xElement.Descendants("filename").First().Value
            };
            return ret;
        }

        public XElement ToXml() {
            return new XElement("movie",
                new XElement("id", ID),
                new XElement("title", Title),
                new XElement("filename", Filename)
                );
        }
    }
}