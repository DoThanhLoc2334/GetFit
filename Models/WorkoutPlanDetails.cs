using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFit.Models
{
	public class WorkoutPlanDetails
	{
        public int planid { get; set; }  
        public int workoutid { get; set; }  
        public int sets { get; set; }
        public int reps { get; set; }
    }
}