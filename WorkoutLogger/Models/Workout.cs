using System.ComponentModel.DataAnnotations;

namespace WorkoutLogger.Models
{
    public class Workout
    {
        [Key]
        public int workoutId { get; set; }
        public int userId { get; set; }
        public DateOnly workoutDate { get; set; }
        public string typeWorkout { get; set; }
        public string comments { get; set; }

    }
}
