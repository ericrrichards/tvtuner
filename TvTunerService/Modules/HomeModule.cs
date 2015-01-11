using System;
using System.IO;
using System.Linq;
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
            Get["/Shows/"] = Shows;
            Get["/Shows/Show/{id}"] = Show;
            Get["/Show/Watch/{id}"] = WatchEpisode;

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
        private dynamic Shows(dynamic parameters) {
            return View["Views/Shows/Index", new ShowIndexModel(Context, ShowRepository.Instance.Shows)];
        }
        private dynamic Show(dynamic parameters) {
            int id = parameters.id;
            return View["Views/Shows/Show", new ShowModel(Context, ShowRepository.Instance[id])];
        }
        private dynamic WatchEpisode(dynamic parameters) {
            int id = parameters.id;
            var episodeModel = new EpisodeModel(Context, ShowRepository.Instance.Episodes.First(e=>e.ID == id));
            episodeModel.Episode.Filename = "/" + TvTunerSvc.siteRoot + "/" + episodeModel.Episode.Filename;
            return View["Views/Shows/Watch", episodeModel];
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
                var filename = TorrentEngine.Instance.AddMagnetLink(model.MagnetLink, sanitizedShowName) + ".mp4";

                var show = ShowRepository.Instance[model.ShowName];
                if (show == null) {
                    ShowRepository.Instance.AddShow(new Show(model.ShowName));
                    show = ShowRepository.Instance[model.ShowName];
                }
                if (!show.HasEpisode(filename)) {
                    show.Episodes.Add(
                        new Episode(show) {
                            Title = Path.GetFileNameWithoutExtension(filename),
                            SeasonNumber = model.Season,
                            EpisodeNumber = model.EpisodeNumber,
                            Filename = filename
                        }
                    );
                }
                ShowRepository.Instance.SaveData();


                return NancyUtils.JsonResponse("Success " + filename);
            } catch (Exception ex) {
                Log.Error("Exception in " + ex.TargetSite.Name, ex);
                return NancyUtils.JsonResponse(ex.Message);
            }
        }
    }
}
