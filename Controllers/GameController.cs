using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using Spotify_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotify_Project.Controllers
{
    public class GameController : Controller
    {
        private readonly SpotifyClient _spotify;

        public GameController(IHttpContextAccessor httpContextAccessor)
        {
            // Retrieve the AccessToken from session
            var accessToken = httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                // Log the missing token and redirect to login if not found
                Console.WriteLine("Access Token is missing from the session.");
                throw new Exception("Access Token is not available. Redirect user to /Auth/Login to authenticate.");
            }

            // Initialize the Spotify client
            _spotify = new SpotifyClient(accessToken);
        }

        public async Task<IActionResult> Index()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Access Token is missing from the session.");
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.AccessToken = accessToken; // Pass the token to the view
            Console.WriteLine($"Access Token fetched in GameController: {accessToken}");
            var songs = new List<Song>();

            try
            {
                // Fetch user's playlists and songs as before
                var playlists = await _spotify.Playlists.CurrentUsers();
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
                                PreviewUrl = track.PreviewUrl,
                                SpotifyUri = track.Uri
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Spotify data: {ex.Message}");
            }

            return View(songs); // Pass songs to the view
        }

        public async Task<IActionResult> RandomSong()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Access Token is missing. Redirecting to login.");
                return RedirectToAction("Login", "Auth");
            }

            var songs = new List<Song>();

            try
            {
                var playlists = await _spotify.Playlists.CurrentUsers();
                if (playlists?.Items == null || playlists.Items.Count == 0)
                {
                    Console.WriteLine("No playlists found for the user.");
                    return Json(null);
                }

                // Filter out null playlists
                var validPlaylists = playlists.Items.Where(p => p != null).ToList();
                if (validPlaylists.Count == 0)
                {
                    Console.WriteLine("No valid playlists available.");
                    return Json(null);
                }

                // Select a random playlist
                var randomPlaylist = validPlaylists[new Random().Next(validPlaylists.Count)];
                Console.WriteLine($"Selected Playlist: {randomPlaylist.Name}");

                // Fetch tracks from the selected playlist
                Console.WriteLine($"Fetching tracks from playlist: {randomPlaylist.Name} (ID: {randomPlaylist.Id})");
                var playlistTracks = await _spotify.Playlists.GetItems(randomPlaylist.Id);
                foreach (var item in playlistTracks.Items)
                {
                    if (item.Track is FullTrack track)
                    {
                        songs.Add(new Song
                        {
                            Title = track.Name,
                            Artist = string.Join(", ", track.Artists.Select(a => a.Name)),
                            Album = track.Album.Name,
                            PreviewUrl = track.PreviewUrl,
                            SpotifyUri = track.Uri
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching Spotify data: {ex.Message}");
            }

            if (!songs.Any())
            {
                Console.WriteLine("No songs found in the playlist.");
                return Json(null);
            }

            // Return a random song from the list
            var randomSong = songs[new Random().Next(songs.Count)];
            Console.WriteLine($"Selected Song: {randomSong.Title}, URI: {randomSong.SpotifyUri}");
            return Json(randomSong);
        }

    }
}
