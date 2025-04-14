using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GetFit.Controllers
{
    public class BMIController : Controller
    {
        // GET: BMI
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult calculateBMI(float height, float weight)
        {
            if(height <= 0 || weight <= 0)
            {
                ViewBag.Result = "Please enter a valid number";
                return View("Index");
            }

            float heightInMeter = height / 100;
            float bmi = weight / (heightInMeter * heightInMeter);
            string category = GetBMICategory(bmi);

            ViewBag.BMI = bmi.ToString("0.00");
            ViewBag.Category = category;

            return View("Index");
        }
        public string GetBMICategory(float bmi)
        {
            if (bmi < 18.5) return "Thin";
            if (bmi >= 18.5 && bmi <= 24.9) return "Normal";
            if (bmi >= 25 && bmi <= 29.9) return "obesity";
            if (bmi >= 30 && bmi <= 34.9) return "obesity level 1";
            if (bmi >= 35 && bmi <= 39.9) return "obesity level 2";
            return "obesity level 3";

        }
    }
}