using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildInspect.Models.ViewModel;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Utility;
using BuildInspect.Models.Security;
using BuildInspect.Models.Domain;
using System.Collections.Generic;
using System.Configuration;
using BuildInspect.Models;

namespace BuildInspect.Controllers
{
    [AccessDeniedAuthorize]
    public class UserController : SuperBaseController
    {
        private BuildInspectEntities db = new BuildInspectEntities();
        private readonly IUserServices userService;
        private readonly IAssessmentServices assessmentService;

        public UserController(IAssessmentServices _assessmentService, IUserServices _userService)
        {
            assessmentService = _assessmentService;
            userService = _userService;
        }

        // GET: User
        public ActionResult Index()
        {
            AppSession.SetCurrentPage("/User/Index");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(userService.getAllUsers().Where(a => a.GroupID > 1 && a.IsActive != 0).ToList());
            }
            else
            {
                return View(userService.getAllUsers().Where(a => a.GroupID > 2 && a.IsActive != 0 && (a.CompanyID == cid || a.CompanyID == null)).ToList());
            }
        }

        // GET: User/Create
        public ActionResult Create()
        {
            AppSession.SetCurrentPage("/User/Create");
            if (AppSession.GetCurrentUserGroup() == 1)
            {
                var grp = userService.GetUserGroups().Where(a => a.GroupID != 1).ToList();
                ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName");

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList();
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName");
            }
            var cpyID = AppSession.GetCompanyId();

            if (AppSession.GetCurrentUserGroup() == 2)
            {
                var grp = userService.GetUserGroups().Where(a => a.GroupID > 2 && a.GroupID != 4 && a.GroupID != 7).ToList();
                ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName");

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1 && a.CompanyID == cpyID).ToList();
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel user)
        {
            if (userService.CheckUser(user.UserName) == false)
            {
                var AssessorId = 0;
                if (user.GroupID == 7)
                {
                    AssessorsMasterViewModel Assessor = new AssessorsMasterViewModel();
                    Assessor.AssessorsName = user.DisplayName;
                    Assessor.CompanyID = AppSession.GetCompanyId();
                    Assessor.CreatedBy = AppSession.GetCurrentUserId();
                    Assessor.CreatedDate = DateTime.Now;
                    Assessor.IsActive = 1;
                    AssessorId = assessmentService.CreateAssessor(Assessor);
                }
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, user.Password);
                user.CreatedBy = AppSession.GetCurrentUserId();
                user.CreatedDate = DateTime.Now;
                user.IsActive = 1;
                user.Assessor_ID = AssessorId;
                user.Password = encWord;                    
                var result = userService.CreateUser(user);
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
                return getFailedOperation("User Id Already exits!");                    
            }
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // win_users win_users = db.win_users.Find(id);
            var user = userService.GetUser(id ?? default(int));
            if (user == null)
            {
                return HttpNotFound();
            }
            

            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //   eng_users eng_users = db.eng_users.Find(id);
            var user = userService.GetUser(id ?? default(int));
            if (user == null)
            {
                return HttpNotFound();
            }
            AppSession.SetCurrentPage("/User/Edit/" + id.ToString());
            if (AppSession.GetCurrentUserGroup() == 1)
            {
                var grp = userService.GetUserGroups().Where(a => a.GroupID != 1).ToList();
                ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName", user.GroupID);

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList();
                //cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName", user.CompanyID);
            }
            var cpyID = AppSession.GetCompanyId();

            if (AppSession.GetCurrentUserGroup() == 2)
            {
                var grp = userService.GetUserGroups().Where(a => a.GroupID > 2 && a.GroupID != 4 && a.GroupID != 7).ToList();
                grp.Insert(0, new GroupViewModel() { GroupID = 0, GroupName = "-Select-" });
                ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName", user.GroupID);

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1 && a.CompanyID == cpyID).ToList();
                cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName", user.CompanyID);
            }

            //var grp = userService.GetUserGroups().Where(a => a.GroupID != 1).ToList();
            //grp.Insert(0, new GroupViewModel() { GroupID = 0, GroupName = "-Select-" });
            //ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName", user.GroupID);

            var sKey = ConfigurationManager.AppSettings["PtStK"];
            SecurityController Scon = new SecurityController();
            var plnPwd = Scon.Decrypt(sKey, user.Password);
            user.Password = plnPwd;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, user.Password);
               
                user.Password = encWord;
                user.UpdatedBy = AppSession.GetCurrentUserId();
                user.UpdatedDate = DateTime.Now;
                var result = userService.SaveUser(user);
                if (result > 0)
                {
                    if (user.GroupID == 7)
                    {
                        if (user.Assessor_ID > 0)
                        {
                            var Assessor = assessmentService.GetAssessor(int.Parse(user.Assessor_ID.ToString()));
                            if (Assessor != null)
                            {
                                Assessor.AssessorsName = user.DisplayName;
                                Assessor.UpdatedBy = AppSession.GetCurrentUserId();
                                Assessor.UpdatedDate = DateTime.Now;
                                Assessor.IsActive = 1;
                                result = assessmentService.SaveAssessor(Assessor);
                            }
                        }
                    }
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }              
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = userService.GetUser(id);
            if (user != null)
            {
                if(user.GroupID == 7)
                {
                    var Assessor = assessmentService.GetAssessor(int.Parse(user.Assessor_ID.ToString()));
                    if (Assessor != null)
                    {
                        var result1 = assessmentService.DeleteAssessor(Assessor.AssessorsID);
                        if (result1 < 0)
                        {
                            return getFailedOperation();
                        }
                    }
                }
            }
            var result = userService.DeleteUser(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            AppSession.SetCurrentPage("/User/ChangePassword");
            return View();
        }

        // POST: /User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel userpwd)
        {
            
            var sKey = ConfigurationManager.AppSettings["PtStK"];
            SecurityController Scon = new SecurityController();
            var encWord = Scon.Encrypt(sKey, userpwd.OldPassword);

            if (userService.VerifyCurrentPassword(AppSession.GetCurrentUserId(),encWord) == true)
            {
                encWord = Scon.Encrypt(sKey, userpwd.NewPassword);
                var user = userService.GetUser(AppSession.GetCurrentUserId());
                user.UpdatedBy = AppSession.GetCurrentUserId();
                user.UpdatedDate = DateTime.Now;
                user.Password = encWord;
                var result = userService.SaveUser(user);
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
                return getFailedOperation("You old password is wrong!");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
