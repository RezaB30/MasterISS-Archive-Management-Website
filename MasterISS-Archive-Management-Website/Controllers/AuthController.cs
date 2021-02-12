﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MasterISS_Archive_Management_Website.Authentication;
using MasterISS_Archive_Management_Website.ViewModels;
using Microsoft.Owin;
using RezaB.Web.Authentication;


namespace MasterISS_Archive_Management_Website.Controllers
{
    public class AuthController: Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var authenticator = new CustomAuthenticator();
                var owinContext = HttpContext.GetOwinContext();
                var isSignedIn = authenticator.SignIn(owinContext, login.Username, login.Password);
                // valid user
                if (isSignedIn)
                {
                    // invalid permissions
                    if (!owinContext.Authentication.User.HasPermission("Archive Access"))
                    {
                        authenticator.SignOut(owinContext);
                    }
                    // valid login
                    else
                    {

                    }
                }
                ModelState.AddModelError("loginFailed", "ASS");
            }

            return View();
        }
    }
}