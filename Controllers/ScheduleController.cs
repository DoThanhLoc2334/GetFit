using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GetFit.Models;
using GetFit.ViewModels;

namespace GetFit.Controllers
{
    public class ScheduleController : Controller
    {
        dbGetFitDataContext db = new dbGetFitDataContext("Data Source=LAPTOP-H2JFVR4R\\MAYAO;Initial Catalog=GetFit;Integrated Security=True");
        private int GetCurrentUserId()
        {
            if (Session["User"] != null)
            {
                var user = (user)Session["User"];
                return user.user_id;
            }
            throw new Exception("User is not logged in.");
        }
        public ActionResult Index()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var plans = db.workoutplans
                .Where(p => p.userid == GetCurrentUserId())
                .Select(p => new WorkoutPlanViewModel
                {
                    PlanID = p.planid,
                    PlanName = p.planname,
                    StartDate = p.startdate,
                    Duration = p.duration,
                    Goal = p.goal,
                    Notes = p.notes
                })
                .ToList();

            return View(plans);
        }

       
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var groupedWorkouts = (from w in db.workouts
                                   group w by w.musclegroup into g
                                   select new GroupedWorkoutViewModel
                                   {
                                       MuscleGroup = g.Key,
                                       Workouts = g.Select(w => new WorkoutViewModel
                                       {
                                           WorkoutId = w.workoutid,
                                           Name = w.name
                                       }).ToList()
                                   }).ToList();

            ViewBag.Workouts = groupedWorkouts;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WorkoutPlan workoutPlan, string[] days, int[] workouts, int[] sets, int[] reps)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                var dbWorkoutPlan = new workoutplan
                {
                    userid = GetCurrentUserId(),
                    planname = workoutPlan.planname,
                    startdate = workoutPlan.startdate,
                    duration = workoutPlan.duration,
                    goal = workoutPlan.goal,
                    notes = workoutPlan.notes,
                    createdat = DateTime.Now
                };

                db.workoutplans.InsertOnSubmit(dbWorkoutPlan);
                db.SubmitChanges();

                if (days != null && workouts != null && days.Length == workouts.Length && sets != null && reps != null && sets.Length == days.Length && reps.Length == days.Length)
                {
                    for (int i = 0; i < days.Length; i++)
                    {
                        System.Diagnostics.Debug.WriteLine($"Day: {days[i]}, WorkoutID: {workouts[i]}, Sets: {sets[i]}, Reps: {reps[i]}");
                        var detail = new workoutplandetail
                        {
                            planid = dbWorkoutPlan.planid,
                            workoutid = workouts[i],
                            workoutday = days[i],
                            sets = sets[i],
                            reps = reps[i]
                        };
                        db.workoutplandetails.InsertOnSubmit(detail);
                    }
                    db.SubmitChanges();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Invalid data: days, workouts, sets, or reps is null, or lengths do not match.");
                }

                return RedirectToAction("Index");
            }

            var groupedWorkouts = (from w in db.workouts
                                   group w by w.musclegroup into g
                                   select new GroupedWorkoutViewModel
                                   {
                                       MuscleGroup = g.Key,
                                       Workouts = g.Select(w => new WorkoutViewModel
                                       {
                                           WorkoutId = w.workoutid,
                                           Name = w.name
                                       }).ToList()
                                   }).ToList();

            ViewBag.Workouts = groupedWorkouts;
            return View(workoutPlan);
        }
        public ActionResult Details(int id)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var plan = db.workoutplans.FirstOrDefault(p => p.planid == id && p.userid == GetCurrentUserId());
            if (plan == null)
            {
                return HttpNotFound("Workout plan not found.");
            }

            var details = db.workoutplandetails
                .Where(d => d.planid == id)
                .Join(db.workouts,
                    d => d.workoutid,
                    w => w.workoutid,
                    (d, w) => new {
                        d.workoutday,
                        w.name,
                        w.musclegroup,
                        d.sets,
                        d.reps,
                        w.videourl
                    })
                .AsEnumerable()
                .GroupBy(x => x.workoutday)
                .Select(g => new GroupedWorkoutDetailViewModel
                {
                    WorkoutDay = g.Key,
                    Workouts = g.Select(x => new WorkoutPlanDetailViewModel
                    {
                        WorkoutDay = x.workoutday,
                        WorkoutName = x.name,
                        MuscleGroup = x.musclegroup,
                        Sets = x.sets,
                        Reps = x.reps,
                        VideoUrl = x.videourl
                    }).OrderBy(x => x.WorkoutName).ToList()
                })
                .OrderBy(g => GetDayOrder(g.WorkoutDay))
                .ToList();

            var viewModel = new WorkoutPlanViewModel
            {
                PlanID = plan.planid,
                PlanName = plan.planname,
                StartDate = plan.startdate,
                Duration = plan.duration,
                Goal = plan.goal,
                Notes = plan.notes,
                WorkoutDetailsGrouped = details
            };

            return View(viewModel);
        }

        private int GetDayOrder(string day)
        {
            var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            return Array.IndexOf(days, day);
        }

        public ActionResult Edit(int id)
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            var plan = db.workoutplans.FirstOrDefault(p => p.planid == id && p.userid == GetCurrentUserId());
            if (plan == null)
            {
                return HttpNotFound();
            }

            var groupedWorkouts = db.workouts
                .GroupBy(w => w.musclegroup)
                .Select(g => new GroupedWorkoutViewModel
                {
                    MuscleGroup = g.Key,
                    Workouts = g.Select(w => new WorkoutViewModel
                    {
                        WorkoutId = w.workoutid,
                        Name = w.name
                    }).ToList()
                }).ToList();

            ViewBag.GroupedWorkouts = groupedWorkouts;

            var details = db.workoutplandetails
                .Where(d => d.planid == id)
                .GroupBy(d => d.workoutday)
                .Select(g => new GroupedWorkoutDetailViewModel
                {
                    WorkoutDay = g.Key,
                    Workouts = g.Select(d => new WorkoutPlanDetailViewModel
                    {
                        WorkoutName = d.workout.name,
                        MuscleGroup = d.workout.musclegroup,
                        Sets = d.sets,
                        Reps = d.reps,
                        VideoUrl = d.workout.videourl
                    }).ToList()
                }).ToList();

            var viewModel = new WorkoutPlanViewModel
            {
                PlanID = plan.planid,
                PlanName = plan.planname,
                StartDate = plan.startdate,
                Duration = plan.duration,
                Goal = plan.goal,
                Notes = plan.notes,
                WorkoutDetailsGrouped = details
            };

            return View(viewModel);
        }

        // POST: Schedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WorkoutPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Xử lý lưu dữ liệu
                // ...
                return RedirectToAction("Details", new { id = model.PlanID });
            }

            // Nếu có lỗi, load lại dropdown bài tập
            var groupedWorkouts = db.workouts
                .GroupBy(w => w.musclegroup)
                .Select(g => new GroupedWorkoutViewModel
                {
                    MuscleGroup = g.Key,
                    Workouts = g.Select(w => new WorkoutViewModel
                    {
                        WorkoutId = w.workoutid,
                        Name = w.name
                    }).ToList()
                }).ToList();

            ViewBag.GroupedWorkouts = groupedWorkouts;

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var plan = db.workoutplans.FirstOrDefault(p => p.planid == id);
            if (plan != null)
            {
                db.workoutplans.DeleteOnSubmit(plan);
                db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

        
    }
}
