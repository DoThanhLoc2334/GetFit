using DocumentFormat.OpenXml.Drawing.Charts;
using GetFit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GetFit.Controllers
{

    public class CartController : Controller
    {
        dbGetFitDataContext db = new dbGetFitDataContext("Data Source=LAPTOP-H2JFVR4R\\MAYAO;Initial Catalog=GetFit;Integrated Security=True");

        // GET: Cart
        public List<Cart> getCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }

        public ActionResult AddCart(int id, string strURL)
        {
            List<Cart> lstCart = getCart();
            Cart workoutPlan = lstCart.Find(n => n.plan_id == id);

            if (string.IsNullOrEmpty(strURL))
            {
                strURL = Url.Action("Index", "BMI"); // Gán giá trị mặc định
            }

            if (workoutPlan == null)
            {
                workoutPlan = new Cart(id);
                lstCart.Add(workoutPlan);
                return Redirect(strURL);
            }
            else
            {
                workoutPlan.quantity++;
                return Redirect(strURL);
            }

        }

        private int sumQuantity()
        {
            int tsl = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;

            if (lstCart != null)
            {
                tsl = lstCart.Sum(n => n.quantity);
            }
            return tsl;
        }

        private int sumProductQuantity()
        {
            int tsl = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;

            if (lstCart != null)
            {
                tsl = lstCart.Count;
            }
            return tsl;
        }

        private double Total()
        {
            double tt = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                tt = lstCart.Sum(n => n.Total);
            }
            return tt;
        }

        public ActionResult Cart()
        {
            List<Cart> lstCart = getCart();
            ViewBag.sumQuantity = sumQuantity();
            ViewBag.Total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();
            return View(lstCart);
        }

        public ActionResult CartPartial()
        {
            ViewBag.sumQuantity = sumQuantity();
            ViewBag.Total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();
            return PartialView();
        }

        public ActionResult CartDelete(int id)
        {
            List<Cart> lstCart = getCart();
            Cart workoutPlan = lstCart.SingleOrDefault(n => n.plan_id == id);
            if (workoutPlan != null)
            {
                lstCart.RemoveAll(n => n.plan_id == id);
                return RedirectToAction("Cart");

            }
            return RedirectToAction("Cart");
        }

        public ActionResult CartUpdate(int id, FormCollection collection)
        {
            List<Cart> lstCart = getCart();
            Cart workoutPlan = lstCart.SingleOrDefault(n => n.plan_id == id);
            if (workoutPlan != null)
            {
                workoutPlan.quantity = int.Parse(collection["txtQuantity"].ToString());
            }
            return RedirectToAction("Cart");
        }

        public ActionResult AllCartDelete()
        {
            List<Cart> lstCart = getCart();
            lstCart.Clear();
            return RedirectToAction("Cart");
        }


        [HttpGet]
        public ActionResult PlaceOrder()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "BMI");
            }
            List<Cart> lstCart = getCart();
            ViewBag.sumQuantity = sumQuantity();
            ViewBag.Total = Total();
            ViewBag.sumProductQuantity = sumProductQuantity();
            return View(lstCart);
        }

        [HttpPost]
        public ActionResult PlaceOrder(FormCollection collection, IEnumerable<Cart> carts)
        {
            order dh = new order();
            user kh = (user)Session["User"];
            workoutplan plan = new workoutplan();

            List<Cart> gh = getCart();
            var delivery_date = String.Format("{0:MM/dd/yyyy}", collection["delivery_date"]);

            dh.user_id = kh.user_id;
            dh.orderdate = DateTime.Now;
            dh.delivery_date = DateTime.Parse(delivery_date);
            dh.isship = false;
            dh.ispayment = false;

            db.orders.InsertOnSubmit(dh);
            db.SubmitChanges();

            var strSanPham = "";
            var thanhTien = decimal.Zero;
            var tongTien = decimal.Zero;

            //Send mail  
            tongTien = thanhTien;
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
            contentCustomer = contentCustomer.Replace("{{MaDon}}", dh.orderid.ToString());
            contentCustomer = contentCustomer.Replace("{{NgapDathang}}", dh.delivery_date.ToString());
            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", kh.user_fullname);
            contentCustomer = contentCustomer.Replace("{{Phone}}", kh.phonenumber);
            contentCustomer = contentCustomer.Replace("{{Email}}", kh.email);
            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", thanhTien.ToString());
            contentCustomer = contentCustomer.Replace("{{TongTien}}", tongTien.ToString());
            Testing.Common.Common.SendMail("ShopOnLine", "Đơn hàng #" + dh.orderid.ToString(), contentCustomer, kh.email);
            if (carts != null)
                foreach (Cart item in carts)
                {
                    strSanPham += "<tr>";
                    strSanPham += "<td>" + item.workout_name + "</td>";
                    strSanPham += "<td>" + item.quantity + "</td>";
                    strSanPham += "<td>" + item.price + "</td>";
                    strSanPham += "</tr>";
                    thanhTien += (decimal)item.price * item.quantity;
                }

            foreach (var item in gh)
            {
                orderdetail ctdh = new orderdetail();

                ctdh.orderid = dh.orderid;
                ctdh.planid = item.plan_id;
                ctdh.quantity = item.quantity;
                ctdh.price = (decimal)item.price;

                plan = db.workoutplans.Single(p => p.planid == item.plan_id);

                db.SubmitChanges();
                db.orderdetails.InsertOnSubmit(ctdh);
            }

            db.SubmitChanges();

            Session["Cart"] = null;
            return RedirectToAction("ConfirmOrder", "Cart");
        }
        public ActionResult ConfirmOrder()
        {
            return View();
        }
    }
}