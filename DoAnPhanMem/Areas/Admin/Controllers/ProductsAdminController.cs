using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DoAnPhanMem;
using DoAnPhanMem.Areas.Admin.Controllers;
using DoAnPhanMem.Common.Helpers;
using DoAnPhanMem.DTOs;
using DoAnPhanMem.Models;
using PagedList;

namespace DoAnPhanMem.Areas.Admin.Controllers
{
    public class ProductsAdminController : Controller
    {
        private WebshopEntities db = new WebshopEntities();

        // GET: Admin/ProductsAdmin
        public ActionResult Index(string search, int? size, int? page) // hiển thị tất cả sp online
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            ViewBag.search = search;
            ViewBag.countTrash = db.Products.Where(a => a.status_ == "0").Count(); //  đếm tổng sp có trong thùng rác
            var list = from a in db.Products
                       join c in db.Categories on a.cate_id equals c.cate_id
                       join d in db.Brands on a.brand_id equals d.brand_id
                       join e in db.Discounts on a.discount_id equals e.discount_id
                       where a.status_ == "1"
                       orderby a.update_at descending // giảm dần
                       select new ProductDTOs
                       {
                           discount_start = (DateTime)e.discount_start,
                           discount_end = (DateTime)e.discount_end,
                           discount_name = e.discount_name,
                           discount_price = e.discount_price,
                           product_name = a.pro_name,
                           quantity = a.quantity,
                           price = a.price,
                           Image = a.pro_img,
                           cate_name = c.cate_name,
                           brand_name = d.brand_name,
                           product_id = a.pro_id
                       };
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(s => s.product_name.Contains(search) || s.product_id.ToString().Contains(search));
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        // GET: Areas/ProductsAdmin/Details/5

        public ActionResult Details(int? id)
        {
            Product product = db.Products.FirstOrDefault(m => m.pro_id == id);
            if (product == null)
            {
                Notification.setNotification1_5s("Không tồn tại! (ID = " + id + ")", "warning");
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Areas/ProductsAdmin/Create
        public ActionResult Create() //Tạo sản phẩm
        {
            Product model = new Product();
            ViewBag.ListDiscount = new SelectList(db.Discounts.OrderBy(m => m.discount_price), "discount_id", "discount_name", 0);
            ViewBag.ListBrand = new SelectList(db.Brands, "brand_id", "brand_name", 0);
            ViewBag.ListGenre = new SelectList(db.Categories, "cate_id", "cate_name", 0);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Product product)
        {
            ViewBag.ListDiscount =
                new SelectList(db.Discounts.OrderBy(m => m.discount_price), "discount_id", "discount_name", 0);
            ViewBag.ListBrand = new SelectList(db.Brands, "brand_id", "brand_name", 0);
            ViewBag.ListGenre = new SelectList(db.Categories, "cate_id", "cate_name", 0);
            try
            {
                if (product.ImageUpload != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(product.ImageUpload.FileName);
                    var extension = Path.GetExtension(product.ImageUpload.FileName);
                    fileName = fileName + DateTime.Now.ToString("HH-mm-dd-MM-yyyy") + extension;
                    product.pro_img = "/Content/Images/" + fileName;
                    product.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileName));
                }
                else
                {
                    Notification.setNotification3s("Vui lòng thêm Ảnh Thumbnail!", "error");
                    return View(product);
                }
                product.status_ = "1";
                product.buyturn = 0;
                product.specification = product.specification;
                product.pro_description = product.pro_description;
                product.cate_id = product.cate_id;
                product.update_at = DateTime.Now;
                db.Products.Add(product);
                db.SaveChanges();
                Notification.setNotification1_5s("Thêm mới thành công!", "success");
                return RedirectToAction("Index");
            }
            catch
            {
                Notification.setNotification1_5s("Lỗi", "error");
                return View(product);
            }
        }

        // GET: Areas/ProductsAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.ListDiscount = new SelectList(db.Discounts.OrderBy(m => m.discount_price), "discount_id", "discount_name", 0);
            ViewBag.ListBrand = new SelectList(db.Brands, "brand_id", "brand_name", 0);
            ViewBag.ListGenre = new SelectList(db.Categories, "cate_id", "cate_name", 0);
            var product = db.Products.FirstOrDefault(x => x.pro_id == id);
            if (product == null || id == null)
            {
                Notification.setNotification1_5s("Không tồn tại! (ID = " + id + ")", "warning");
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Product model)
        {
            ViewBag.ListDiscount = new SelectList(db.Discounts.OrderBy(m => m.discount_price), "discount_id", "discount_name", 0);
            ViewBag.ListBrand = new SelectList(db.Brands, "brand_id", "brand_name", 0);
            ViewBag.ListGenre = new SelectList(db.Categories, "cate_id", "cate_name", 0);
            var product = db.Products.SingleOrDefault(x => x.pro_id == model.pro_id);
            try
            {
                if (model.ImageUpload != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(model.ImageUpload.FileName);
                    var extension = Path.GetExtension(model.ImageUpload.FileName);
                    fileName = fileName + extension;
                    product.pro_img = "/Content/Images/" + fileName;
                    model.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileName));
                }
                product.pro_name = model.pro_name;
                product.quantity = model.quantity;
                product.pro_description = model.pro_description;
                product.specification = model.specification;
                product.discount_id = model.discount_id;
                product.price = model.price;
                product.brand_id = model.brand_id;
                product.cate_id = model.cate_id;
                product.update_at = DateTime.Now;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                Notification.setNotification1_5s("Đã cập nhật lại thông tin!", "success");
                return RedirectToAction("Index");
            }
            catch
            {
                Notification.setNotification1_5s("Lỗi", "error");
                return View(model);
            }
        }
        public JsonResult Disable(int id)
        {
            string result = "error";
            Product product = db.Products.FirstOrDefault(m => m.pro_id == id);
            try
            {
                result = "disabled";
                product.status_ = "0";
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Undo(int id)
        {
            string result = "error";
            Product product = db.Products.FirstOrDefault(m => m.pro_id == id);
            try
            {
                result = "activate";
                product.status_ = "1";
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Delete(int id)
        {
            string result = "error";

            var checkExistOrder = db.Oder_Detail.FirstOrDefault(m => m.pro_id == id);
            if(checkExistOrder != null)
            {
                result = "exist";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            Product product = db.Products.FirstOrDefault(m => m.pro_id == id);
            try
            {
                List<ProductImg> listImage = db.ProductImgs.Where(m => m.product_id == id).ToList();
                foreach (var item in listImage)
                {
                    db.ProductImgs.Remove(item);
                    db.SaveChanges();
                }
                result = "delete";
                db.Products.Remove(product);
                db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Trash(string search, int? size, int? page) // hiển thị tất cả sp online
        {
            var pageSize = size ?? 15;
            var pageNumber = page ?? 1;
            ViewBag.search = search;
            var list = from a in db.Products
                       join c in db.Categories on a.cate_id equals c.cate_id
                       join d in db.Brands on a.brand_id equals d.brand_id
                       join e in db.Discounts on a.discount_id equals e.discount_id
                       where a.status_ == "0"
                       orderby a.update_at descending // giảm dần
                       select new ProductDTOs
                       {
                           discount_start = (DateTime)e.discount_start,
                           discount_end = (DateTime)e.discount_end,
                           discount_name = e.discount_name,
                           discount_price = e.discount_price,
                           product_name = a.pro_name,
                           quantity = a.quantity,
                           price = a.price,
                           Image = a.pro_img,
                           cate_name = c.cate_name,
                           brand_name = d.brand_name,
                           product_id = a.pro_id
                       };
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(s => s.product_name.Contains(search) || s.product_id.ToString().Contains(search));
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}