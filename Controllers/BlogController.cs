using Blogging.DAL;
using Blogging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Blogging.Controllers
{
    [RoutePrefix("blog")]
    public class BlogController : Controller
    {
        public BlogController()
        {
            // Constructor
            ViewBag.SoftwareName = "Blogger";
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

        [Route("{Url}")]
        public ActionResult Index(string Url)
        {
            GetUserDetails();
            return View();
        }

        [Route("AjaxLoadProfile")]
        public JsonResult AjaxLoadProfile(FormCollection form)
        {
            bool status = false;

            double CurrentUserID = Convert.ToDouble(form["CurrentUserID"]);
            int Type = Convert.ToInt32(form["Sort"]);

            StringBuilder Content = new StringBuilder();

            IndexUtil indexUtil = new IndexUtil();

            // Validating input
            if (CurrentUserID != 0)
            {
                switch (Type)
                {
                    case 1:
                        status = true;
                        break;
                    case 2:
                        status = true;
                        break;
                    case 3:
                        status = true;
                        break;
                    case 4:
                        status = true;
                        break;
                    default:
                        status = false;
                        break;
                }
            }

            if (status)
            {
                List<AllBlogsModel> BlogsList = indexUtil.AllBlogs(CurrentUserID, Type);

                if (BlogsList.Count > 0)
                {
                    foreach (var item in BlogsList)
                    {
                        if (!item.HasImages)
                        {
                            item.BlogImg = "cyan-plain-bg.png";
                        }
                        Content.AppendFormat(@"
                        <div class=""row no-gutters"">
                                <div class=""col-md-4"">

                                    <a href=""/blog/{4}"">
                                        <img class=""card-img img-fluid lazy"" src =""/Images/img-spinner.svg"" data-src=""/Images/Blog/{0}"" alt =""{1}"" >
                                    </a>
                                </div>
                                <div class=""col-md-8"">
                                    <div class=""card-body"" style=""padding-top:unset"">
                                        <h4 class=""card-title font-weight-bold"">{1}</h4>
                                        <br />
                                        <p class=""card-text"">
                                            {2}
                                            <a href=""/blog/{4}"">Read More</a>
                                        </p>
                                        <p class=""card-text"">
                                            <small class=""text-muted"">
                                                Published on: {3}
                                            </small>
                                            <span class=""badge text-dark float-right"">
                                                <i class=""fas fa-tags""></i>&nbsp;
                                                {8}
                                            </span>
                                        </p>

                                        <div class=""row text-center text-sm"">
                                            <div class=""col"">
                                                <i class=""fas fa-thumbs-up""></i> {5}
                                            </div>
                                            <div class=""col"">
                                                <i class=""fas fa-eye""></i> {6}
                                            </div>
                                            <div class=""col"">
                                                <i class=""fas fa-comments""></i> {7}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>", item.BlogImg, item.Title, item.Content, item.PublistDate, item.URL, item.Likes, item.Views, item.Comments, item.CatName);
                        //0 img
                        //1 title
                        //2 content
                        //3 date
                        //4 url
                        //5 likes
                        //6 views
                        //7 comments
                        //8 category name
                    }
                }
                else
                {
                    status = false;
                }
            }
            return Json(new { status, content = Content.ToString() });
        }

    }
}