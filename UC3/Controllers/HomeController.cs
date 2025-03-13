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


}
