using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using System.Linq;
using Spotify_Project.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Spotify_Project.Controllers
{
    public class GameController : Controller
    {
        private readonly SpotifyClient _spotify;
        private readonly ILogger<GameController> _logger; //temp

        public GameController(IHttpContextAccessor httpContextAccessor, ILogger<GameController> logger)
        {
            var accessToken = httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            _spotify = new SpotifyClient(accessToken);
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index method reached!");
            // Fetch user's playlists
            var playlists = await _spotify.Playlists.CurrentUsers();

            var songs = new List<Song>();

            // Fetch tracks from the first playlist as an example
            if (playlists.Items.Count > 0)
            {
                var firstPlaylist = playlists.Items.First();
                var playlistTracks = await _spotify.Playlists.GetItems(firstPlaylist.Id);

                foreach (var item in playlistTracks.Items)
                {
                    if (item.Track is FullTrack track)
                    {
                        songs.Add(new Song
                        {
                            Title = track.Name,
                            Artist = string.Join(", ", track.Artists.Select(a => a.Name)),
                            Album = track.Album.Name,
                            PreviewUrl = track.PreviewUrl
                        });
                        _logger.LogInformation($"Title: {track.Name}, Artist: {string.Join(", ", track.Artists.Select(a => a.Name))}, Preview URL: {track.PreviewUrl}");
                    }
                }
            }

            return View(songs); // Pass song data to the view
        }
    }
}
