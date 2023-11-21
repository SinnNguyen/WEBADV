using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DoAnPhanMem.Common.Helpers;
using DoAnPhanMem.Models;
using PagedList;

namespace DoAnPhanMem.Areas.Admin.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly WebshopEntities db = new WebshopEntities();

        // GET: Areas/Brands
        public ActionResult Index(string search, int? size, int? page)
        {
            var pageSize = (size ?? 15);
            var pageNumber = (page ?? 1);
            ViewBag.search = search;
            var list = from a in db.Feedbacks
                       orderby a.create_at descending
                       select a;
            if (!string.IsNullOrEmpty(search))
            {
                list = from a in db.Feedbacks
                       where a.account_id.ToString().Contains(search)
                       orderby a.create_at descending
                       select a;
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        //phản hồi bình luận
        //[HttpPost]
        //[ValidateInput(false)]
        //public JsonResult ReplyComment(int id, string reply_content, ReplyFeedback reply)
        //{
        //    var user = Session["TaiKhoan"] as Account;
        //    bool result = false;
        //    if (user != null && user.role_id == 1)
        //    {
        //        reply.feedback_id = id;
        //        reply.acc_id = user.acc_id;
        //        reply.content = reply_content;
        //        reply.status = "2";
        //        reply.create_at = DateTime.Now;
        //        db.ReplyFeedbacks.Add(reply);
        //        db.SaveChanges();
        //        result = true;
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ReplyComment(Feedback comment, int productID, string reply_content, int id)
        {
            var user = Session["TaiKhoan"] as Account;
            bool result = false;
            if (user != null && user.role_id == 1)
            {
                int userID = user.acc_id;
                comment.account_id = userID;
                comment.product_id = productID;
                comment.content = reply_content;
                comment.replyfor = id;
                comment.status = "2";
                comment.create_at = DateTime.Now;

                db.Feedbacks.Add(comment);
                db.SaveChanges();

                result = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

    }
}