using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnPhanMem.Models;

namespace DoAnPhanMem.Controllers
{
    public class HomeController : Controller
    {
        WebshopEntities db = new WebshopEntities();
        public ActionResult Index()
        {
            ViewBag.AvgFeedback = db.Feedbacks.ToList();
            ViewBag.HotProduct = db.Products.Where(item => item.status_ == "1" && item.quantity != 0).OrderByDescending(item => item.buyturn).Take(8).ToList();
            ViewBag.NewProduct = db.Products.Where(item => item.status_ == "1" && item.quantity != 0).OrderByDescending(item => item.update_at).Take(8).ToList();
            ViewBag.OrderDetail = db.Oder_Detail.ToList();
            return View();
        }
        public ActionResult PageNotFound()
        {
            return View();
        }

    }
}