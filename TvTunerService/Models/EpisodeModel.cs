using Nancy;
using TvTunerService.Infrastructure;

namespace TvTunerService.Models {
    public class EpisodeModel : ModelBase {
        public Episode Episode { get; set; }
        public EpisodeModel(NancyContext context, Episode episode)
            : base(context) {
            Episode = episode;
        }
    }
}