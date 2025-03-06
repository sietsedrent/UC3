using System.ComponentModel.DataAnnotations;

namespace WorkoutLogger.Models
{
    public class TrainingData
    {
        [Key]
        public int TrainingDataId { get; set; }
        public int workoutId { get; set; }
        public int exerciseId { get; set; }
        public int amountOfSets { get; set; }
        public int amountOfReps { get; set; }
        public float liftedWeight { get; set; }
        public float e1RM { get; set; }
        public bool pr { get; set; }
    }
}
