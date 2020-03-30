using Blogging.DAL;
using Blogging.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogging.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
            ViewBag.SoftwareName = "Blogger";
        }

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

            IndexUtil indexUtil = new IndexUtil();
            ViewBag.Sidenav_CatList = indexUtil.PersonalizedCats();
        }

        // GET: Account
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        [Route("register")]
        public ActionResult Register()
        {
            return View();
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult RegisterAjax(AccountModel accountModel)
        {
            CommonUtil commonUtil = new CommonUtil();

            // Validating mobile number
            if (!commonUtil.Validate("users", "phone", accountModel.Mobile))
            {
                // Validating username
                if (!commonUtil.Validate("users", "username", accountModel.UserName))
                {
                    // mobile number does not exists
                    // All Good!
                    
                    //Generating a random integer for ID
                    Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    Random random = new Random();
                    int randomNo = random.Next(1000);

                    accountModel.ID = Convert.ToInt64(string.Format("{0}{1}", unixTimestamp, randomNo));

                    if (accountModel.Image != null)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(accountModel.Image.FileName);
                        uniqueFileName = uniqueFileName + extension;

                        string path = Server.MapPath("~/Images/UserProfiles/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        accountModel.Image.SaveAs(path + uniqueFileName);
                        accountModel.ImgUrl = uniqueFileName;
                    }

                    AccountUtil accountUtil = new AccountUtil();

                    if(accountUtil.CreatAcc(accountModel))
                    {
                        Session["userID"] = accountModel.ID;
                        return Json(new { errorCode = 4 });
                    }
                    else
                    {
                        return Json(new { errorCode = 3 });
                    }
                    
                }
                else
                {
                    // username exists!
                    return Json(new { errorCode = 2 });
                }
            }
            else
            {
                // mobile number exists!
                return Json(new { errorCode = 1 });
            }
        }

        [Route("login")]
        public ActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult LoginAjax(AccountModel accountModel)
        {
            bool status = false;
            string errorText = "Unknown error";

            AccountUtil accountUtil = new AccountUtil();
            int result = accountUtil.Login(accountModel.unameORmobile, accountModel.Password);
            if(result == 1)
            {
                status = true;
                errorText = "";
            }
            else if(result == 0)
            {
                status = false;
                errorText = "Invalid credientials";
            }
            else if(result == 2)
            {
                status = false;
                errorText = "Invalid credientials";
            }

            return Json(new { status, errorText });
        }

        [Route("user/{Username}")]
        public ActionResult ViewProfile(string Username)
        {
            GetUserDetails();

            AccountUtil accountUtil = new AccountUtil();
            CommonUtil commonUtil = new CommonUtil();
            if (commonUtil.Validate("users", "username", Username))
            {
                ProfileModel profileModel = accountUtil.GetUserByUname(Username);

                long CurrentUserID = Convert.ToInt64(Session["userID"]);

                string query = $"userID = {CurrentUserID} AND Follow_userID = {profileModel.UserID}";
                ViewData["Follow"] = commonUtil.RawValidate("Followers", query) ? "Unfollow" : "Follow";

                return View(profileModel);
            }
            else
            {
                string Info = "User not found :(";
                ViewBag.Info = Info;
                return View("Error");
            }
        }

        [SessionAuthorize]
        public JsonResult FollowUser(FormCollection formCollection)
        {
            bool status = false;
            long UserID = Convert.ToInt64(formCollection["userID"]);
            long CurrentUserID = Convert.ToInt64(Session["userID"]);
            AccountUtil accountUtil = new AccountUtil();
            status = accountUtil.InsertFollow(CurrentUserID, UserID);
            return Json(new { status });
        }

        [SessionAuthorize]
        public JsonResult UnFollowUser(FormCollection formCollection)
        {
            bool status = false;
            long UserID = Convert.ToInt64(formCollection["userID"]);
            long CurrentUserID = Convert.ToInt64(Session["userID"]);
            AccountUtil accountUtil = new AccountUtil();
            status = accountUtil.DeleteFollow(CurrentUserID, UserID);
            return Json(new { status });
        }

        [Route("logout")]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Error()
        {
            return View();
        }

    }
}