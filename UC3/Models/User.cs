using System.ComponentModel.DataAnnotations;

namespace UC3.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }


        [Display(Name = "Email Address")]
        [EmailAddress]
        [RegularExpression("^[_A-Za-z'`+-.]+([_A-Za-z0-9'+-.]+)*@([A-Za-z0-9-])+(\\.[A-Za-z0-9]+)*(\\.([A-Za-z]*){3,})$", ErrorMessage = "Enter proper email")]
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail voldoet niet aan de eisen")]
        public string email { get; set; }


        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Wachtwoord voldoet niet aan de eisen")]
        public string password { get; set; }
        public string name { get; set; }
        public string bio { get; set; }
        public string profilepicture { get; set; }
        public DateOnly RegisteryDate { get; set; }
        public int role { get; set; }
        
    }
}
