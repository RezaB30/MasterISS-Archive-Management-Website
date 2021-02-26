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
                    if (!owinContext.Authentication.AuthenticationResponseGrant.Principal.HasPermission("Archive Access"))
                    {
                        ViewBag.HasNotPermission = MasterISS_Archive_Management_Website.Localization.Model.HasNotPermission;
                        //ViewBag.Username = owinContext.Authentication.AuthenticationResponseGrant.Identity.Name;
                        authenticator.SignOut(owinContext);
                    }
                    // valid login
                    else
                    {
                        return RedirectToAction("Index", "Archive");
                    }
                }
                else
                {
                    ModelState.AddModelError("loginFailed",MasterISS_Archive_Management_Website.Localization.Model.LoginFailed );
                }
            }
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