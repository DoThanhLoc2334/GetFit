using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFit.ViewModels
{
	public class GroupedWorkoutViewModel
	{
        public string MuscleGroup { get; set; }
        public List<WorkoutViewModel> Workouts { get; set; }
    }
}