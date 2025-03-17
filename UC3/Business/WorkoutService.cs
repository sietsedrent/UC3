using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UC3.Data;
using UC3.Models;

namespace UC3.Business
{
    public class WorkoutService
    {
        private readonly WorkoutContext _context;

        public WorkoutService(WorkoutContext context)
        {
            _context = context;
        }

        public async Task<List<Workout>> GetWorkoutsForUser(int userId)
        {
            return await _context.WorkoutModels
                .Where(w => w.userId == userId)
                .ToListAsync();
        }

        public async Task<object> GetWorkoutDetails(int workoutId, int userId)
        {
            // Haal workout op met alle gerelateerde gegevens
            var workout = await _context.WorkoutModels
                .Where(w => w.workoutId == workoutId && w.userId == userId)
                .FirstOrDefaultAsync();

            if (workout == null)
            {
                return null;
            }

            // Haal alle trainingsgegevens op voor deze workout
            var trainingDataList = await _context.TrainingDataModels
                .Where(td => td.workoutId == workoutId)
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

            return result;
        }

        public async Task<int> SaveWorkout(WorkoutDTO workoutDTO, int userId)
        {
            // 1. Eerste de workout opslaan en de workoutId ophalen
            var workout = new Workout
            {
                userId = userId,
                workoutDate = DateOnly.Parse(workoutDTO.workoutDate),
                typeWorkout = workoutDTO.typeWorkout,
                comments = workoutDTO.comments
            };

            _context.WorkoutModels.Add(workout);
            await _context.SaveChangesAsync();

            // Nu hebben we een workoutId
            int workoutId = workout.workoutId;

            // 2. Voor elke exercise in de DTO:
            foreach (var exerciseDTO in workoutDTO.exercises)
            {
                // a. Zoek de exercise op basis van naam of maak een nieuwe aan
                var exercise = await _context.ExerciseModels
                    .FirstOrDefaultAsync(e => e.exerciseName == exerciseDTO.exerciseName);

                if (exercise == null)
                {
                    // Maak een nieuwe exercise aan als deze nog niet bestaat
                    exercise = new Exercise
                    {
                        exerciseName = exerciseDTO.exerciseName,
                        muscleGroup = exerciseDTO.muscleGroup
                    };

                    _context.ExerciseModels.Add(exercise);
                    await _context.SaveChangesAsync();
                }

                // b. Controleer of dit een PR is door te vergelijken met eerdere resultaten
                bool isPR = false;
                var calculatedE1RM = CalculateE1RM(exerciseDTO.trainingData.liftedWeight, exerciseDTO.trainingData.amountOfReps);

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
                    amountOfSets = exerciseDTO.trainingData.amountOfSets,
                    amountOfReps = exerciseDTO.trainingData.amountOfReps,
                    liftedWeight = exerciseDTO.trainingData.liftedWeight,
                    e1RM = calculatedE1RM,
                    pr = isPR
                };

                _context.TrainingDataModels.Add(trainingData);
            }

            await _context.SaveChangesAsync();
            return workoutId;
        }

        // Helper methode om geschatte 1 rep max te berekenen (Brzycki formule)
        private float CalculateE1RM(float weight, int reps)
        {
            if (reps == 1) return weight;
            return weight * (36 / (37 - reps));
        }
    }

    // Data Transfer Objects
    public class WorkoutDTO
    {
        public string typeWorkout { get; set; }
        public string workoutDate { get; set; }
        public string comments { get; set; }
        public List<ExerciseDTO> exercises { get; set; }
    }

    public class ExerciseDTO
    {
        public string exerciseName { get; set; }
        public string muscleGroup { get; set; }
        public TrainingDataDTO trainingData { get; set; }
    }

    public class TrainingDataDTO
    {
        public int amountOfSets { get; set; }
        public int amountOfReps { get; set; }
        public float liftedWeight { get; set; }
    }
}