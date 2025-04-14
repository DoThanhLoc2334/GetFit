using GetFit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFit.Models
{
	public class WorkoutPlan
	{
        public int planid { get; set; }
        public string planname { get; set; }
        public int userid { get; set; }
        public DateTime createdat { get; set; }
        public DateTime? startdate { get; set; }
        public double price { get; set; }

        public int? duration { get; set; }        
        public string goal { get; set; }         
        public string notes { get; set; }
        public List<WorkoutPlanDetailViewModel> WorkoutDetails { get; set; } = new List<WorkoutPlanDetailViewModel>();
        public List<IGrouping<string, workout>> GroupedWorkouts { get; set; }
    }

}
