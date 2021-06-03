using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuildInspect.Models.Domain;
using AutoMapper;
using System.Data.Entity;
using BuildInspect.Models.Utility;
using System.IO;

namespace BuildInspect.Models.Repository.Interface
{
    public class UserRepository : IUserRepository
    {
        BuildInspectEntities BInDB = new BuildInspectEntities();
         Logger logger = LogManager.GetCurrentClassLogger();

        public UserViewModel ValidateUserLoginMobile(string username, string password, string deviceid)
        {
            UserViewModel userViewModel = new UserViewModel();
            try
            {
                var user = BInDB.users.Where(a => a.UserName == username.Trim() && a.Password == password.Trim()).FirstOrDefault();  
                userViewModel = Mapper.Map<UserViewModel>(user);

                if (userViewModel != null)
                {
                    if (userViewModel.IsActive == 1)
                    {
                        var devUser = BInDB.users.Find(userViewModel.UserID);
                        devUser.DeviceID = deviceid;
                        BInDB.Entry(devUser).State = EntityState.Modified;
                        BInDB.SaveChanges();
                    }
                }

                return userViewModel;

            }
            catch (Exception ex)
            {
                logger.Debug("ValidateLoginMobile:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                logger.Info(username + ":" + password + ";");
                return userViewModel;
            }

        }

        public UserViewModel ValidateUserLoginWeb(string username, string password)
        {
            UserViewModel userViewModel = new UserViewModel();
            try
            {
                var user = BInDB.users.Where(a => a.UserName == username.Trim() && a.Password == password.Trim()).FirstOrDefault();
                //var user = BInDB.users.Where(a => a.UserName == username.Trim()).FirstOrDefault();
                userViewModel = Mapper.Map<UserViewModel>(user);              

                return userViewModel;

            }
            catch (Exception ex)
            {
                logger.Debug("ValidateLoginWeb:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                logger.Info(username + ":" + password + ";");
                return userViewModel;
            }

        }

        public bool VerifyCurrentPassword(int uid, string pwd)
        {
            var user = BInDB.users.Find(uid);
            if (user.Password == pwd)
                return true;
            else
                return false;
        }

        public UserViewModel GetUser(int uid)
        {
            var user = BInDB.users.Find(uid);
            return Mapper.Map<UserViewModel>(user);
        }

        public int CreateUser(UserViewModel user)
        {
            try
            {
                var _db_user = Mapper.Map<user>(user);
                BInDB.users.Add(_db_user);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CreateUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
                
            }
        }

        public int SaveUser(UserViewModel user)
        {            
            try
            {
                var _db_user = BInDB.users.Find(user.UserID);
                _db_user.CompanyID = user.CompanyID;
                _db_user.GroupID = user.GroupID;
                _db_user.DisplayName = user.DisplayName;
                _db_user.FirstName = user.FirstName;
                _db_user.LastName = user.LastName;
                _db_user.Designation = user.Designation;
                _db_user.Zone = user.Zone;
                _db_user.Email = user.Email;
                _db_user.Mobile = user.Mobile;
                _db_user.Password = user.Password;
                _db_user.SubCon_ID = user.SubCon_ID;
                _db_user.Assessor_ID = user.Assessor_ID;
                _db_user.UpdatedBy = user.UpdatedBy;
                _db_user.UpdatedDate = user.UpdatedDate;
                //var _db_user = Mapper.Map<user>(user);
                BInDB.Entry(_db_user).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("SaveUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public int DeleteUser(int userID)
        {
            try
            {
                var _db_user = BInDB.users.First(a => a.UserID == userID);
                //_db_user.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_user.UpdatedDate = DateTime.Now;
                //_db_user.IsActive = 0;

                //BInDB.Entry(_db_user).State = EntityState.Modified;
                BInDB.users.Remove(_db_user);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("DeleteUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                //return -1;
                return InActiveUser(userID);
            }
        }

        public int InActiveUser(int userID)
        {
            try
            {
                var _db_user = BInDB.users.First(a => a.UserID == userID);
                _db_user.UpdatedBy = AppSession.GetCurrentUserId();
                _db_user.UpdatedDate = DateTime.Now;
                _db_user.IsActive = 0;

                BInDB.Entry(_db_user).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("InActiveUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<UserViewModel> getAllUsers()
        {
            var users = BInDB.users.ToList();
            var lstUserView = Mapper.Map<List<UserViewModel>>(users);
            return lstUserView;
        }

        public List<GroupViewModel> GetUserGroups()
        {
            var usergroup = BInDB.usergroups.ToList();
            var lstGroupView = Mapper.Map<List<GroupViewModel>>(usergroup);
            return lstGroupView;
        }

        public bool CheckUser(string username)
        {
            try
            {
                var user = BInDB.users.Where(a => a.UserName == username).SingleOrDefault();
                if (user == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public int UpdateProfile(UserViewModel BIn_user, string newPwd)
        {
            try
            {
                var _db_user = BInDB.users.Find(BIn_user.UserID);
                _db_user.DisplayName = BIn_user.DisplayName;
                _db_user.Password = newPwd;
                BInDB.Entry(_db_user).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("ProfileUpdate:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }


        #region Company

       
        public List<CompanyMasterViewModel> GetAllCompanies()
        {
            var res = BInDB.company_master.OrderBy(a => a.CompanyName).ToList();
            var lists = Mapper.Map<List<CompanyMasterViewModel>>(res);

            return lists;
        }

        public bool CheckCompany(string name)
        {
            try
            {
                var res = BInDB.company_master.Where(a => a.CompanyName == name).SingleOrDefault();
                if (res == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        
        public  CompanyMasterViewModel GetCompany(int id)
        {
            var res = BInDB.company_master.Find(id);
            return Mapper.Map<CompanyMasterViewModel>(res);
        }

       
        public int CreateCompany(CompanyMasterViewModel cpy, string path)
        {
            try
            {
                if (cpy.profile_Path != null)
                {
                    var filename = Path.GetFileName(cpy.profile_Path.FileName);
                    var rn = DateTime.Now.ToString("yyMMddhhmmss");
                    var ext = Path.GetExtension(cpy.profile_Path.FileName);
                    var newfn = cpy.ShortName + "_" + rn + ext;

                    Directory.CreateDirectory(path);
                    var finalPath = path + newfn;
                    cpy.profile_Path.SaveAs(finalPath);
                    cpy.LogoPath = finalPath;
                }
                var db_cpy = Mapper.Map<company_master>(cpy);
                BInDB.company_master.Add(db_cpy);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CompanyCreation:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public int SaveCompany(CompanyMasterViewModel cpy, string path)
        {
            try
            {
                if (cpy.profile_Path == null)
                {
                    var _db_cpy = Mapper.Map<company_master>(cpy);
                    BInDB.Entry(_db_cpy).State = EntityState.Modified;
                    return BInDB.SaveChanges();
                }
                else
                {
                    File.Delete(cpy.LogoPath);
                    var _db_cpy = Mapper.Map<company_master>(cpy);

                    var filename = Path.GetFileName(cpy.profile_Path.FileName);
                    var rn = DateTime.Now.ToString("yyMMddhhmmss");
                    var ext = Path.GetExtension(cpy.profile_Path.FileName);
                    var newfn = cpy.ShortName + "_" + rn + ext;

                    //Directory.CreateDirectory(path);
                    var finalPath = path + newfn;
                    cpy.profile_Path.SaveAs(finalPath);
                    _db_cpy.LogoPath = finalPath;

                    BInDB.Entry(_db_cpy).State = EntityState.Modified;
                    return BInDB.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SaveCompany:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }

        }

        public int DeleteCompany(int cID)
        {
            try
            {
                var _db_res = BInDB.company_master.First(a => a.CompanyID == cID);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;
                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.company_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Company Deletion:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        #endregion


        #region SmartFM

        public SmartFMUserViewModel ValidateUserLoginSmartFM(string username, string password)
        {
            SmartFMUserViewModel userViewModel = new SmartFMUserViewModel();
            try
            {
                var user = BInDB.AppUsers.Where(a => a.UserName == username.Trim() && a.Password == password.Trim()).FirstOrDefault();
                userViewModel = Mapper.Map<SmartFMUserViewModel>(user);
               
                return userViewModel;

            }
            catch (Exception ex)
            {
                logger.Debug("ValidateLoginSmartFM:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                logger.Info(username + ":" + password + ";");
                return userViewModel;
            }

        }

        #endregion
    }

}