// WorkoutPlanning.cs
using System.ComponentModel.DataAnnotations;

namespace UC3.Models
{
    public class WorkoutPlanning
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DayOfWeek { get; set; } // 0-6 voor maandag-zondag
        public int WeekNumber { get; set; } // Week van het jaar
    }
}