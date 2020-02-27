using Blogging.DAL;
using Blogging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogging.Controllers
{
    public class CreatorController : Controller
    {
        public CreatorController()
        {
            //Constructor
            ViewBag.SoftwareName = "Creator - Blogger";
        }

        /// <summary>
        /// <b>Gets user details from session that's displayed on nav-bar</b>
        /// </summary>
        internal void GetUserDetails()
        {
            AccountUtil accountUtil = new AccountUtil();

            long userID = Convert.ToInt64(Session["userID"]);
            if (userID != 0)
            {
                AccountModel accountModel = accountUtil.GetUserById(userID);
                ViewBag.Name = accountModel.Name;
                ViewBag.UserName = accountModel.UserName;
                ViewBag.ImgUrl = accountModel.ImgUrl;
                ViewBag.Followers = 0;
                ViewBag.Blogs = 0;
            }
        }

        // GET: Creator
        public ActionResult Index()
        {
            GetUserDetails();
            return View();
        }

        public ActionResult AddBlog()
        {
            GetUserDetails();
            CommonUtil commonUtil = new CommonUtil();
            ViewBag.AllCat = new SelectList(commonUtil.GetAllCat(), "catid", "name");
            //ViewData["AllCat"] = new SelectList(commonUtil.GetAllCat(), "catid", "name");
            return View();
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult AddBlogAjax(BlogModel blogModel)
        {
            try
            {
                string TestContent = " ";
                if (blogModel.Content != null)
                {
                    TestContent = blogModel.Content.Replace(" ", "");

                    TestContent = TestContent.Replace("<p>", "");
                    TestContent = TestContent.Replace("</p>", "");
                    TestContent = TestContent.Replace("<hr>", "");
                    TestContent = TestContent.Replace("<br>", "");
                    TestContent = TestContent.Replace("<h1>", "");
                    TestContent = TestContent.Replace("</h1>", "");
                    TestContent = TestContent.Replace("<span>", "");
                    TestContent = TestContent.Replace("</span>", "");
                    TestContent = TestContent.Replace("style", "");
                }

                if (TestContent.Length < 100)
                {
                    // Content is less than 100 characters after trimming form `trimChars`
                    return Json(new { status = 3 });
                }
                else
                {
                    CommonUtil commonUtil = new CommonUtil();
                    if (!commonUtil.Validate("blogs", "url", blogModel.URL))
                    {
                        blogModel.BlogID = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        blogModel.UserID = Convert.ToInt64(Session["userID"]);
                        blogModel.DateTime = DateTime.Now;
                        blogModel.Status = 0;
                        blogModel.ViewCount = 0;
                        blogModel.ViewTime = 0;

                        if(blogModel.NextStep)
                        {
                            // Redirect to Add Images
                            return Json(new { status = 2, retURL = "/Creator/AddBlog/"+ blogModel.BlogID + "/AddImages" });
                        }
                        else
                        {
                            // Redirect to Blogs List
                            return Json(new { status = 2, retURL = "/Creator/MyBlogs" });
                        }
                    }
                    else
                    {
                        // `url` already exists!
                        return Json(new { status = 1 });
                    }
                }
            }
            catch
            {
                return Json(new { status = 0 });
            }
        }

        [Route("Creator/AddBlog/{blogid}/AddImages")]
        public ActionResult AddImages(int blogid)
        {
            GetUserDetails();
            ViewBag.Title = "Add Images to blog";
            return View();
        }

        [HttpPost]
        public JsonResult TestSubmit(FormCollection Fc)
        {
            return Json(new { status = 3 });
        }
        
        public ActionResult MyBlogs()
        {
            GetUserDetails();

            return View();
        }
    }
}