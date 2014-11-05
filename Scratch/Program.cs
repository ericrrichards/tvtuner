using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scratch {
    using DatabaseStuffer;

    class Program {
        static void Main(string[] args) {
            using (var db = new TbTunerDataContext()) {
                var movie = db.Movies.First(m => m.MovieID == 256);

                var episode = new Episode() {
                    EpisodeNumber = 1,
                    Season = 0,
                    SeriesID = 53,
                    Name = movie.Title,
                    Summary = movie.Summary,
                    Thumbnail = movie.Thumbnail,
                    VideoPath = movie.VideoPath,
                };
                db.Episodes.InsertOnSubmit(episode);
                db.SubmitChanges();
            }
        }
    }
}
