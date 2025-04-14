using GetFit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq;
using PagedList;

namespace GetFit.Controllers
{
    public class WorkoutController : Controller
    {
        dbGetFitDataContext db = new dbGetFitDataContext("Data Source=LAPTOP-H2JFVR4R\\MAYAO;Initial Catalog=GetFit;Integrated Security=True");

        public ActionResult Index(int? page)
        {
            int pageSize = 5; 
            int pageNumber = (page ?? 1); 

            var workouts = db.workouts.OrderBy(w => w.workoutid).ToList(); 
            var pagedWorkouts = workouts.ToPagedList(pageNumber, pageSize);

            ViewBag.AllWorkouts = db.workouts.ToList();

            return View(pagedWorkouts); 
        }

        public JsonResult GetWorkoutPage(int workoutId)
        {
            int pageSize = 5; 
            var workouts = db.workouts.OrderBy(w => w.workoutid).ToList();
            int index = workouts.FindIndex(w => w.workoutid == workoutId); 
            int page = (index / pageSize) + 1; 
            return Json(page, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            var workout = db.workouts.FirstOrDefault(m => m.workoutid == id);
            if (workout == null) return HttpNotFound();
            return View(workout);
        }

        public ActionResult Create()
        {
            if(Session["Role"] == null || !Session["Role"].Equals("Admin"))
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(workout w, HttpPostedFileBase videoFile)
        {
            if (Session["Role"] == null || !Session["Role"].Equals("Admin"))
            {
                return RedirectToAction("Login", "User");
            }
            if (ModelState.IsValid)
            {
                if (videoFile != null && videoFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(videoFile.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/video/"), fileName);
                    videoFile.SaveAs(filePath);

                    w.videourl = "/Content/video/" + fileName;
                }

                db.workouts.InsertOnSubmit(w);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            return View(w);
        }

        public ActionResult Edit(int id)
        {
            var workout = db.workouts.FirstOrDefault(m => m.workoutid == id);
            if (workout == null) return HttpNotFound();
            return View(workout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, workout w, HttpPostedFileBase videoFile)
        {
            var workoutToUpdate = db.workouts.FirstOrDefault(work => work.workoutid == id);
            if (workoutToUpdate == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                workoutToUpdate.name = w.name;
                workoutToUpdate.musclegroup = w.musclegroup;
                workoutToUpdate.reps = w.reps;

                if (videoFile != null && videoFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(videoFile.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/video/"), fileName);
                    videoFile.SaveAs(filePath);

                    workoutToUpdate.videourl = "/Content/video/" + fileName;
                }

                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            return View(w);
        }

        public ActionResult Delete(int id)
        {
            var workout = db.workouts.FirstOrDefault(m => m.workoutid == id);
            if (workout == null) return HttpNotFound();
            return View(workout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var workout = db.workouts.FirstOrDefault(m => m.workoutid == id);
            if (workout != null)
            {
                db.workouts.DeleteOnSubmit(workout);
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }
    }
}