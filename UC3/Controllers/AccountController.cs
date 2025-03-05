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
        public async Task<IActionResult> Login(string? email, string? password)
        {
            if (email == null || password == null)
            {
                return NotFound();
            }
            if ((from u in _context.UserModels where u.email == email select u).Count() == 0)
            {
                ModelState.AddModelError("", "This email does not exist in our database");
                return View();
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
            }
            else
            {
                ModelState.AddModelError("", "The username or password is incorrect");
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
        public async Task<IActionResult> Register(string? email, string? password, string? name)
        {
            //overbodig
            var user = await _context.UserModels
            .FirstOrDefaultAsync(m => m.email == email);

            if (email != null && password != null && name != null)
            {
                _accountService.Register(email, password, name);
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        //Logout
        public IActionResult Logout()
        {
            return View();
        }
    }
}
