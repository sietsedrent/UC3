using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UC3.Models;
using UC3.Business;
using UC3.Data;
using Microsoft.AspNetCore.Http;

namespace UC3.Controllers
{
    public class TrackController : Controller
    {
        private readonly WorkoutContext _context;
        private readonly WorkoutService _workoutService;

        public TrackController(WorkoutContext context, WorkoutService workoutService)
        {
            _context = context;
            _workoutService = workoutService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewWorkout()
        {
            // Retourneer de view vanuit de Home-folder
            return View("~/Views/Home/NewWorkout.cshtml");
        }

        [HttpGet]
        public async Task<JsonResult> GetWorkouts()
        {
            // Haal de gebruikers-id op uit de sessie
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Json(new { error = "Niet ingelogd" });
            }

            // Gebruik de WorkoutService om workouts op te halen
            var workouts = await _workoutService.GetWorkoutsForUser(userId.Value);
            return Json(workouts);
        }

        [HttpGet]
        public async Task<JsonResult> GetWorkoutDetails(int id)
        {
            // Haal de gebruikers-id op uit de sessie
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Json(new { error = "Niet ingelogd" });
            }

            // Gebruik de WorkoutService om workout details op te halen
            var result = await _workoutService.GetWorkoutDetails(id, userId.Value);

            if (result == null)
            {
                return Json(new { error = "Workout niet gevonden" });
            }

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveWorkout([FromBody] WorkoutDTO workoutDTO)
        {
            // Haal de gebruikers-id op uit de sessie
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Unauthorized(); // Of een andere gepaste actie
            }

            // Gebruik de WorkoutService om de workout op te slaan
            await _workoutService.SaveWorkout(workoutDTO, userId.Value);

            return RedirectToAction("Track", "Home");
        }
    }
}