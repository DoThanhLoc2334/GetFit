using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFit.ViewModels
{
    public class GroupedWorkoutDetailViewModel
    {
        public string WorkoutDay { get; set; }
        public List<WorkoutPlanDetailViewModel> Workouts
        {
            get; set;
        }
    }
}