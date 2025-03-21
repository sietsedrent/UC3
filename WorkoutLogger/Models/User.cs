﻿using System.ComponentModel.DataAnnotations;

namespace WorkoutLogger.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string bio { get; set; }
        public string profilepicture { get; set; }
        public DateOnly RegisteryDate { get; set; }
        
    }
}
