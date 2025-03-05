using MailAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace MailAPI.Controllers
{
    public class MailController : ControllerBase
    {
        [HttpPost]
        public ActionResult Post([Bind("FirstName, LastName, Email, Phone, Subject, Description")] Contactform form)
        {
            try
            {
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
                    Body = $"authenticatiecode",
                    IsBodyHtml = false
                };

                mailMessage.To.Add("to@example.com");

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
