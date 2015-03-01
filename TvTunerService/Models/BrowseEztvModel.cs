using System.Collections.Generic;
using EZTV;
using Nancy;

namespace TvTunerService.Models {
    public class BrowseEztvModel : ModelBase {
        public List<EZTVShow> Shows { get; set; }
        public EZTVEpisodeList Episodes { get; set; }

        public BrowseEztvModel(NancyContext context):base(context) {
        }
    }
}