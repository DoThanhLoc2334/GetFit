using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFit.ViewModels
{
	public class WorkoutPlanDetailViewModel
	{
        public string WorkoutDay { get; set; }
        public string WorkoutName { get; set; }
        public string MuscleGroup { get; set; }
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public string VideoUrl { get; set; } 
    }
}