using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DoAnPhanMem;
//using DoAnPhanMem.Common.Helpers;
using DoAnPhanMem.Models;
using PagedList;

namespace DoAnPhanMem.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private WebshopEntities db = new WebshopEntities();

        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            ViewBag.countTrash = db.Accounts.Where(a => a.acc_status == "0").Count();
            var list = from a in db.Accounts
                       where a.acc_status != "0"
                       orderby a.acc_id ascending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in db.Accounts
                       where a.email.Contains(search) || a.acc_id.ToString().Contains(search) || a.acc_name.Contains(search)
                       orderby a.acc_id ascending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Trash(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in db.Accounts
                       where a.acc_status == "0"
                       orderby a.acc_id ascending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in db.Accounts
                       where a.email.Contains(search) || a.acc_id.ToString().Contains(search) || a.acc_name.Contains(search)
                       orderby a.acc_id ascending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        public JsonResult ChangeRoles(int accountID, int roleID)
        {
            var user = Session["TaiKhoan"] as Account;
            var account = db.Accounts.FirstOrDefault(m => m.acc_id == accountID);
            int role = user.role_id;
            bool result = false;
            try
            {
                if (account != null && role == 1)
                {
                    account.role_id = roleID;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Entry(account).State = EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Disable(int id)
        {
            var user = Session["TaiKhoan"] as Account;
            string result = "error";
            Account account = db.Accounts.FirstOrDefault(m => m.acc_id == id);

            if (user.acc_id != id)
            {
                result = "success";
                account.acc_status = "0";
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult IsActive(int id)
        {
            string result = "error";
            Account account = db.Accounts.FirstOrDefault(m => m.acc_id == id);
            try
            {
                result = "success";
                account.acc_status = "1";
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}