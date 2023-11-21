﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DoAnPhanMem.Areas.Admin.Controllers;
using DoAnPhanMem.Common.Helpers;
using DoAnPhanMem.Models;
using PagedList;

namespace DoAnPhanMem.Areas.Admin.Controllers
{
    public class BrandsController : Controller
    {
        private readonly WebshopEntities _db = new WebshopEntities();

        // GET: Areas/Brands
        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in _db.Brands
                       orderby a.brand_id ascending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in _db.Brands
                       where a.brand_name.Contains(search)
                       orderby a.brand_id ascending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }



        [HttpPost]
        public JsonResult Create(string brandName, Brand brand)
        {
            string result = "false";
            try
            {
                Brand checkExist = _db.Brands.SingleOrDefault(m => m.brand_name == brandName);
                if (checkExist != null)
                {
                    result = "exist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                brand.brand_name = brandName;
                _db.Brands.Add(brand);
                _db.SaveChanges();
                result = "success";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Edit(int id, string brandName)
        {
            string result = "error";
            Brand brand = _db.Brands.FirstOrDefault(m => m.brand_id == id);
            var checkExist = _db.Brands.SingleOrDefault(m => m.brand_name == brandName);
            try
            {
                if (checkExist != null)
                {
                    result = "exist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result = "success";
                brand.brand_name = brandName;
                _db.Entry(brand).State = EntityState.Modified;
                _db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(int id)
        {
            string result = "error";

            bool check = _db.Products.Any(m => m.brand_id == id);
            if(check)
            {
                result = "exist";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            Brand brand = _db.Brands.FirstOrDefault(m => m.brand_id == id);
            try
            {
                result = "delete";
                _db.Brands.Remove(brand);
                _db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}