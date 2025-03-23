using UC3.Data;
using UC3.Models;
using System.Linq;

namespace UC3.Business
{
    public class HomeService
    {
        private readonly IWorkoutContext _context;

        public HomeService(IWorkoutContext context)
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
            if (bio == null)
            {
                return "";
            }
            else
            {
                return bio;
            }
        }
    }
}