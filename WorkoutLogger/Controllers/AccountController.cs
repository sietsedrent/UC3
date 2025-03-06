using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WorkoutLogger.Data;
using WorkoutLogger.Models;
using WorkoutLogger.Business;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;



namespace WorkoutLogger.Controllers
{
    public class AccountController : Controller
    {
        private readonly WorkoutContext _context;
        private readonly AccountService _accountService;
        private readonly HttpClient _httpClient;

        public AccountController(WorkoutContext context, AccountService accountService, HttpClient httpClient)
        {
            _context = context;
            _accountService = accountService;
            _httpClient = httpClient;
            //Moet ook nog HTTPClient httpclient in initialisatie
            _httpClient.BaseAddress = new Uri("https://localhost:7205");
        }

        //GET Login
        public IActionResult Login()
        {
            return View();
        }

        //POST Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? email, string? password, int? vericode)
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
        public async Task<IActionResult> Register(string? email, string? password, string? name, User user)
        {
            //overbodig
            var users = await _context.UserModels
            .FirstOrDefaultAsync(m => m.email == email);

            if (email != null && password != null && name != null)
            {
                _accountService.Register(email, password, name);
                return RedirectToAction("Login", "Account");
            }
            return View();
        }


        // POST: SendAuthenticationController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMail(User user)
        {

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(user, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:7205/api/mail", content);       


            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Message = $"Er is iets misgegaan. Statuscode: {response.StatusCode}, Response inhoud: {response.Content}";
                return View();
            }

            ViewBag.Message = "De verificatiecode is verstuurd";
            ModelState.Clear();

            return View();
        }
        


        //Logout
        public IActionResult Logout()
        {
            return View();
        }
    }
}
