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



namespace UC3.Controllers
{
    public class AccountController : Controller
    {
        private readonly WorkoutContext _context;
        private readonly AccountService _accountService;
        //private readonly HttpClient _httpClient;

        public AccountController(WorkoutContext context, AccountService accountService)
        {
            _context = context;
            _accountService = accountService;
            //Moet ook nog HTTPClient httpclient in initialisatie
            //_httpClient = httpClient;
            //_httpClient.BaseAddress = new Uri("https://localhost:7205");
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
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMail(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "De ingevulde velden voldoen niet aan de gestelde voorwaarden";
                return View();
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(user, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //Gebruik _httpClient om een POST-request te doen naar ShowcaseAPI die de Mail uiteindelijk verstuurt met Mailtrap (of een alternatief).
            //Verstuur de gegevens van het ingevulde formulier mee aan de API, zodat dit per mail verstuurd kan worden naar de ontvanger.
            //Hint: je kunt dit met één regel code doen. Niet te moeilijk denken dus. :-)
            //Hint: vergeet niet om de mailfunctionaliteit werkend te maken in ShowcaseAPI > Controllers > MailController.cs,
            //      nadat je een account hebt aangemaakt op Mailtrap (of een alternatief).

            HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:7205/api/mail", content);             // Vervang deze regel met het POST-request


            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Message = $"Er is iets misgegaan. Statuscode: {response.StatusCode}, Response inhoud: {response.Content}";
                return View();
            }

            // Looking to send emails in production? Check out our Email API/SMTP product!



            ViewBag.Message = "Het contactformulier is verstuurd";
            ModelState.Clear();

            return View();
        }
        */


        //Logout
        public IActionResult Logout()
        {
            return View();
        }
    }
}
