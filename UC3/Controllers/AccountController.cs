﻿using System;
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
        public async Task<IActionResult> Login(string? email, string? password, int? vericode, string action)
        {
            
            if (action == "Send verification")
            {
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


                TempData["email"] = email;
                TempData["password"] = password;

                _toastNotification.AddSuccessToastMessage("De verificatiecode is verstuurd");

                // Verzend verificatiecode (implementatie API-aanroep)
                var json = JsonConvert.SerializeObject(user, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:7205/api/mail", content);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Message = $"Er is iets misgegaan. Statuscode: {response.StatusCode}, Response inhoud: {response.Content}";
                    return View();
                }
                return View();
            }

            if (action == "Log in")
            {
                string storedEmail = TempData["email"]?.ToString();
                string storedPassword = TempData["password"]?.ToString();

                var user = await _context.UserModels.FirstOrDefaultAsync(m => m.email == storedEmail);

                // Controleer verificatiecode
                var currentVericode = HttpContext.Session.GetInt32("randomNumber");
                if (vericode != currentVericode)
                {
                    _toastNotification.AddWarningToastMessage("De verificatiecode was onjuist");
                    return View();
                }

                
                    // Sla gebruiker gegevens op in de sessie
                    HttpContext.Session.SetString("userId", user.userId.ToString());
                    HttpContext.Session.SetString("email", user.email);
                    HttpContext.Session.SetString("name", user.name);
                    HttpContext.Session.SetString("IsLoggedIn", "true");

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
