using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAnPhanMem.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        public BaseController()
        {
            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/Home/Index");
            }
            else
            {
                if (System.Web.HttpContext.Current.User.IsInRole("Member"))
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/Home/Index");
                }
            }
        }
        //đăng xuất admin quay về trang chủ
        public ActionResult Logout()
        {
            Session.Clear();
            return Redirect("~/Home/Index");
        }
        //chuyển từ trang admin sang trang thông tin cá nhân
        public ActionResult ViewProfile()
        {
           
            return Redirect("~/Account/Editprofile");
        }
        //chuyển từ trang admin sang trang chủ
        public ActionResult BackToHome()
        {

            return Redirect("~/Home/Index");

        }
    }
}