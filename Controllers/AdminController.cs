using GetFit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GetFit.Controllers
{
    public class AdminController : Controller
    {
        dbGetFitDataContext db = new dbGetFitDataContext("Data Source=LAPTOP-H2JFVR4R\\MAYAO;Initial Catalog=GetFit;Integrated Security=True");
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection, user u)
        {
            var fullName = collection["user_fullname"];
            var username = collection["username"];
            var password = collection["password"];
            var confirmPassword = collection["confirmPassword"];
            var email = collection["email"];
            var phoneNumber = collection["phonenumber"];
            var dob = collection["user_dob"];
            if (String.IsNullOrEmpty(confirmPassword))
            {
                ViewData["enterpassword"] = "Must enter password to confirm";
            }
            else
            {
                if (!password.Equals(confirmPassword))
                {
                    ViewData["samepassword"] = "Password and confirmation password must be the same";
                }
                else
                {
                    u.user_fullname = fullName;
                    u.username = username;
                    u.password = password;
                    u.email = email;
                    u.phonenumber = phoneNumber;
                    u.user_dob = DateTime.Parse(dob);
                    db.users.InsertOnSubmit(u);
                    db.SubmitChanges();
                    return RedirectToAction("Register");
                }
            }
            return RedirectToAction("Login", "Admin");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var username = collection["username"];
            var password = collection["password"];
            user u = db.users.SingleOrDefault(n => n.username == username && n.password == password);
            if (u != null)
            {
                ViewBag.ThongBao = "Congratulations on successful login";
                Session["User"] = u;
            }
            else
            {
                ViewBag.ThongBao = "Username or password is incorrect";
            }
            return RedirectToAction("Index", "Home");
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
    }
}