using Nancy;
using TvTunerService.Infrastructure;

namespace TvTunerService.Models {
    public class ShowModel : ModelBase {
        public Show Show { get; set; }
        public ShowModel(NancyContext context, Show show)
            : base(context) {
            Show = show;
        }
    }
}