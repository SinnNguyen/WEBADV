using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DoAnPhanMem.Common.Helpers;
using DoAnPhanMem.Models;
using PagedList;
using System.Globalization;

namespace DoAnPhanMem.Areas.Admin.Controllers
{
    public class DiscountsController : Controller
    {
        private readonly WebshopEntities _db = new WebshopEntities();

        // GET: Areas/Brands
        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in _db.Discounts
                       orderby a.discount_id ascending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in _db.Discounts
                       where a.discount_name.Contains(search) || a.discount_price.ToString().Contains(search)
                       orderby a.discount_id ascending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public JsonResult Create(DateTime discountStart, DateTime discountEnd, double discountPrice, string discountCode, Discount discount, int quantity)
        {
            string result = "false";
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            try
            {
                discount.discount_name = "Giảm " +
                        discountPrice.ToString("#,0₫", cul.NumberFormat);
                        //+ " Từ " +
                        //discountStart.ToString("dd-MM-yyyy") + " => " +
                        //discountEnd.ToString("dd-MM-yyyy");
                discount.discount_price = discountPrice;
                discount.quantity = quantity;
                discount.discount_start = discountStart;
                discount.discount_end = discountEnd;
                discount.discount_code = discountCode;
                _db.Discounts.Add(discount);
                _db.SaveChanges();
                result = "success";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Edit(int id, DateTime discountStart, DateTime discountEnd, double discountPrice, string discountCode, int quantity)
        {
            string result = "error";
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            Discount discount = _db.Discounts.FirstOrDefault(m => m.discount_id == id);
            try
            {
                discount.discount_name = "Giảm " +
                discountPrice.ToString("#,0₫", cul.NumberFormat);
                //+ " Từ " +
                //discountStart.ToString("dd-MM-yyyy") + " => " +
                //discountEnd.ToString("dd-MM-yyyy");
                discount.discount_price = discountPrice;
                discount.discount_start = discountStart;
                discount.discount_end = discountEnd;
                discount.quantity = quantity;
                discount.discount_code = discountCode;
                _db.Entry(discount).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Discounts/Delete/5
        public ActionResult Delete(int id)
        {
            string result = "error";
            Discount discount = _db.Discounts.FirstOrDefault(m => m.discount_id == id);
            try
            {
                result = "delete";
                _db.Discounts.Remove(discount);
                _db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing) _db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}