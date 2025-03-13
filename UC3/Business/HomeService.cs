using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using UC3.Data;
using UC3.Models;

namespace UC3.Business
{
    public class HomeService
    {
        private readonly WorkoutContext _context;
        public HomeService(WorkoutContext context)
        {
            _context = context;
        }

        public void setBio(string newBio, User user)
        {
            if (user != null)
            {
                user.bio = newBio;
                _context.SaveChanges();
            }
        }
        public string getBio(int user)
        {
            
            var bio = _context.UserModels.Where(i => i.userId == user).Select(i => i.bio).FirstOrDefault();
            if(bio == null)
            {
                return "";
            } else
            {
                return bio;
            }
        }

    }
}
