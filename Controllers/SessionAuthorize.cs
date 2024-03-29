﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogging.Controllers
{
    public class SessionAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.Session["userID"] == null)
            {
                context.Result = new RedirectResult("/login");
            }
        }
    }
    
    public class IsSessionAuthorized : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.Session["userID"] != null)
            {
                context.Result = new RedirectResult("/Home");
            }
        }
    }

}