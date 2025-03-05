using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UC3.Data;
using UC3.Models;
using UC3.Business;



namespace UC3.Controllers
{
    public class AccountController : Controller
    {
        private readonly WorkoutContext _context;
        private readonly AccountService _accountService;
        public AccountController(WorkoutContext context, AccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        //GET Login
        public IActionResult Login()
        {
            return View();
        }

        //POST Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string? password)
        {
            if (email == null || password == null)
            {
                return NotFound();
            }

            var user = await _context.UserModels
                .FirstOrDefaultAsync(m => m.email == email);
            if (user == null)
            {
                return NotFound();
            }
            
            if (_accountService.ValidLogin(email, password) == true) 
            {
                return RedirectToAction("Index", "Home");
            } else
            {
                return View();
            }



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
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Login", "Account");

        }

        //Logout
        public IActionResult Logout()
        {
            return View();
        }
    }
}
