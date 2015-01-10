using System;
using System.Reflection;
using log4net;
using Nancy;
using Nancy.ModelBinding;
using TvTunerService.Infrastructure;
using TvTunerService.Models;

namespace TvTunerService.Modules {
    public class HomeModule : NancyModule{
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HomeModule() {
            Get["/"] = Index;
            Get["/BrowseEZTV"] = BrowseEZTV;
            Get["/EZTV/SearchShows"] = SearchEZTVShows;
            Get["/EZTV/GetEpisodes"] = GetEpisodes;
            Post["/EZTV/AddToLibrary"] = AddToLibrary;
        }

        private dynamic Index(dynamic parameters) {
            return View["Views/Home/Index", new ModelBase(Context)];
        }
        private dynamic BrowseEZTV(dynamic parameters) {
            return View["Views/Home/BrowseEztv", new ModelBase(Context)];
        }
        private dynamic SearchEZTVShows(dynamic parameters) {
            string searchFragment = Request.Query["searchTerm"];

            var shows = EZTV.EZTV.GetShows(searchFragment);

            return NancyUtils.JsonResponse(shows);
        }
        private dynamic GetEpisodes(dynamic parameters) {
            int showId = Request.Query["showId"];

            var episodes = EZTV.EZTV.GetEpisodes(showId);

            return NancyUtils.JsonResponse(episodes);
        }
        private dynamic AddToLibrary(dynamic parameters) {
            try {
                var model = this.Bind<AddToLibraryModel>();
                var sanitizedShowName = model.ShowName.Replace(" ", "_");
                TorrentEngine.Instance.AddMagnetLink(model.MagnetLink, sanitizedShowName);

                return NancyUtils.JsonResponse("Success");
            } catch (Exception ex) {
                Log.Error("Exception in " + ex.TargetSite.Name, ex);
                return NancyUtils.JsonResponse(ex.Message);
            }
        }
    }

    public class AddToLibraryModel {
        public string ShowName { get; set; }
        public string MagnetLink { get; set; }
    }
}
