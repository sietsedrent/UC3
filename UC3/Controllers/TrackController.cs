using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using UC3.Models;

namespace UC3.Controllers
{
    public class TrackController : Controller
    {
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
        public JsonResult GetWorkouts()
        {
            // Haal workouts op uit de database (voorbeeld data)
            var workouts = new List<Workout>
    {
        new Workout {
            workoutId = 1,
            userId = 1,
            workoutDate = DateOnly.Parse("2023-10-01"), // Converteer string naar DateOnly
            typeWorkout = "Hardlopen",
            comments = "Goede run!"
        },
        new Workout {
            workoutId = 2,
            userId = 1,
            workoutDate = DateOnly.Parse("2023-10-03"), // Converteer string naar DateOnly
            typeWorkout = "Fietsen",
            comments = "Mooie route"
        },
        new Workout {
            workoutId = 3,
            userId = 1,
            workoutDate = DateOnly.Parse("2023-10-05"), // Converteer string naar DateOnly
            typeWorkout = "Gewichtheffen",
            comments = "Zware training"
        }
    };

            return Json(workouts);
        }

        [HttpGet]
        public JsonResult GetWorkoutDetails(int workoutId)
        {
            // Haal details van een specifieke workout op (voorbeeld data)
            var workout = new Workout { workoutId = workoutId, userId = 1, workoutDate = DateOnly.FromDateTime(DateTime.Now), typeWorkout = "Hardlopen", comments = "Goede run!" };

            return Json(workout);
        }

        [HttpPost]
        public IActionResult SaveWorkout([FromBody] Workout workout)
        {
            // Sla de workout op in de database
            // Voor nu retourneren we gewoon een OK status
            return Ok();
        }
    }
}