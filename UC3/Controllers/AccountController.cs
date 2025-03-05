using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UC3.Data;
using UC3.Models;

namespace UC3.Controllers
{
    public class AccountController : Controller
    {
        private readonly WorkoutContext _context;

        public AccountController(WorkoutContext context)
        {
            _context = context;
        }

        //GET Login
        public IActionResult Login()
        {
            return View();
        }

        //POST Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? email)
        {
            if (email == null)
            {
                return NotFound();
            }

            var user = await _context.UserModels
                .FirstOrDefaultAsync(m => m.email == email);
            if (email == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Home");
        }

        //GET Register
        public IActionResult Register()
        {
            return View();
        }

        // POST Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("email,password,name")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");

        }

        //Logout
        public IActionResult Logout()
        {
            return View();
        }
    }
}
