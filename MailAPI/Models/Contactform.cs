using System.ComponentModel.DataAnnotations;

namespace MailAPI.Models
{
    public class Contactform
    {
        public string authenticationCode { get; set; }
        public string email { get; set; }
        public int randomNumber {  get;  set; }
    }
}
