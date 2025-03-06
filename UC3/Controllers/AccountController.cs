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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using NToastNotify;




namespace UC3.Controllers
{
    public class AccountController : Controller
    {
        private readonly WorkoutContext _context;
        private readonly AccountService _accountService;
        private readonly HttpClient _httpClient;
        private readonly IToastNotification _toastNotification;
        public bool inCooldown;

        public AccountController(WorkoutContext context, AccountService accountService, HttpClient httpClient, IToastNotification iToastNotification)
        {
            _context = context;
            _accountService = accountService;
            _httpClient = httpClient;
            _toastNotification = iToastNotification;
            _httpClient.BaseAddress = new Uri("https://localhost:7205");
        }

        //GET Login
        public IActionResult Login()
        {
            return View();
        }

        // POST Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? email, string? password, int vericode, string action)
        {
            if (vericode != null && vericode != 0)
            {
                HttpContext.Session.SetInt32("vericode", vericode);
            }

            Random r = new Random();

            if (action == "Send verification")
            {
                if (inCooldown)
                {
                    _toastNotification.AddErrorToastMessage($"Druk alstublieft eens per 5 seconden");
                    return View();
                }

                inCooldown = true;
                Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    inCooldown = false;
                });

                if (email == null || password == null)
                {
                    return NotFound();
                }

                var user = await _context.UserModels.FirstOrDefaultAsync(m => m.email == email);
                if (user == null)
                {
                    ModelState.AddModelError("", "This email does not exist in our database");
                    return View();
                }


                if (!_accountService.ValidLogin(email, password))
                {
                    ModelState.AddModelError("", "The username or password is incorrect");
                    return View();
                }

                // Sla gebruiker gegevens op in de sessie
                HttpContext.Session.SetString("userId", user.userId.ToString());
                HttpContext.Session.SetString("email", user.email);
                HttpContext.Session.SetString("name", user.name);
                HttpContext.Session.SetString("password", user.password);

                var authcode = r.Next(1000);
                HttpContext.Session.SetInt32("randomNumber", authcode);

                _toastNotification.AddSuccessToastMessage($"De verificatiecode is {authcode}");

                // Verzend verificatiecode (implementatie API-aanroep)
                //Schijnbaar stuurt mailtrap niet naar andere mails
                //var json = JsonConvert.SerializeObject(user, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                //var content = new StringContent(JsonConvert.SerializeObject(new
                //{
                //    email = user.email
                //}), Encoding.UTF8, "application/json");
                //HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:7205/api/mail", content);

                //if (!response.IsSuccessStatusCode)
                //{
                //    ViewBag.Message = $"Er is iets misgegaan. Statuscode: {response.StatusCode}, Response inhoud: {response.Content}";
                //    return View();
                //}
                return View();

            }

            if (action == "Log in")
            {
                
                string storedEmail = HttpContext.Session.GetString("email");
                string storedPassword = HttpContext.Session.GetString("password");
                if (storedEmail == null)
                {
                    _toastNotification.AddErrorToastMessage($"Verificatie is verplicht");
                    return View();
                }
                HttpContext.Session.SetString("IsLoggedIn", "true");


                // Controleer verificatiecode
                var currentVericode = HttpContext.Session.GetInt32("randomNumber");
                var verrieverrie = HttpContext.Session.GetInt32("vericode");
                if (verrieverrie != currentVericode)
                {
                    _toastNotification.AddWarningToastMessage("De verificatiecode was onjuist");
                    return View();
                }    
                return RedirectToAction("Index", "Home");
            }
            return View();
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



        //Logout
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("IsLoggedIn", "false");
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
