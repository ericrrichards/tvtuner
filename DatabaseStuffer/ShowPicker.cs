using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseStuffer {
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Xml.Linq;

    public partial class ShowPicker : Form {
        public ShowPicker(string videoPath) {
            InitializeComponent();

            lblVideoPath.Text = videoPath;

            txtShow.Text = Path.GetFileName(videoPath);
            lbResults.DisplayMember = "Item1";
        }

        private void btnLookup_Click(object sender, EventArgs e) {
            using (var wc = new WebClient()) {
                var address = string.Format("http://thetvdb.com/api/GetSeries.php?seriesname={0}", GetSearchTerms(txtShow.Text));
                var searchResults = XDocument.Parse(wc.DownloadString(address));

                var series = searchResults.Descendants("Series");
                lbResults.Items.Clear();
                foreach (var seri in series) {
                    var id = seri.Descendants("seriesid").First().Value;
                    var name = seri.Descendants("SeriesName").First().Value;
                    lbResults.Items.Add(new Tuple<string, string>(name, id));
                }
            }
        }

        private static string GetSearchTerms(string text) {
            return text.Split(new[] { ' ' }).Aggregate((i, j) => i + "+" + j);
        }

        private void btnGetData_Click(object sender, EventArgs e) {
            using (var wc = new WebClient()) {
                var address = string.Format("http://thetvdb.com/api/{0}/series/{1}/all/en.zip", TVDBKey.ApiKey, ((Tuple<string, string>)lbResults.SelectedItem).Item2);

                var tempPath = Path.GetTempFileName();

                wc.DownloadFile(address, tempPath);

                var zip = ZipFile.Open(tempPath, ZipArchiveMode.Read);

                var seriesXml = zip.GetEntry("en.xml");
                XmlStream = seriesXml.Open();

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public Stream XmlStream { get; set; }
    }
}
