using Microsoft.AspNetCore.Mvc;

namespace Spotify_Project.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
