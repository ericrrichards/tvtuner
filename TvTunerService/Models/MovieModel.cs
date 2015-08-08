using Nancy;
using TvTunerService.Infrastructure;

namespace TvTunerService.Models {
    public class MovieModel : ModelBase {
        public Movie Movie { get; set; }
        public MovieModel(NancyContext context, Movie movie)
            : base(context) {
            Movie = movie;
        }
    }
}