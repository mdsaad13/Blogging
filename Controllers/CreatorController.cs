using Blogging.DAL;
using Blogging.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Blogging.Controllers
{
    [SessionAuthorize]
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
                    //TestContent = blogModel.Content.Replace(" ", "");

                    //TestContent = TestContent.Replace("<p>", "");
                    //TestContent = TestContent.Replace("</p>", "");
                    //TestContent = TestContent.Replace("<hr>", "");
                    //TestContent = TestContent.Replace("<br>", "");
                    //TestContent = TestContent.Replace("<h1>", "");
                    //TestContent = TestContent.Replace("</h1>", "");
                    //TestContent = TestContent.Replace("<span>", "");
                    //TestContent = TestContent.Replace("</span>", "");
                    //TestContent = TestContent.Replace("style", "");
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(blogModel.Content);
                    // Removing all html tags
                    TestContent = htmlDoc.DocumentNode.InnerText;
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
                        // Inserting into blog
                        CreatorUtil creatorUtil = new CreatorUtil();
                        if (creatorUtil.CreatBlog(blogModel))
                        {
                            if (blogModel.NextStep)
                            {
                                // Redirect to Add Images
                                return Json(new { status = 2, retURL = "/Creator/AddBlog/" + blogModel.BlogID + "/AddImages" });
                            }
                            else
                            {
                                // Redirect to Blogs List
                                return Json(new { status = 2, retURL = "/Creator/MyBlogs" });
                            }
                        }
                        else
                        {
                            return Json(new { status = 0 });
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
            CommonUtil commonUtil = new CommonUtil();
            if (commonUtil.Validate("blog", "blogid", Convert.ToString(blogid)))
            {
                ViewBag.BlogID = blogid;
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public JsonResult AddImagesAjax(FormCollection formCollection)
        {
            var id = Convert.ToInt32(formCollection["id"]);

            HttpFileCollectionBase file = Request.Files;
            CommonUtil commonUtil = new CommonUtil();

            if (commonUtil.CountByArgs("blogimg", "blogid = "+ id) < 6)
            {
                if (file[0] != null && id != 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file[0].FileName);
                    uniqueFileName = uniqueFileName + extension;


                    string path = Server.MapPath("~/Images/Blog/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    file[0].SaveAs(path + uniqueFileName);
                    string ImgUrl = uniqueFileName;
                    CreatorUtil creatorUtil = new CreatorUtil();

                    if (creatorUtil.AttachImgToBlog(ImgUrl, id))
                    {
                        return Json(new { status = true });
                    }
                    else
                    {
                        // Image not inserted to DB
                        return Json(new { status = false, message = "An unknown error occured. Please try again later..." });
                    }
                }
                else
                {
                    // Image file or `blogid` not found
                    return Json(new { status = false, message = "An unknown error occured. Please try again later..." });
                }
            }
            else
            {
                return Json(new { status = false, message = "You can only attach 4 images to your blog" });
            }
        }

        public JsonResult DeleteImgAjax(FormCollection formCollection)
        {
            int blogID = Convert.ToInt32(formCollection["blogid"]);
            int imgID = Convert.ToInt32(formCollection["imgid"]);

            CreatorUtil creatorUtil = new CreatorUtil();

            if (creatorUtil.DeleteBlogImg(blogID, imgID))
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }

        }

        [HttpPost]
        public JsonResult FetchBlogImages(FormCollection formCollection)
        {
            int BlogID = Convert.ToInt32(formCollection["id"]);

            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            StringBuilder Content = new StringBuilder();

            CreatorUtil creatorUtil = new CreatorUtil();

            List<BlogImages> ImgsList = creatorUtil.GetImgsByBlog(BlogID);

            if (ImgsList.Count > 0)
            {
                foreach (var item in ImgsList)
                {

                    Content.AppendFormat(@"
        <div class=""col-sm-3 text-center"">
            <a href = ""{3}/Images/Blog/{0}"" data-toggle = ""lightbox"" data-title = ""{2} - Blog Image"" data-gallery = ""gallery""  >
                   <img src = ""{3}/Images/Blog/{0}"" class=""img-fluid mb-2 img-bordered-sm"" alt =""{2} - Blog Image"" >
            </a>
            <button class=""btn btn-outline-danger btn-block deleteImg"" id = ""{1}""> Delete</button>
        </div>", item.URL, item.ID, BlogID, baseUrl);

                }
            }
            else
            {
                Content.Append(@"
        <div class=""col-sm-12 text-center"">
            <div class=""alert alert-default-info alert-dismissible fade show"" role=""alert"">
                <strong>No images found!</strong>
                <button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close"">
                    <span aria-hidden=""true"">&times;</span>
                </button>
             </div>
        </div>");

            }

            return Json(new { status = true, content = Content.ToString() });
        }
        
        public ActionResult MyBlogs()
        {
            GetUserDetails();
            CreatorUtil creatorUtil = new CreatorUtil();
            List<BlogModel> list = creatorUtil.GetAllBlogs();
            return View(list);
        }
        
        [HttpGet]
        [Route("Creator/MyBlogs/{blogid}")]
        public ActionResult ViewBlog(int blogid)
        {
            GetUserDetails();

            return View();
        }

        public ActionResult Error()
        {
            GetUserDetails();
            return View();
        }

    }
}