using System.ComponentModel.DataAnnotations;

namespace WorkoutLogger.Models
{
    public class Exercise
    {
        [Key]
        public int exerciseId { get; set; }
        public string exerciseName { get; set; }
        public string muscleGroup { get; set; }
    }
}
