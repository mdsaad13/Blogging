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
                ViewBag.Followers = 0;
                ViewBag.Blogs = 0;
            }            
        }

        internal void CatList(int CatId = 0)
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

            //bool MoreData = (CurrentCount < Count);

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
                        <div class=""col-6 col-sm-6 col-md-3"">
                            <div class=""info-box elevation-3"">
                                <span class=""info-box-icon ""><i class=""fas fa-{2}""></i></span>
                                <div class=""info-box-content"">
                                    <a href=""#"" class=""muted-link"">
                                        <span class=""info-box-text"">{0}</span>
                                    </a>
                                    <span class=""info-box-number"">
                                        {1}
                                        <small>Blogs</small>
                                    </span>
                                </div>
                            </div>
                        </div>
                    ",item.Name, item.FormatedCount, Icon);
                }
            }

            return Json(new { status, content = Content.ToString(), MoreData, CurrentDataCount });
        }
        
        [Route("categories/{CatName}")]
        public ActionResult SingleCategory(string CatName)
        {
            GetUserDetails();
            CatList();

            return View();
        }

    }
}