using System;
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
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.LoginPage = "LoginPage";
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login,string ReturnUrl)
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
                    if (!owinContext.Authentication.AuthenticationResponseGrant.Principal.HasPermission("Archive Access"))
                    {
                        ViewBag.HasNotPermission = MasterISS_Archive_Management_Website.Localization.Model.HasNotPermission;
                        authenticator.SignOut(owinContext);
                    }
                    // valid login
                    else
                    {
                        if (string.IsNullOrEmpty(ReturnUrl))
                        {
                            return RedirectToAction("Index", "Archive");
                        }
                        return Redirect(ReturnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("loginFailed",MasterISS_Archive_Management_Website.Localization.Model.LoginFailed );
                    ViewBag.LoginPage = "LoginPage";
                }
            }
            ViewBag.LoginPage = "LoginPage";
            return View();
        }


        //[HttpPost]       
        public ActionResult LogOut()
        {   var authenticator = Request.GetOwinContext().Authentication;
            authenticator.SignOut();
            return RedirectToAction("Login","Auth");
        }
    }
}