using Microsoft.Data.Sqlite;
using Dapper;
using UC3.Data;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace UC3.Business
{
    public class AccountService
    {
        private readonly WorkoutContext _context;

        public AccountService(WorkoutContext context)
        {
            _context = context;
        }

        public bool GetEmail(IQueryable email)
        {
            var dataEmail = _context.UserModels.Where(i => i.email.Length > 3);
            if (dataEmail == email)
            {
                return true;
            } else
            {
                return false;
            }
        }
        public bool GetPassword(IQueryable password)
        {
            var dataPassword = _context.UserModels.Where(i => i.password.Length > 3);
            if (dataPassword == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ValidLogin(IQueryable email, IQueryable password)
        {
            if (GetEmail(email) && GetPassword(password) == true) {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
