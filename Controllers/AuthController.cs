using Microsoft.AspNetCore.Mvc;
using Spotify_Project.Models;
using SpotifyAPI.Web;

namespace Spotify_Project.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var loginRequest = new LoginRequest(
                new Uri("https://localhost:7237/callback"), // Redirect URI
                clientId, // Retrieve Client ID from environment variable
                LoginRequest.ResponseType.Code
            )
            {
                Scope = new[]
                {
                    Scopes.PlaylistReadPrivate,
                    Scopes.PlaylistReadCollaborative,
                    Scopes.Streaming,
                    Scopes.UserReadPlaybackState,
                    Scopes.UserModifyPlaybackState
                }
            };

            var uri = loginRequest.ToUri();
            return Redirect(uri.ToString());
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var tokenRequest = new AuthorizationCodeTokenRequest(
                Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID"),
                Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET"),
                code,
                new Uri("https://localhost:7237/callback")
            );

            var oAuthClient = new OAuthClient();
            var tokenResponse = await oAuthClient.RequestToken(tokenRequest);

            // Log the retrieved token
            Console.WriteLine($"Access Token retrieved in AuthController: {tokenResponse.AccessToken}");
            Console.WriteLine($"Refresh Token retrieved in AuthController: {tokenResponse.RefreshToken}");

            // Save tokens in session
            HttpContext.Session.SetString("AccessToken", tokenResponse.AccessToken);
            HttpContext.Session.SetString("RefreshToken", tokenResponse.RefreshToken);

            return RedirectToAction("Index", "Game");
        }

    }
}
