using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFit.Models
{
	public class Workout
	{
        public int workoutid { get; set; } 
        public string name { get; set; }
        public string musclegroup { get; set; }
        public string videourl { get; set; }
        public int reps { get; set; }

    }
}