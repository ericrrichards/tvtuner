using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieStuffer {
    using System.IO;
    using System.Net;

    using Newtonsoft.Json.Linq;

    using TvTuner;

    public partial class Form1 : Form {
        private string _filename;
        private string _apiKey = "vzysu6m2sy5rcrxfedmav7v3";

        public Form1() {
            InitializeComponent();
            listBox1.DisplayMember = "Item1";
        }

        private void button1_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                _filename = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            using (var wc = new WebClient()) {
                var movieUrl = "http://api.rottentomatoes.com/api/public/v1.0/movies.json?apikey={0}&q={1}&page_limit=10";
                var movieWords = Path.GetFileNameWithoutExtension(_filename).Split(new[] { '.', '-', '(', ')', '[', ']', '_' }, StringSplitOptions.RemoveEmptyEntries);
                var address = string.Format(movieUrl, _apiKey, movieWords.Aggregate((i, j) => i + "+" + j));
                var json = wc.DownloadString(address);

                var ret = JObject.Parse(json);
                var movies = ret["movies"];
                while (!movies.Any() && movieWords.Any()) {
                    movieWords = movieWords.Take(movieWords.Count() - 1).ToArray();
                    address = string.Format(movieUrl, _apiKey, movieWords.Aggregate((i, j) => i + "+" + j));
                    json = wc.DownloadString(address);

                    ret = JObject.Parse(json);
                    movies = ret["movies"];

                }
                foreach (var movie in movies) {
                    listBox1.Items.Add(new Tuple<string, JToken>(movie["title"].ToString() + "[" + movie["year"] + "]", movie));
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            using (var db = new TvTuner.TvTunerDataContext()) {
                var movie = new Movies();
                var json = ((Tuple<string, JToken>)listBox1.SelectedItem).Item2;
                movie.Title = json["title"].ToString();
                movie.Year = Convert.ToInt32(json["year"].ToString());
                movie.Summary = (json["critics_consensus"]?? "").ToString();
                movie.Rating = Convert.ToInt32(json["ratings"]["critics_score"].ToString());
                using (var wc = new WebClient()) {
                    movie.Thumbnail = wc.DownloadData(json["posters"]["detailed"].ToString()).ToArray();
                }
                movie.VideoPath = _filename;

                db.Movies.InsertOnSubmit(movie);
                db.SubmitChanges();
            }
            MessageBox.Show("movie added");
            _filename = null;
            listBox1.Items.Clear();
        }
    }
}
