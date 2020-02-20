using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogging.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
            ViewBag.SoftwareName = "Blogging";
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Register()
        {
            return View();
        }
        
        public ActionResult Login()
        {
            return View();
        }
    }
}