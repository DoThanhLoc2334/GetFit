using GetFit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFit.ViewModels
{
	public class WorkoutPlanViewModel
	{
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public DateTime? StartDate { get; set; }
        public int? Duration { get; set; }
        public string Goal { get; set; }
        public string Notes { get; set; }
        public List<WorkoutPlanDetailViewModel> WorkoutDetails
        {
            get; set;
        }
        public List<GroupedWorkoutDetailViewModel> WorkoutDetailsGrouped { get; set; }
    }
}