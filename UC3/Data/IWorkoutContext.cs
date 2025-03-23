using Microsoft.EntityFrameworkCore;
using UC3.Models;
using System.Threading;
using System.Threading.Tasks;

namespace UC3.Data
{
    public interface IWorkoutContext
    {
        DbSet<Exercise> ExerciseModels { get; }
        DbSet<TrainingData> TrainingDataModels { get; }
        DbSet<Workout> WorkoutModels { get; }
        DbSet<User> UserModels { get; }
        DbSet<WorkoutPlanning> WorkoutPlanningModels { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}