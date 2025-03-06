using MailAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace MailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        Random r = new Random();
        private readonly Contactform _contactform;
        private readonly HttpClient _httpClient;
        public MailController(Contactform contactform, HttpClient httpClient)
        {
            _contactform = contactform;
            _httpClient = httpClient;
        }
        [HttpPost]
        public ActionResult Post([Bind("authenticationCode, email")] Contactform form)
        {
            try
            {
                //var authcode = _contactform.randomNumber = r.Next(1000);
                var authcode = 14;
                HttpContext.Session.SetInt32("randomNumber", authcode);                
                var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
                {
                    Credentials = new NetworkCredential("ab6bf5befc4a02", "3ad7fd5780c303"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    //nog verbeteren
                    From = new MailAddress("123"),
                    Subject = "Authenticatiecode",
                    Body = $"authenticatiecode {authcode}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add("email");

                client.Send(mailMessage);

                System.Console.WriteLine("E-mail succesvol verzonden!");
                return Ok();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Er is een fout opgetreden bij het verzenden van de e-mail: {ex.Message}");
                return StatusCode(500, "Er ging iets mis bij het verzenden van de e-mail.");
            }
        }
    }
}
