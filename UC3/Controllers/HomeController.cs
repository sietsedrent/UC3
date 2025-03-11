using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UC3.Models;

namespace UC3.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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
}
