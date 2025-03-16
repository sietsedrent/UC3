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

        public TrackController(WorkoutContext context)
        {
            _context = context;
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

            // Haal workouts op uit de database met WorkoutContext
            var workouts = await _context.WorkoutModels
                .Where(w => w.userId == userId)
                .ToListAsync();

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

            // Haal workout op met alle gerelateerde gegevens
            var workout = await _context.WorkoutModels
                .Where(w => w.workoutId == id && w.userId == userId)
                .FirstOrDefaultAsync();

            if (workout == null)
            {
                return Json(new { error = "Workout niet gevonden" });
            }

            // Haal alle trainingsgegevens op voor deze workout
            var trainingDataList = await _context.TrainingDataModels
                .Where(td => td.workoutId == id)
                .ToListAsync();

            // Maak lijst om oefeningen en bijbehorende trainingsgegevens te verzamelen
            var exercisesList = new List<object>();

            foreach (var trainingData in trainingDataList)
            {
                // Haal oefening op voor deze trainingsgegevens
                var exercise = await _context.ExerciseModels
                    .FirstOrDefaultAsync(e => e.exerciseId == trainingData.exerciseId);

                if (exercise != null)
                {
                    // Voeg oefening toe aan de lijst met bijbehorende trainingsgegevens
                    exercisesList.Add(new
                    {
                        exerciseName = exercise.exerciseName,
                        muscleGroup = exercise.muscleGroup,
                        trainingData = new
                        {
                            amountOfSets = trainingData.amountOfSets,
                            amountOfReps = trainingData.amountOfReps,
                            liftedWeight = trainingData.liftedWeight,
                            e1RM = trainingData.e1RM,
                            pr = trainingData.pr
                        }
                    });
                }
            }

            // Stel het volledige resultaat samen
            var result = new
            {
                workoutId = workout.workoutId,
                typeWorkout = workout.typeWorkout,
                workoutDate = workout.workoutDate.ToString("yyyy-MM-dd"),
                comments = workout.comments,
                exercises = exercisesList
            };

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveWorkout([FromBody] WorkoutViewModel workoutViewModel)
        {
            // Haal de gebruikers-id op uit de sessie
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return Unauthorized(); // Of een andere gepaste actie
            }

            // 1. Eerste de workout opslaan en de workoutId ophalen
            var workout = new Workout
            {
                userId = userId.Value,
                workoutDate = DateOnly.Parse(workoutViewModel.workoutDate),
                typeWorkout = workoutViewModel.typeWorkout,
                comments = workoutViewModel.comments
            };

            _context.WorkoutModels.Add(workout);
            await _context.SaveChangesAsync();

            // Nu hebben we een workoutId
            int workoutId = workout.workoutId;

            // 2. Voor elke exercise in de viewmodel:
            foreach (var exerciseVM in workoutViewModel.exercises)
            {
                // a. Zoek de exercise op basis van naam of maak een nieuwe aan
                var exercise = await _context.ExerciseModels
                    .FirstOrDefaultAsync(e => e.exerciseName == exerciseVM.exerciseName);

                if (exercise == null)
                {
                    // Maak een nieuwe exercise aan als deze nog niet bestaat
                    exercise = new Exercise
                    {
                        exerciseName = exerciseVM.exerciseName,
                        muscleGroup = exerciseVM.muscleGroup
                    };

                    _context.ExerciseModels.Add(exercise);
                    await _context.SaveChangesAsync();
                }

                // b. Controleer of dit een PR is door te vergelijken met eerdere resultaten
                bool isPR = false;
                var calculatedE1RM = CalculateE1RM(exerciseVM.trainingData.liftedWeight, exerciseVM.trainingData.amountOfReps);

                var previousTrainingData = await _context.TrainingDataModels
                    .Where(td => td.exerciseId == exercise.exerciseId)
                    .ToListAsync();

                if (!previousTrainingData.Any() || previousTrainingData.All(td => td.e1RM < calculatedE1RM))
                {
                    isPR = true;
                }

                // c. Maak TrainingData aan en koppel aan de workout en exercise
                var trainingData = new TrainingData
                {
                    workoutId = workoutId,
                    exerciseId = exercise.exerciseId,
                    amountOfSets = exerciseVM.trainingData.amountOfSets,
                    amountOfReps = exerciseVM.trainingData.amountOfReps,
                    liftedWeight = exerciseVM.trainingData.liftedWeight,
                    e1RM = calculatedE1RM,
                    pr = isPR
                };

                _context.TrainingDataModels.Add(trainingData);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Track", "Home");
        }

        // Helper methode om geschatte 1 rep max te berekenen (Brzycki formule)
        private float CalculateE1RM(float weight, int reps)
        {
            if (reps == 1) return weight;
            return weight * (36 / (37 - reps));
        }
    }

    // ViewModel klassen voor het verwerken van de formuliergegevens
    public class WorkoutViewModel
    {
        public string typeWorkout { get; set; }
        public string workoutDate { get; set; }
        public string comments { get; set; }
        public List<ExerciseViewModel> exercises { get; set; }
    }

    public class ExerciseViewModel
    {
        public string exerciseName { get; set; }
        public string muscleGroup { get; set; }
        public TrainingDataViewModel trainingData { get; set; }
    }

    public class TrainingDataViewModel
    {
        public int amountOfSets { get; set; }
        public int amountOfReps { get; set; }
        public float liftedWeight { get; set; }
    }
}