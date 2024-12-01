using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace Spotify_Project.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            var loginRequest = new LoginRequest(
                new Uri("https://localhost:7237/callback"), // Updated Redirect URI
                "2e450fda8e8d4ff594d80e4345035f7d",
                LoginRequest.ResponseType.Code
            )
            {
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative }
            };

            var uri = loginRequest.ToUri();
            return Redirect(uri.ToString());
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return View("Error", error); // Display an error view if needed
            }

            var tokenRequest = new AuthorizationCodeTokenRequest(
                "2e450fda8e8d4ff594d80e4345035f7d",
                "4099efbf05324a12b86361d626c3b53d",
                code,
                new Uri("https://localhost:7237/callback") // Updated Redirect URI
            );

            var oAuthClient = new OAuthClient();
            var tokenResponse = await oAuthClient.RequestToken(tokenRequest);

            // Save tokens in session
            HttpContext.Session.SetString("AccessToken", tokenResponse.AccessToken);
            HttpContext.Session.SetString("RefreshToken", tokenResponse.RefreshToken);

            return RedirectToAction("Index", "Game");
        }
    }
}
