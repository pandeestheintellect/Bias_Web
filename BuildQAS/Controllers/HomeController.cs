using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuildInspect.Models.Security;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Service.Interface;
using BuildInspect.Models.Utility;
using BuildInspect.Models.ViewModel;
using System.IO;
using System.Configuration;

namespace BuildInspect.Controllers
{
    [AccessDeniedAuthorize]
    public class HomeController : SuperBaseController
    {
        private readonly IUserServices userService;

        public HomeController(UserServices _userService)
        {
            userService = _userService;
        }

        public ActionResult Index()
        {
            var groupid = Models.Utility.AppSession.GetCurrentUserGroup();
            var userid = Models.Utility.AppSession.GetCurrentUserId();
            var Ivm = new IndexViewModel();
            if (userid == 0)
            {
                AppSession.SetCurrentPage("");
                return RedirectToAction("Index", "Login");
            }
            else
            {
                
                var user = userService.GetUser(userid);
                if (user != null)
                {
                    Ivm.DisplayName = user.DisplayName;
                    return View(Ivm);
                }
                else
                {
                    AppSession.SetCurrentPage("");
                    return RedirectToAction("Index", "Login");
                }
            }
        }
        public ActionResult Navbar()
        {            
            return PartialView();
        }
        public ActionResult LeftMenu()
        {
            return PartialView();
        }

        public ActionResult AdminIndex()
        {
            AppSession.SetCurrentMenu("Admin", "", "Index");
            AppSession.SetCurrentPage("/Home/AdminIndex");
            return View();
        }

        public ActionResult profile_modal()
        {
            var uid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(uid);
            var sKey = ConfigurationManager.AppSettings["PtStK"];
            SecurityController Scon = new SecurityController();
            var plnPwd = Scon.Decrypt(sKey, user.Password);
            user.Password = plnPwd;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(UserViewModel Bin_user)
        {
            ModelState.Remove("Email");
            if (ModelState.IsValid)
            {
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, Bin_user.Curr_Password);

                if (userService.VerifyCurrentPassword(Bin_user.UserID, encWord))
                {
                    //Session["success"] = "Profile updated successfully";
                    var newPwd = Scon.Encrypt(sKey, Bin_user.Password);
                    var res = userService.UpdateProfile(Bin_user, newPwd);
                    return getSuccessfulOperation();
                }
                else
                {
                    //Session["error"] = "Incorrect Password";
                    return getFailedOperation("Incorrect Password");
                }
                
            }
            return getFailedOperation();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region Company

        // GET: Company
        public ActionResult CompanyIndex()
        {
            AppSession.SetCurrentMenu("Admin", "", "Index");
            AppSession.SetCurrentPage("/Home/CompanyIndex");
            return View(userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList());
        }

        public ActionResult CompanyCreate()
        {
            AppSession.SetCurrentPage("/Home/CompanyCreate");
            return View(new CompanyMasterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyCreate(CompanyMasterViewModel company)
        {
            if (userService.CheckCompany(company.CompanyName) == false)
            {

                var path = Path.Combine(Server.MapPath("~/images/CompanyLogo/"));
                company.IsActive = 1;
                company.CreatedBy = AppSession.GetCurrentUserId();
                company.CreatedDate = DateTime.Now;
                var result = userService.CreateCompany(company, path);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            else
            {
                return getFailedOperation("Company Name Already exits!");
            }
        }

        public ActionResult CompanyEdit(int? id)
        {
            AppSession.SetCurrentPage("/Home/CompanyEdit/" + id.ToString());
            var cpy = userService.GetCompany(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyEdit(CompanyMasterViewModel company)
        {
            var path = Path.Combine(Server.MapPath("~/images/CompanyLogo/"));
            company.UpdatedBy = AppSession.GetCurrentUserId();
            company.UpdatedDate = DateTime.Now;

            var result = userService.SaveCompany(company, path);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteCompany")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCompany(int id)
        {
            var result = userService.DeleteCompany(id);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }

        }

        #endregion
    }
}