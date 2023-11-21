using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DoAnPhanMem.Models;
using Newtonsoft.Json;
using PagedList;

namespace DoAnPhanMem.Controllers
{
    public class AccountController : Controller
    {
        private WebshopEntities db = new WebshopEntities();
        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            LoginViewModels model = new LoginViewModels();
            if (String.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null && Request.UrlReferrer.ToString().Length > 0)
            {
                return RedirectToAction("Login", new { returnUrl = Request.UrlReferrer.ToString() });
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModels model, string returnUrl)
        {
            Account account = db.Accounts.FirstOrDefault(m => m.email == model.email && m.acc_password == model.acc_password && m.acc_status == "1");
            if (account != null)
            {
                Session["TaiKhoan"] = account;
                Notification.setNotification1_5s("Đăng nhập thành công", "success");
                if (!String.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");

            }
            Notification.setNotification3s("Email, mật khẩu không đúng, hoặc tài khoản bị vô hiệu hóa", "error");
            return View(model);
        }
        public ActionResult LogOut()
        {
            Notification.setNotification1_5s("Đăng xuất thành công", "success");
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            Account model = new Account();
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        //Code xử lý đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Account account)
        {
            var checkemail = db.Accounts.FirstOrDefault(m => m.email == account.email);
            if (checkemail != null)
            {
                Notification.setNotification3s("Email đã được sử dụng!!!", "error");
                return View();
            }
            int roleid = db.Roles.FirstOrDefault(role => role.role_name == "Người dùng")?.role_id ?? 0;
            account.acc_status = "1";
            account.role_id = roleid;
            account.avatar = "/Content/Images/avatar/default.jpg";
            db.Configuration.ValidateOnSaveEnabled = false;
            db.Accounts.Add(account);
            db.SaveChanges();
            Session["TaiKhoan"] = account;
            Notification.setNotification1_5s("Đăng ký thành công", "success");
            return RedirectToAction("Index", "Home", account);
        }
        public ActionResult ChangePassword()
        {
            var user = Session["TaiKhoan"] as Account;
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ChangePasswordViewModels model = new ChangePasswordViewModels();
            return View(model);
        }
        //Code xử lý Thay đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModels model)
        {
            var user = Session["TaiKhoan"] as Account;
            Account account = db.Accounts.FirstOrDefault(m => m.acc_id == user.acc_id);
            if (model.OldPassword != account.acc_password)
            {
                Notification.setNotification3s("Mật khẩu cũ không chính xác!", "error");
                return View(model);
            }

            if (model.NewPassword == account.acc_password)
            {
                Notification.setNotification3s("Mật khẩu mới và cũ không được trùng!", "error");
                return View(model);
            }
            account.acc_password = model.NewPassword;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();
            Notification.setNotification3s("Đổi mật khẩu thành công", "success");
            return RedirectToAction("ChangePassword", "Account", model);
        }
        public ActionResult Editprofile()
        {
            var userId = Session["TaiKhoan"] as Account;
            var user = db.Accounts.Where(u => u.acc_id == userId.acc_id).FirstOrDefault();
            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editprofile(Account acc)
        {
            var user = Session["TaiKhoan"] as Account;
            Account TK = db.Accounts.FirstOrDefault(m => m.acc_id == user.acc_id);
            TK.acc_name = acc.acc_name;
            TK.phone = acc.phone;
            TK.address = acc.address;
            db.Configuration.ValidateOnSaveEnabled = false;
            db.Entry(TK).State = EntityState.Modified;
            db.SaveChanges();
            Notification.setNotification3s("Cập nhật thông tin thành công", "success");
            return View(acc);
        }
        public JsonResult UpdateAvatar()
        {
            var user = Session["TaiKhoan"] as Account;
            var userId = user.acc_id;
            var account = db.Accounts.Where(m => m.acc_id == userId).FirstOrDefault();
            HttpPostedFileBase file = Request.Files[0];
            if (file != null)
            {
                string oldAvatarPath = Server.MapPath(account.avatar);
                if (account.avatar != null && account.avatar != "/Content/Images/avatar/default.jpg")
                {
                    System.IO.File.Delete(oldAvatarPath);
                }
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                fileName = fileName + extension;
                account.avatar = "/Content/Images/avatar/" + fileName;
                file.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/avatar/"), fileName));
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);

        }
        public ActionResult Address()
        {
            var user = Session["TaiKhoan"] as Account;
            if (user != null)
            {
                var address = db.AccountAddresses.Where(m => m.acc_id == user.acc_id).ToList();
                ViewBag.Check_address = db.AccountAddresses.Where(m => m.acc_id == user.acc_id).Count();
                return View(address);
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult AddressCreate(AccountAddress address)
        {
            bool result = false;
            var user = Session["TaiKhoan"] as Account;
            var userid = user.acc_id;
            var checkdefault = db.AccountAddresses.Where(m => m.acc_id == userid).ToList();
            var limit_address = db.AccountAddresses.Where(m => m.acc_id == userid).ToList();
            try
            {

                foreach (var item in checkdefault)
                {
                    if (item.isDefault == true && address.isDefault == true)
                    {
                        item.isDefault = false;
                        db.SaveChanges();
                    }
                }
                address.acc_id = userid;
                db.AccountAddresses.Add(address);
                db.SaveChanges();
                result = true;
                Notification.setNotification3s("Thêm thành công", "success");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        //Lịch sử mua hàng
        public ActionResult TrackingOrder(int? page)
        {
            var user = Session["TaiKhoan"] as Account;
            if (user != null)
            {
                return View("TrackingOrder", GetOrder(page));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //Chi tiết đơn hàng đã mua
        public ActionResult TrackingOrderDetail(int id)
        {
            var user = Session["TaiKhoan"] as Account;
            List<Oder_Detail> order = db.Oder_Detail.Where(m => m.oder_id == id).ToList();
            ViewBag.Order = db.Orders.FirstOrDefault(m => m.order_id == id);

            ViewBag.Shipfee = db.Deliveries.FirstOrDefault(m => m.delivery_id == db.Orders.FirstOrDefault(p => p.order_id == id).delivery_id).price;

            ViewBag.OrderID = id;
            if (user != null)
            {
                return View(order);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult CancelOrder(int OrderId)
        {
            bool result = false;
            try
            {
                Order order = db.Orders.FirstOrDefault(o => o.order_id == OrderId);

                if (order != null)
                {
                    order.status = "4";
                    db.SaveChanges();
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //đánh số trang
        private IPagedList GetOrder(int? page)
        {
            var user = Session["TaiKhoan"] as Account;
            int pageSize = 10;
            int pageNumber = (page ?? 1); //đánh số trang
            var list = db.Orders.Where(m => m.acc_id == user.acc_id).OrderByDescending(m => m.order_id)
                .ToPagedList(pageNumber, pageSize);
            return list;
        }


    }
}