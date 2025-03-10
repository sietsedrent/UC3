using Microsoft.EntityFrameworkCore;
using UC3.Models;

namespace UC3.Data
{
    public class WorkoutContext : DbContext
    {
        public WorkoutContext(DbContextOptions<WorkoutContext> options) : base(options) { }
        

        public DbSet<Exercise> ExerciseModels { get; set; }
        public DbSet<TrainingData> TrainingDataModels { get; set; }
        public DbSet<Workout> WorkoutModels { get; set; }
        public DbSet<User> UserModels { get; set; }


    }
}
