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
            ViewBag.SoftwareName = SoftwareConfig.AppName;
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
                CommonUtil commonUtil = new CommonUtil();
                ViewBag.Followers = commonUtil.CountByArgs("Followers", $"Follow_userID = {userID}");
                ViewBag.Blogs = commonUtil.CountByArgs("blog", $"userID = {userID}");
                ViewBag.Sidenav_SubsList = accountUtil.GetAllFollowing(userID, false, true);
            }

            IndexUtil indexUtil = new IndexUtil();
            ViewBag.Sidenav_CatList = indexUtil.PersonalizedCats();
        }

        [Route("{Url}")]
        public ActionResult Index(string Url)
        {
            GetUserDetails();
            CommonUtil commonUtil = new CommonUtil();
            if (commonUtil.Validate("blog", "url", Url))
            {
                IndexUtil indexUtil = new IndexUtil();
                SingleBlog Content = indexUtil.FetchSingleBlog(Url);
                if (Session["userID"] == null)
                {
                    ViewData["Bookmark"] = "DisabledBookmark";
                    ViewData["BookmarkIcon"] = "far";

                    ViewData["Like"] = "DisabledLikeBlog";
                }
                else
                {
                    long userID = Convert.ToInt64(Session["userID"]);

                    string query1 = $"userID = {userID} AND Follow_userID = {Content.Profile.UserID}";
                    ViewData["Follow"] = commonUtil.RawValidate("Followers", query1) ? "Unfollow" : "Follow";

                    string query = $"userID = {userID} AND blogid = {Content.Blog.BlogID}";

                    ViewData["Like"] = commonUtil.RawValidate("likes", query) ? "LikedBlog" : "LikeBlog";
                    ViewData["Bookmark"] = commonUtil.RawValidate("bookmarks", query) ? "Bookmarked" : "Bookmark";

                    if (Convert.ToString(ViewData["Bookmark"]) == "Bookmarked")
                    {
                        ViewData["BookmarkIcon"] = "fas";
                    }
                    else
                    {
                        ViewData["BookmarkIcon"] = "far";
                    }
                    
                }
                
                return View(Content);
            }
            else
            {
                string Info = "Blog not found :(";
                ViewBag.Info = Info;
                return View("Error");
            }            
        }

        [Route("AjaxLoadProfile")]
        public JsonResult AjaxLoadProfile(FormCollection form)
        {
            bool status = false;
            StringBuilder Content = new StringBuilder();

            if (form.Count != 0)
            {
                double CurrentUserID = Convert.ToDouble(form["CurrentUserID"]);
                int Type = Convert.ToInt32(form["Sort"]);               

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
                    List<AllBlogsModel> BlogsList = indexUtil.UsersBlogs(CurrentUserID, Type);

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
                            </div>", item.BlogImg, item.Title, item.Content, item.PublishDate, item.URL, item.Likes, item.Views, item.Comments, item.CatName);
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
            }

            return Json(new { status, content = Content.ToString() });
        }

        [HttpPost]
        [Route("AjaxLoadComments")]
        public JsonResult AjaxLoadComments(FormCollection formCollection)
        {
            CommonUtil commonUtil = new CommonUtil();
            IndexUtil indexUtil = new IndexUtil();
            StringBuilder Content = new StringBuilder();

            bool MoreData = true;
            long CurrentDataCount = Convert.ToInt64(formCollection["CurrentDataCount"]);
            long BlogID = Convert.ToInt64(formCollection["BlogID"]);
            long CommentsCount = commonUtil.CountByArgs("comments", "blogid = "+BlogID);

            List<CommentsModel> CommentsList = indexUtil.BlogComments(CurrentDataCount, BlogID);

            CurrentDataCount += CommentsList.Count;
            if (CurrentDataCount >= CommentsCount)
            {
                MoreData = false;
            }

            bool status = CommentsList.Count > 0 ? true : false;
            string Image;
            if (status)
            {
                foreach (var Commentitem in CommentsList)
                {
                    if (Commentitem.ImgURL == null)
                    {
                        Image = "/Images/default_user.png";
                    }
                    else
                    {
                        Image = "/Images/UserProfiles/"+Commentitem.ImgURL;
                    }
                    
                    Content.AppendFormat(@"
                        <div class=""direct-chat-msg"" id=""{0}"">
                            <div class=""direct-chat-infos clearfix"">
                                <span class=""direct-chat-name float-left"">
                                     <a href=""/user/{3}"" class=""text-dark"">
                                         {4}
                                      </a>
                                 </span>
                                <span class=""direct-chat-timestamp float-right"">{2}</span>
                             </div>
                             <a href=""/user/{3}"" class=""text-dark"">
                                 <img class=""direct-chat-img lazy"" src=""/Images/img-spinner.svg"" data-src=""{5}"" alt=""{3}"">
                             </a>
                             <div class=""direct-chat-text"">
                                {1}
                             </div>
                        </div>
                    ", Commentitem.CommentID, Commentitem.Text, Commentitem.FormatDateTime, Commentitem.UserName, Commentitem.Name, Image);
                }
            }

            return Json(new { status, content = Content.ToString(), MoreData, CurrentDataCount, CommentsCount });
        }

        [SessionAuthorize]
        [ValidateAntiForgeryToken]
        [Route("AjaxAddComment")]
        public JsonResult AjaxAddComment(CommentsModel commentsModel)
        {
            bool status = false;
            IndexUtil indexUtil = new IndexUtil();
            commentsModel.DateTime = DateTime.Now;

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Random random = new Random();
            int randomNo = random.Next(1000);

            commentsModel.CommentID = Convert.ToInt64(string.Format("{1}{0}", unixTimestamp, randomNo));

            status = indexUtil.InsertComment(commentsModel);

            return Json(new { status , commentsModel.CommentID });
        }

        [SessionAuthorize]
        [Route("AjaxBookmark")]
        public JsonResult AjaxBookmark(FormCollection formCollection)
        {
            bool status = false;
            IndexUtil indexUtil = new IndexUtil();

            long userID = Convert.ToInt64(Session["userID"]);

            int State = Convert.ToInt32(formCollection["State"]);
            int BlogID = Convert.ToInt32(formCollection["BlogID"]);

            if (State == 1) // Bookmark Blog
            {
                status = indexUtil.InsertBookmarkOrLike("bookmarks", userID, BlogID);
            }
            else if (State == 2) // Remove From Bookmarks
            {
                status = indexUtil.DeleteBookmarkOrLike("bookmarks", userID, BlogID);
            }
            return Json(new { status });
        }

        [SessionAuthorize]
        [Route("AjaxLike")]
        public JsonResult AjaxLike(FormCollection formCollection)
        {
            bool status = false;
            IndexUtil indexUtil = new IndexUtil();

            long userID = Convert.ToInt64(Session["userID"]);
            int State = Convert.ToInt32(formCollection["State"]);
            int BlogID = Convert.ToInt32(formCollection["BlogID"]);

            if (State == 1) // Like Blog
            {
                status = indexUtil.InsertBookmarkOrLike("likes", userID, BlogID);
            }
            else if (State == 2) // Remove From Like
            {
                status = indexUtil.DeleteBookmarkOrLike("likes", userID, BlogID);
            }
            return Json(new { status });
        }

        [HttpPost]
        [Route("AjaxIncrBlogViews")]
        public JsonResult AjaxIncrBlogViews(FormCollection formCollection)
        {
            IndexUtil indexUtil = new IndexUtil();

            int BlogID = Convert.ToInt32(formCollection["BlogID"]);
            indexUtil.IncrBlogViews(BlogID);
            return Json(new { status = true });
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}