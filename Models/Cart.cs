using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetFit.Models
{
	public class Cart
	{
        dbGetFitDataContext db = new dbGetFitDataContext("Data Source=LAPTOP-H2JFVR4R\\MAYAO;Initial Catalog=GetFit;Integrated Security=True");
        public int plan_id { get; set; }

        [Display(Name = "Workout Name")]
        public string workout_name { get; set; }

        [Display(Name = "Duration")]
        public int duration { get; set; }

        [Display(Name = "Price")]
        public double price { get; set; }

        [Display(Name = "Quantity")]
        public int quantity { get; set; }

        [Display(Name = "Total")]
        public double Total => quantity * price;

        public Cart(int id)
        {
            plan_id = id;
            var plan = db.workoutplans.Single(p => p.planid == plan_id);
            if (plan != null)
            {
                workout_name = plan.planname;
                duration = plan.duration.HasValue ? plan.duration.Value : 0; // Fixed line
                price = double.Parse(plan.price.ToString());
                quantity = 1;
            }
        }
    }
}