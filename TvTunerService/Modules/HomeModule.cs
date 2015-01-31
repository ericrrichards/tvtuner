using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using EZTV;
using log4net;
using Nancy;
using Nancy.ModelBinding;
using TvTunerService.Infrastructure;
using TvTunerService.Models;

namespace TvTunerService.Modules {
    public class HomeModule : NancyModule {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string ShowBannerDir = "Content/images/showBanners";

        public HomeModule() {
            Get["/"] = Index;
            Get["/BrowseEZTV"] = BrowseEZTV;
            Get["/Shows/"] = Shows;
            Get["/Shows/Show/{id}"] = Show;
            Get["/Show/Watch/{id}"] = WatchEpisode;

            Get["/EZTV/SearchShows"] = SearchEZTVShows;
            Get["/EZTV/GetEpisodes"] = GetEpisodes;
            Post["/EZTV/AddToLibrary"] = AddToLibrary;

            Post["/shows/show/update/{ShowName}"] = UpdateShow;
            Get["/Video/{episodeID}"] = Video;
            Get["/ShowInformation/{name}"] = ShowInformation;
            Get["/EpisodeInformation/{id}"] = EpisodeInformation;

        }

        private dynamic Index(dynamic parameters) {
            return View["Views/Home/Index", new ModelBase(Context)];
        }
        private dynamic BrowseEZTV(dynamic parameters) {
            string searchFragment = Request.Query["q"];
            bool showEpisodes = false;
            if (Request.Query["episodes"] != null) {
                bool.TryParse(Request.Query["episodes"], out showEpisodes);
            }

            var results = new List<EZTVShow>();
            if (!string.IsNullOrWhiteSpace(searchFragment)) {
                results = EZTV.EZTV.GetShows(searchFragment.Trim());
            }
            EZTVEpisodeList episodes = null;
            if (results.Count == 1 && showEpisodes) {
                episodes = EZTV.EZTV.GetEpisodes(results.First().Id);
                var myShowEpisodes = ShowRepository.Instance[episodes.ShowTitle].Episodes;
                foreach (var episode in episodes.Episodes) {
                    EZTVEpisode episode1 = episode;
                    if ( myShowEpisodes.Any(e=>e.SeasonNumber == episode1.Season && e.EpisodeNumber == episode1.EpisodeNum && e.Filename.Contains(episode1.Title.Replace(' ', '.')))) {
                        episode.InLibrary = true;
                    }
                }
            }

            var model = new BrowseEztvModel(Context) {
                Shows = results,
                Episodes = episodes
            };
            return View["Views/Home/BrowseEztv", model];
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
            var episodeModel = new EpisodeModel(Context, ShowRepository.Instance.Episodes.First(e => e.ID == id));
            //episodeModel.Episode.Filename = "/" + TvTunerSvc.siteRoot + "/" + episodeModel.Episode.Filename;
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
                int id = -1;
                if (!show.HasEpisode(filename)) {
                    var episode = new Episode(show) {
                        Title = Path.GetFileNameWithoutExtension(filename),
                        SeasonNumber = model.Season,
                        EpisodeNumber = model.EpisodeNumber,
                        Filename = filename
                    };
                    id = episode.ID;
                    show.Episodes.Add(
                        episode
                    );
                }
                ShowRepository.Instance.SaveData();


                return NancyUtils.JsonResponse("Success " + filename + " " + id);
            } catch (Exception ex) {
                Log.Error("Exception in " + ex.TargetSite.Name, ex);
                return NancyUtils.JsonResponse(ex.Message);
            }
        }

        private dynamic UpdateShow(dynamic parameters) {
            try {
                var model = this.Bind<UpdateShowModel>();

                Show show = ShowRepository.Instance[parameters.ShowName.ToString()];
                if (show == null) {
                    show = new Show(parameters.ShowName.ToString());
                    ShowRepository.Instance.AddShow(show);
                }
                show.Summary = model.Summary;
                var bannerUrl = model.BannerUrl;
                var index = bannerUrl.IndexOf("/Content/");


                show.BannerImg = "~" + bannerUrl.Substring(index);

                var episode = show.Episodes.FirstOrDefault(e => e.ID == model.OriginalEpisodeId);
                if (episode != null) {
                    episode.Title = model.EpisodeTitle;
                    episode.Summary = model.EpisodeSummary;
                    episode.ThumbFilePath = model.EpisodeThumb;
                    episode.EpisodeNumber = model.EpisodeNumber;
                    episode.SeasonNumber = model.EpisodeSeason;
                } else if (ShowRepository.Instance.Episodes.Any(e => e.ID == model.OriginalEpisodeId)) {
                    episode = ShowRepository.Instance.Episodes.First(e => e.ID == model.OriginalEpisodeId);
                    episode.Show.Episodes.Remove(episode);
                    episode.Show = show;
                    episode.Title = model.EpisodeTitle;
                    episode.Summary = model.EpisodeSummary;
                    episode.ThumbFilePath = model.EpisodeThumb;
                    episode.EpisodeNumber = model.EpisodeNumber;
                    episode.SeasonNumber = model.EpisodeSeason;
                    show.Episodes.Add(episode);
                }

                ShowRepository.Instance.SaveData();

                return NancyUtils.JsonResponse("Success");
            } catch (Exception ex) {
                Log.Error("Exception in " + ex.TargetSite.Name, ex);
                return NancyUtils.JsonResponse(ex.Message);
            }
        }

        private dynamic Video(dynamic parameters) {
            int id = parameters.episodeID;
            var path = ShowRepository.Instance.Episodes.First(e => e.ID == id).Filename;

            return Response.FromPartialFile(Request, path, "video/mp4");
        }
        private dynamic ShowInformation(dynamic parameters) {
            string showName = parameters.name;
            var series = ShowRepository.Instance[showName];
            if (series != null) {
                var name = series.Name;
                var summary = series.Summary;
                string imgDlPath = null;
                if (!string.IsNullOrWhiteSpace(series.BannerImg)) {
                    imgDlPath = series.BannerImg.Substring(2);
                }
                var s = new { Name = name, Summary = summary, BannerPath = imgDlPath };

                return NancyUtils.JsonResponse(s);
            }
            return NancyUtils.JsonResponse("No result found");
        }

        private dynamic EpisodeInformation(dynamic parameters) {
            int id = parameters.id;
            var episode = ShowRepository.Instance.Episodes.FirstOrDefault(e => e.ID == id);
            if (episode != null) {
                return NancyUtils.JsonResponse(new {
                    episode.SeasonNumber, 
                    episode.EpisodeNumber,
                    episode.Title,
                    episode.Summary,
                    episode.ThumbFilePath
                });
            }
            return NancyUtils.JsonResponse("No result found");
        }
    }
}



