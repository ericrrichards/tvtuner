using System.Collections.Generic;
using Nancy;
using TvTunerService.Infrastructure;

namespace TvTunerService.Models {
    public class ShowIndexModel : ModelBase {
        public List<Show> Shows { get; set; } 
        public ShowIndexModel(NancyContext context, List<Show> shows) : base(context) {
            Shows = shows;
        }
    }

    public class MoviesIndexModel : ModelBase {
        public List<Movie> Movies { get; set; }

        public MoviesIndexModel(NancyContext context, List<Movie> movies) : base(context) {
            Movies = movies;
        }
    }
}