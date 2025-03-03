using Microsoft.EntityFrameworkCore;
using UC3.Models;

namespace UC3.Data
{
    public class WorkoutContext : DbContext
    {
        public WorkoutContext(DbContextOptions<WorkoutContext> options) : base(options) { }

        public DbSet<ExerciseModel> ExerciseModels { get; set; }
        public DbSet<TrainingDataModel> TrainingDataModels { get; set; }
        public DbSet<WorkoutModel> WorkoutModels { get; set; }
        public DbSet<UserModel> UserModels { get; set; }
    }
}
