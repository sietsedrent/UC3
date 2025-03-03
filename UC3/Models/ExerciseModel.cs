using System.ComponentModel.DataAnnotations;

namespace UC3.Models
{
    public class ExerciseModel
    {
        [Key]
        public int exerciseId { get; set; }
        public string exerciseName { get; set; }
        public string muscleGroup { get; set; }
    }
}
