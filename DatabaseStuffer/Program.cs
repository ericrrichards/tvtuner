using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseStuffer {
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Windows.Forms;
    using System.Xml.Linq;

    class Program {
        private static string VideoPath;
        [STAThreadAttribute]
        static void Main(string[] args) {
            var dirPicker = new System.Windows.Forms.FolderBrowserDialog();
            if (dirPicker.ShowDialog() != DialogResult.OK) {
                return;
            }
            VideoPath = dirPicker.SelectedPath;


            var xml = XDocument.Load("en.xml");

            var series = GetSeries(xml);
            
            using (var db = new TbTunerDataContext()) {

                if (!db.Series.Any(s => s.Name == series.Name)) {
                    db.Series.InsertOnSubmit(series);
                }
                db.SubmitChanges();

                series = db.Series.First(f => f.Name == series.Name);
                var episodes = GetEpisodes(xml, series);
                foreach (var episode in episodes) {
                    if (db.Episodes.Any(e1 => e1.Name == episode.Name)) {

                    } else {
                        
                        db.Episodes.InsertOnSubmit(episode);
                        db.SubmitChanges();
                    }
                }

                db.SubmitChanges();

            }
        }

        private static List<Episode> GetEpisodes(XDocument xml, Series series) {
            var ret = new List<Episode>();
            var bads = new List<string>();
            var vidFiles = Directory.GetFiles(VideoPath, "*.mp4");
            var eps = xml.Descendants("Episode");
            var wc = new WebClient();
            foreach (var epElem in eps) {
                var ep = new Episode();
                ep.Season = Convert.ToInt32(epElem.Descendants("SeasonNumber").First().Value);
                ep.EpisodeNumber = Convert.ToInt32(epElem.Descendants("EpisodeNumber").First().Value);
                ep.Name = epElem.Descendants("EpisodeName").First().Value;
                
                ep.VideoPath = FindVideoFile(vidFiles, ep, series);

                if (ep.VideoPath == null) {
                    Console.WriteLine(ep.Name);
                    bads.Add(ep.Name);
                    continue;
                }

                ep.Summary = epElem.Descendants("Overview").First().Value;
                if (string.IsNullOrEmpty(ep.Summary)) {
                    ep.Summary = "no summary";
                }

                var thumbPath = Path.Combine("http://thetvdb.com/banners", epElem.Descendants("filename").First().Value);
                ep.Thumbnail = wc.DownloadData(thumbPath).ToArray();
                ep.Series = series;
                ret.Add(ep);
            }
            return ret;
        }

        private static string FindVideoFile(string[] vidFiles, Episode ep, Series series) {
            var bestMatch = vidFiles.FirstOrDefault(v => v.ToUpper().Contains(ep.Name.ToUpper()));
            if (!string.IsNullOrEmpty(bestMatch)) {
                return bestMatch;
            }
            bestMatch = vidFiles.FirstOrDefault(v =>
                                                (v.Contains("S" + ep.Season.ToString("D2")) ) &&
                                                (v.Contains("E" + ep.EpisodeNumber.ToString("D2"))));
            if (!string.IsNullOrEmpty(bestMatch)) {
                return bestMatch;
            }
            var words = ep.Name.Split(new[] { ' ', ':', ')', '(', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            words.Remove("the");
            words.Remove("of");

            foreach (var vidFile in vidFiles) {
                var contains = 0;
                var v = vidFile.Replace(series.Name, "");
                foreach (var word in words) {
                    if (v.Contains(word)) {
                        contains++;
                    }
                }
               
                if (contains >= words.Count * 0.7) {
                    bestMatch = vidFile;
                    return vidFile;
                }
            }


            return null;
        }

        private static Series GetSeries(XDocument xml) {
            var series = xml.Descendants("Series").FirstOrDefault();
            if (series != null) {
                string bannerPath = null;
                string name = null;
                string summary = null;
                var nameElem = series.Descendants("SeriesName").FirstOrDefault();

                if (nameElem != null) {
                    name = nameElem.Value;
                }
                var bannerElem = series.Descendants("banner").FirstOrDefault();

                if (bannerElem != null) {
                    bannerPath = Path.Combine("http://thetvdb.com/banners", bannerElem.Value);
                }
                var summaryElem = series.Descendants("Overview").FirstOrDefault();
                if (summaryElem != null) {
                    summary = summaryElem.Value;
                }
                var s = new Series { Name = name, Summary = summary };

                var wc = new WebClient();
                s.BannerImg = wc.DownloadData(bannerPath);
                return s;
            }
            return null;
        }
    }
}
