using DoAnPhanMem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnPhanMem.Areas.Admin.Controllers
{
    public class DashBoardsController : Controller
    {
        private WebshopEntities db = new WebshopEntities();
        // GET: Admin/DashBoards
        public ActionResult Index()
        {
            ViewBag.Order = db.Orders.ToList();
            ViewBag.OrderDetail = db.Oder_Detail.ToList();
            ViewBag.ListOrderDetail = db.Oder_Detail.Take(3).ToList();
            ViewBag.ListOrder = db.Orders.Take(7).ToList();
            return View();
        }
    }
}