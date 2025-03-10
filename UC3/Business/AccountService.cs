using Microsoft.Data.Sqlite;
using Dapper;
using UC3.Data;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;
using UC3.Models;

namespace UC3.Business
{
    public class AccountService
    {
        private readonly WorkoutContext _context;
        public AccountService(WorkoutContext context)
        {
            _context = context;
        }

        public bool ValidLogin(string email, string password)
        {
            

            bool emailAndPassword = _context.UserModels.Any(i => i.email == email && i.password == password);

            if (emailAndPassword)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public void Register(string email, string password, string name)
        {
            if ((from u in _context.UserModels where u.email == email select u).Count() == 0)
            {
                var registerDate = DateOnly.FromDateTime(DateTime.Now);
                var newUser = new User
                {
                    email = email,
                    password = password,
                    name = name,
                    bio = "empty",
                    profilepicture = "empty",
                    RegisteryDate = registerDate,
                    role = 1

                };
                _context.UserModels.Add(newUser);
                _context.SaveChanges();
            }
            else
            {
                return;
            }
            
        }
    }
}
