using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UC3.Data;
using UC3.Models;
using UC3.Business;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace UC3.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly WorkoutContext _context;
    private readonly HomeService _homeService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IToastNotification _toastNotification;



    public HomeController(ILogger<HomeController> logger, WorkoutContext context, HomeService homeService, IHttpContextAccessor httpContextAccessor, IToastNotification itoastnotification)
    {
        _logger = logger;
        _context = context;
        _homeService = homeService;
        _httpContextAccessor = httpContextAccessor;
        _toastNotification = itoastnotification;
    }

    public IActionResult Index()
    {
        HttpContext.Session.SetString("Photo", "p1");
        var isLoggedIn = HttpContext.Session.GetString("IsLoggedIn");
        HttpContext.Session.SetString("Friends", "false");

        if (isLoggedIn == "true")
        {
            // Haal de huidige gebruiker op uit de database, bijvoorbeeld op basis van de userId uit de sessie
            var userId = HttpContext.Session.GetInt32("userId");
            var user = _context.UserModels.FirstOrDefault(u => u.userId == userId);

            // Stuur het user object naar de view
            if (user != null)
            {
                return View(user);
            }
            else
            {
                // Redirect naar Login als er geen gebruiker is
                return RedirectToAction("Login", "Account");
            }
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
        HttpContext.Session.SetString("Friends", "false");
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
            _toastNotification.AddWarningToastMessage("Voer een naam in");
            return RedirectToAction("Index");
        }

        // Zoek de gebruiker die overeenkomt met de naam
        var user = _context.UserModels.FirstOrDefault(u => u.name.Equals(searchTerm));

        if (user != null)
        {
            // Als een gebruiker is gevonden, stuur je door naar de profielpagina van deze gebruiker
            return RedirectToAction("Profile", new { userId = user.userId });
        }
        else
        {
            // Als er geen gebruiker is gevonden, geef dan een bericht en stuur de gebruiker terug naar de homepagina
            _toastNotification.AddWarningToastMessage("Deze gebruiker bestaat niet");
            return RedirectToAction("Index");
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
        HttpContext.Session.SetString("Friends", "true");
        return View(user); // Toon de profielpagina van de gebruiker
    }

    [HttpPost]
    public IActionResult UpdateBio(string bio)
    {
        try
        {
            // Haal de huidige ingelogde gebruiker op
            // (Dit is een voorbeeldimplementatie - pas aan naar je authenticatiemethode)
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Gebruiker niet ingelogd" });
            }

            // Haal de gebruiker op uit de database
            var user = _context.UserModels.Find(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Gebruiker niet gevonden" });
            }

            // Update de bio
            user.bio = bio;

            // Sla de wijzigingen op in de database
            _context.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            // Log de exception
            Console.WriteLine(ex.Message);
            return Json(new { success = false, message = ex.Message });
        }
    }
}
