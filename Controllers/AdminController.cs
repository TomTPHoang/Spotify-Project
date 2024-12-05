using Microsoft.AspNetCore.Mvc;
using Spotify_Project.Models;

namespace Spotify_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "password")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard");
            }

            ViewBag.ErrorMessage = "Invalid credentials. Please try again.";
            return View();
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }

            var userStats = _context.UserStats.ToList();
            return View(userStats);
        }
    }
}
