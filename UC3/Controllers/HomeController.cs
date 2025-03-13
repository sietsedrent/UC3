using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UC3.Data;
using UC3.Models;
using UC3.Business;
using Microsoft.EntityFrameworkCore;

namespace UC3.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly WorkoutContext _context;
    private readonly HomeService _homeService;
    private readonly IHttpContextAccessor _httpContextAccessor;



    public HomeController(ILogger<HomeController> logger, WorkoutContext context, HomeService homeService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _context = context;
        _homeService = homeService;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
        HttpContext.Session.SetString("Photo", "p1");
        var isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
        if (isLoggedIn == "true")
        {
            return View();
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }
    }

    public IActionResult Privacy()
    {
        var isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
        if (isLoggedIn == "true")
        {
            return View();
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }
    }
    public IActionResult Track()
    {
        HttpContext.Session.SetString("Photo", "p1");
        var isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
        if (isLoggedIn == "true")
        {
            return View();
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }
    }

    



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult UpdateProfilePicture(IFormFile profilePicture)
    {
        var useriddd = HttpContext.Session.GetInt32("userId");

        if (useriddd == null) return RedirectToAction("Login", "Account");

        var user = _context.UserModels.FirstOrDefault(u => u.userId == useriddd);
        if (user == null) return RedirectToAction("Index");

        if (profilePicture != null && profilePicture.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfilePictures");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{profilePicture.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                profilePicture.CopyTo(stream);
            }

            user.profilepicture = "/ProfilePictures/" + uniqueFileName;
            _context.SaveChanges();

            HttpContext.Session.SetString("profilepicture", user.profilepicture);
        }

        return RedirectToAction("Index");
    }

    [HttpPost] // Dit zorgt ervoor dat de zoekterm via een formulier naar de actie wordt verzonden
    public IActionResult Search(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            // Als de zoekterm leeg is, stuur je de gebruiker terug naar de homepagina
            return RedirectToAction("Index");
        }

        // Zoek de gebruiker die overeenkomt met de naam
        var user = _context.UserModels.FirstOrDefault(u => u.name.Contains(searchTerm));

        if (user != null)
        {
            // Als een gebruiker is gevonden, stuur je door naar de profielpagina van deze gebruiker
            return RedirectToAction("Profile", new { userId = user.userId });
        }
        else
        {
            // Als er geen gebruiker is gevonden, geef dan een bericht en stuur de gebruiker terug naar de homepagina
            ViewBag.Message = "User not found!";
            return View("Index"); // Dit zou de homepagina moeten zijn
        }
    }

    // Profielpagina van de gebruiker
    public IActionResult Profile(int userId)
    {
        var user = _context.UserModels.FirstOrDefault(u => u.userId == userId);
        if (user == null)
        {
            return RedirectToAction("Index");
        }
        return View(user); // Toon de profielpagina van de gebruiker
    }
}
