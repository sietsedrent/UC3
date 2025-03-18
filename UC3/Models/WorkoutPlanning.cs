// WorkoutPlanning.cs
using System.ComponentModel.DataAnnotations;

namespace UC3.Models
{
    public class WorkoutPlanning
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DayOfWeek { get; set; } 
        public int WeekNumber { get; set; } 
    }
}