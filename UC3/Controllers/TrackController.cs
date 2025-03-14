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
                    workoutDate = DateOnly.Parse("2023-10-01"),
                    typeWorkout = "Hardlopen",
                    comments = "Goede run!"
                },
                new Workout {
                    workoutId = 2,
                    userId = 1,
                    workoutDate = DateOnly.Parse("2023-10-03"),
                    typeWorkout = "Fietsen",
                    comments = "Mooie route"
                },
                new Workout {
                    workoutId = 3,
                    userId = 1,
                    workoutDate = DateOnly.Parse("2023-10-05"),
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
        public IActionResult SaveWorkout([FromBody] WorkoutViewModel workoutViewModel)
        {
            // In een echte applicatie zou je hier de workout en exercises opslaan in de database
            // Voor nu maken we een nieuw Workout object aan en retourneren we OK

            // 1. Eerste de workout opslaan en de workoutId ophalen
            var workout = new Workout
            {
                userId = 1, // In een echte app zou dit uit de gebruikersessie komen
                workoutDate = DateOnly.Parse(workoutViewModel.workoutDate),
                typeWorkout = workoutViewModel.typeWorkout,
                comments = workoutViewModel.comments
            };

            // In een echte app: workoutId = dbContext.Workouts.Add(workout).Entity.workoutId;
            int workoutId = 123; // Voorbeeld ID

            // 2. Voor elke exercise in de viewmodel:
            foreach (var exerciseVM in workoutViewModel.exercises)
            {
                // a. Zoek de exercise op basis van naam of maak een nieuwe aan
                var exercise = new Exercise
                {
                    exerciseName = exerciseVM.exerciseName,
                    muscleGroup = exerciseVM.muscleGroup
                };

                // In een echte app: int exerciseId = dbContext.Exercises.Add(exercise).Entity.exerciseId;
                int exerciseId = 456; // Voorbeeld ID

                // b. Maak TrainingData aan en koppel aan de workout en exercise
                var trainingData = new TrainingData
                {
                    workoutId = workoutId,
                    exerciseId = exerciseId,
                    amountOfSets = exerciseVM.trainingData.amountOfSets,
                    amountOfReps = exerciseVM.trainingData.amountOfReps,
                    liftedWeight = exerciseVM.trainingData.liftedWeight,
                    e1RM = CalculateE1RM(exerciseVM.trainingData.liftedWeight, exerciseVM.trainingData.amountOfReps),
                    pr = false // Dit zou je kunnen bepalen door te vergelijken met eerdere resultaten
                };

                // In een echte app: dbContext.TrainingData.Add(trainingData);
            }

            // In een echte app: dbContext.SaveChanges();

            return Ok();
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