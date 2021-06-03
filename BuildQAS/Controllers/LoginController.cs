using BuildInspect.Models.ViewModel;
using BuildInspect.Models.Service.Imp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BuildInspect.Models.Utility;
using BuildInspect.Models.Service.Interface;
using System.Configuration;
using NLog;

namespace BuildInspect.Controllers
{
    public class LoginController : SuperBaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IUserServices userService;

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="_userService"></param>
        public LoginController(IUserServices _userService)
        {
            userService = _userService;          

        }

        // GET: Login
        public ActionResult Index()
        {
            AppSession.SetCurrentPage("");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, model.Password);

                var returnUser = userService.ValidateUserLoginWeb(model.UserName, encWord);
                
                //var company = userService.getCompany();
                if (returnUser != null)
                {
                    if (returnUser.IsActive == 1)
                    {
                        FormsAuthentication.SetAuthCookie(returnUser.UserName,false);
                        AppSession.SetCurrentUserId(returnUser.UserID);
                        AppSession.SetCurrentUserGroup(returnUser.GroupID?? default(int));
                        AppSession.SetCurrentUserName(returnUser.DisplayName);
                        AppSession.SetCompanyId(returnUser.CompanyID ?? default(int));
                        if(returnUser.GroupID < 4)
                        {
                            AppSession.SetCurrentMenu("Admin", "", "Index");
                        }
                        else if (returnUser.GroupID == 7)
                        {
                            AppSession.SetCurrentMenu("Assessment", "", "Index");
                        }
                        else
                        {
                            AppSession.SetCurrentMenu("QCInspection", "", "Index");
                        }
                        AppSession.SetUserDetail(returnUser);
                        AppSession.SetCompanyDetail(userService.GetCompany((int)returnUser.CompanyID));
                        //logger.Info("Login successful:");
                        return RedirectToAction("Index", "Home");
                    }
                    else {
                        model.StatusFlag = 2;
                        return View(model);
                    }

                }
                model.StatusFlag = 1;
                return View(model);
            }

            return View(model);
        }

        public ActionResult SignOut()
        {
            AppSession.SetCurrentPage("");
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Index", "Login");
        }
    }
}