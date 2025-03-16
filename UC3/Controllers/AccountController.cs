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
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace UC3.Controllers
{
    public class AccountController : Controller
    {
        private readonly WorkoutContext _context;
        private readonly AccountService _accountService;
        private readonly HttpClient _httpClient;
        private readonly IToastNotification _toastNotification;
        private readonly IConfiguration _configuration;

        public AccountController(WorkoutContext context, AccountService accountService, HttpClient httpClient, IToastNotification iToastNotification, IConfiguration configuration)
        {
            _context = context;
            _accountService = accountService;
            _httpClient = httpClient;
            _toastNotification = iToastNotification;
            _configuration = configuration;
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
                if (HttpContext.Session.GetString("inCooldown") == "true")
                {
                    _toastNotification.AddErrorToastMessage($"Druk alstublieft eens per 30 seconden");
                    return View();
                }

                HttpContext.Session.SetString("inCooldown", "true");
                Task.Run(async () =>
                {
                    await Task.Delay(30000);
                    HttpContext.Session.SetString("inCooldown", "false");
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
                HttpContext.Session.SetInt32("userId", user.userId);
                HttpContext.Session.SetString("email", user.email);
                HttpContext.Session.SetString("name", user.name);
                HttpContext.Session.SetString("password", user.password);
                HttpContext.Session.SetString("profilepicture", user.profilepicture);

                // Genereer 4-cijferige verificatiecode (tussen 1000-9999)
                var authcode = r.Next(1000, 10000);
                HttpContext.Session.SetInt32("randomNumber", authcode);

                try
                {
                    // Stuur e-mail met verificatiecode
                    await SendVerificationEmail(user.email, authcode);
                    _toastNotification.AddSuccessToastMessage($"Verificatiecode is verstuurd naar {user.email}");
                }
                catch (Exception ex)
                {
                    // Als e-mail versturen mislukt, toon de code alsnog
                    _toastNotification.AddWarningToastMessage($"E-mail kon niet worden verstuurd. De verificatiecode is: {authcode}");
                }

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
                if (verrieverrie == null || verrieverrie == 0)
                {
                    _toastNotification.AddWarningToastMessage("Verificatie is verplicht");
                    return View();
                }
                if (verrieverrie != currentVericode)
                {
                    _toastNotification.AddWarningToastMessage("De verificatiecode was onjuist");
                    return View();
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Methode om verificatie-email te versturen
        private async Task SendVerificationEmail(string toEmail, int verificationCode)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var mail = new MimeMessage();

            mail.From.Add(new MailboxAddress(emailSettings["DisplayName"], emailSettings["Mail"]));
            mail.To.Add(MailboxAddress.Parse(toEmail));

            mail.Subject = "Je verificatiecode voor WorkoutLogger";

            var body = new TextPart("html")
            {
                Text = $@"
                    <h1>Verificatiecode</h1>
                    <p>Gebruik de volgende code om in te loggen op WorkoutLogger:</p>
                    <h2 style='background-color: #f0f0f0; padding: 10px; text-align: center;'>{verificationCode}</h2>
                    <p>Deze code is 10 minuten geldig.</p>
                    <p>Als je geen toegang hebt aangevraagd, kun je deze e-mail negeren.</p>
                "
            };

            mail.Body = body;

            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(
                    emailSettings["Host"],
                    int.Parse(emailSettings["Port"]),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(emailSettings["Mail"], emailSettings["Password"]);
                await smtp.SendAsync(mail);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log de fout of gooi deze door
                throw new Exception($"Fout bij het verzenden van e-mail: {ex.Message}", ex);
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
            if ((from u in _context.UserModels where u.email == email select u).Count() != 0)
            {
                _toastNotification.AddWarningToastMessage("Emailadres is al in gebruik");
                return View();
            }
            if ((from u in _context.UserModels where u.name == name select u).Count() != 0)
            {
                _toastNotification.AddWarningToastMessage("Username is al in gebruik");
                return View();
            }

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