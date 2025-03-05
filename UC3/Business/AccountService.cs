using Microsoft.Data.Sqlite;
using Dapper;
using UC3.Data;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;

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
            if ((from u in _context.UserModels where u.email == email select u).Count() > 0) 
            {
            } else
            {
                return false;
            }

            bool emailAndPassword = _context.UserModels.Any(i => i.email == email && i.password == password);

            if (emailAndPassword)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
