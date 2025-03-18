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
            var userId = HttpContext.Session.GetInt32("userId");
            var user = _context.UserModels.FirstOrDefault(u => u.userId == userId);

            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
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


    [HttpGet]
    public async Task<IActionResult> GetWorkoutPlannings(int userId)
    {
        var now = DateTime.Now;
        var date = new DateTime(now.Year, now.Month, now.Day);
        date = date.AddDays(3 - (int)(date.DayOfWeek + 6) % 7);
        var week1 = new DateTime(date.Year, 1, 4);
        int weekNumber = 1 + (int)((date - week1).TotalDays / 7);

        var plannings = await _context.WorkoutPlanningModels
            .Where(w => w.UserId == userId && w.WeekNumber == weekNumber)
            .ToListAsync();

        return Json(new { plannings = plannings.Select(p => p.DayOfWeek).ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateWorkoutPlanning(int dayIndex, bool hasWorkout)
    {
        var userId = HttpContext.Session.GetInt32("userId");
        if (userId == null)
        {
            return Json(new { success = false, message = "Niet ingelogd" });
        }

        var now = DateTime.Now;
        var date = new DateTime(now.Year, now.Month, now.Day);
        date = date.AddDays(3 - (int)(date.DayOfWeek + 6) % 7);
        var week1 = new DateTime(date.Year, 1, 4);
        int weekNumber = 1 + (int)((date - week1).TotalDays / 7);

        var existingPlanning = await _context.WorkoutPlanningModels
            .FirstOrDefaultAsync(w => w.UserId == userId && w.DayOfWeek == dayIndex && w.WeekNumber == weekNumber);

        if (hasWorkout)
        {
            if (existingPlanning == null)
            {
                _context.WorkoutPlanningModels.Add(new WorkoutPlanning
                {
                    UserId = userId.Value,
                    DayOfWeek = dayIndex,
                    WeekNumber = weekNumber
                });
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            if (existingPlanning != null)
            {
                _context.WorkoutPlanningModels.Remove(existingPlanning);
                await _context.SaveChangesAsync();
            }
        }

        return Json(new { success = true });
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

    [HttpPost] 
    public IActionResult Search(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            _toastNotification.AddWarningToastMessage("Voer een naam in");
            return RedirectToAction("Index");
        }

        var user = _context.UserModels.FirstOrDefault(u => u.name.Equals(searchTerm));

        if (user != null)
        {
            return RedirectToAction("Profile", new { userId = user.userId });
        }
        else
        {
            _toastNotification.AddWarningToastMessage("Deze gebruiker bestaat niet");
            return RedirectToAction("Index");
        }
    }

    public IActionResult Profile(int userId)
    {
        var user = _context.UserModels.FirstOrDefault(u => u.userId == userId);
        if (user == null)
        {
            return RedirectToAction("Index");
        }
        HttpContext.Session.SetString("Friends", "true");
        return View(user); 
    }

    [HttpPost]
    public IActionResult UpdateBio(string bio)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Gebruiker niet ingelogd" });
            }

            var user = _context.UserModels.Find(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Gebruiker niet gevonden" });
            }

            user.bio = bio;

            _context.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Json(new { success = false, message = ex.Message });
        }
    }
}
