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
            return View("~/Views/Home/NewWorkout.cshtml");
        }

        [HttpGet]
        public async Task<JsonResult> GetWorkouts()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Json(new { error = "Niet ingelogd" });
            }

            var workouts = await _workoutService.GetWorkoutsForUser(userId.Value);
            return Json(workouts);
        }

        [HttpGet]
        public async Task<JsonResult> GetWorkoutDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Json(new { error = "Niet ingelogd" });
            }

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
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Unauthorized(); 
            }

            await _workoutService.SaveWorkout(workoutDTO, userId.Value);

            return RedirectToAction("Track", "Home");
        }
    }
}