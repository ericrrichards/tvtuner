namespace TvTunerService.Models {
    public class UpdateShowModel {
        public string ShowName { get; set; }
        public string Summary { get; set; }
        public string BannerUrl { get; set; }
        public int EpisodeSeason { get; set; }
        public int EpisodeNumber { get; set; }
        public string EpisodeTitle { get; set; }
        public string EpisodeSummary { get; set; }
        public string EpisodeThumb { get; set; }
        public int OriginalEpisodeId { get; set; }
    }
}