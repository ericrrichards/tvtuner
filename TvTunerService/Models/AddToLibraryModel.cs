using TvTunerService.Annotations;

namespace TvTunerService.Models {
    [UsedImplicitly]
    public class AddToLibraryModel {
        public string ShowName { get; set; }
        public string MagnetLink { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
    }
}