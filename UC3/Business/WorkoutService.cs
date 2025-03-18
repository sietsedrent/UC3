using Microsoft.EntityFrameworkCore;
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
            var workout = await _context.WorkoutModels
                .Where(w => w.workoutId == workoutId && w.userId == userId)
                .FirstOrDefaultAsync();

            if (workout == null)
            {
                return null;
            }

            var trainingDataList = await _context.TrainingDataModels
                .Where(td => td.workoutId == workoutId)
                .ToListAsync();

            var exercisesList = new List<object>();

            foreach (var trainingData in trainingDataList)
            {
                var exercise = await _context.ExerciseModels
                    .FirstOrDefaultAsync(e => e.exerciseId == trainingData.exerciseId);

                if (exercise != null)
                {
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
            var workout = new Workout
            {
                userId = userId,
                workoutDate = DateOnly.Parse(workoutDTO.workoutDate),
                typeWorkout = workoutDTO.typeWorkout,
                comments = workoutDTO.comments
            };

            _context.WorkoutModels.Add(workout);
            await _context.SaveChangesAsync();

            int workoutId = workout.workoutId;

            foreach (var exerciseDTO in workoutDTO.exercises)
            {
                var exercise = await _context.ExerciseModels
                    .FirstOrDefaultAsync(e => e.exerciseName == exerciseDTO.exerciseName);

                if (exercise == null)
                {
                    exercise = new Exercise
                    {
                        exerciseName = exerciseDTO.exerciseName,
                        muscleGroup = exerciseDTO.muscleGroup
                    };

                    _context.ExerciseModels.Add(exercise);
                    await _context.SaveChangesAsync();
                }

                bool isPR = false;
                var calculatedE1RM = CalculateE1RM(exerciseDTO.trainingData.liftedWeight, exerciseDTO.trainingData.amountOfReps);

                var previousTrainingData = await _context.TrainingDataModels
                    .Where(td => td.exerciseId == exercise.exerciseId)
                    .ToListAsync();

                if (!previousTrainingData.Any() || previousTrainingData.All(td => td.e1RM < calculatedE1RM))
                {
                    isPR = true;
                }

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