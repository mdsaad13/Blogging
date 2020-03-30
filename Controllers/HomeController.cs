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
    public class HomeController : Controller
    {
        public HomeController()
        {
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
                CommonUtil commonUtil = new CommonUtil();
                ViewBag.Followers = commonUtil.CountByArgs("Followers", $"Follow_userID = {userID}");
                ViewBag.Blogs = commonUtil.CountByArgs("blog", $"userID = {userID}");
            }            
        }

        internal void CatList(long CatId = 0)
        {
            IndexUtil indexUtil = new IndexUtil();
            ViewBag.Sidenav_CatList = indexUtil.PersonalizedCats(CatId);
        }
        
        public ActionResult Index()
        {
            GetUserDetails();
            CatList();

            IndexUtil indexUtil = new IndexUtil();

            List<AllBlogsModel> blogBundles = indexUtil.AllBlogs();

            return View(blogBundles);
        }
        
        [HttpGet]
        //[Route("Search/AddBlog/{blogid}/AddImages")]
        public ActionResult Search(string query)
        {
            GetUserDetails();
            CatList();

            IndexUtil indexUtil = new IndexUtil();

            List<AllBlogsModel> blogBundles = indexUtil.AllBlogs();

            return View("Index", blogBundles);
        }

        [Route("categories")]
        public ActionResult AllCategories()
        {
            GetUserDetails();
            CatList();

            IndexUtil indexUtil = new IndexUtil();
            List<CategoryModel> categories = indexUtil.AllCategories();

            return View(categories);
        }
        
        [HttpPost]
        public JsonResult AjaxAllCategories(FormCollection formCollection)
        {
            IndexUtil indexUtil = new IndexUtil();
            CommonUtil commonUtil = new CommonUtil();
            StringBuilder Content = new StringBuilder();

            bool MoreData = true;
            long CurrentDataCount = Convert.ToInt64(formCollection["CurrentDataCount"]);
            long ActualCount = commonUtil.Count("categories");
            
            List<CategoryModel> CategoriesList = indexUtil.AllCategories(CurrentDataCount);
            CurrentDataCount += CategoriesList.Count;

            if (CurrentDataCount >= ActualCount)
            {
                MoreData = false;
            }

            bool status = CategoriesList.Count > 0 ? true : false;
            string Icon;

            if (status)
            {
                foreach (var item in CategoriesList)
                {
                    Icon = item.Icon ?? "list-alt";

                    Content.AppendFormat(@"
                        <div class=""col-6 col-sm-6 col-md-3 div-wrapper-category"" id=""{3}"">
                            <div class=""info-box elevation-2"" onclick=""Redirect('/categories/{0}',{3})"">
                                <span class=""info-box-icon ""><i class=""fas fa-{2}""></i></span>
                                <div class=""info-box-content"">
                                    <span class=""info-box-text"">{0}</span>
                                    <span class=""info-box-number"">
                                        {1}
                                        <small>Blogs</small>
                                    </span>
                                </div>
                            </div>
                        </div>
                    ", item.Name, item.FormatedCount, Icon, item.CatID);
                }
            }

            return Json(new { status, content = Content.ToString(), MoreData, CurrentDataCount });
        }
        
        [HttpGet]
        [Route("categories/{CatName}")]
        public ActionResult SingleCategory(string CatName)
        {
            GetUserDetails();
            
            IndexUtil indexUtil = new IndexUtil();

            CategoryModel categoryModel = indexUtil.SingleCategory(CatName);

            List<AllBlogsModel> blogBundles = indexUtil.AllBlogs(0, categoryModel.CatID);

            CatList(categoryModel.CatID);

            ViewBag.CategoryDetails_CatID = categoryModel.CatID;
            ViewBag.CategoryDetails_Name = categoryModel.Name;
            ViewBag.CategoryDetails_Icon = categoryModel.Icon ?? "list-alt";
            ViewBag.CategoryDetails_Count = categoryModel.Count;
            ViewBag.CategoryDetails_FormatedCount = categoryModel.FormatedCount;

            return View(blogBundles);
        }
        
        [SessionAuthorize]
        [Route("bookmarks")]
        public ActionResult Bookmarks()
        {
            GetUserDetails();
            CatList();

            IndexUtil indexUtil = new IndexUtil();

            //List<AllBlogsModel> blogBundles = indexUtil.AllBlogs(0, categoryModel.CatID);

            return View();
        }
        
        [SessionAuthorize]
        [Route("liked")]
        public ActionResult Liked()
        {
            GetUserDetails();
            CatList();

            IndexUtil indexUtil = new IndexUtil();

            //List<AllBlogsModel> blogBundles = indexUtil.AllBlogs(0, categoryModel.CatID);

            return View();
        }

    }
}