using Blogging.DAL;
using Blogging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogging.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            ViewBag.SoftwareName = "Blogger";
        }

        public ActionResult Index()
        {
            GetUserDetails();
            return View();
        }

        internal void GetUserDetails()
        {
            AccountUtil accountUtil = new AccountUtil();
            
            long userID = Convert.ToInt64(Session["userID"]);
            if(userID != 0)
            {
                AccountModel accountModel = accountUtil.GetUserById(userID);
                ViewBag.Name = accountModel.Name;
                ViewBag.UserName = accountModel.UserName;
                ViewBag.ImgUrl = accountModel.ImgUrl;
                ViewBag.Followers = 0;
                ViewBag.Blogs = 0;
            }
        }
    }
}