using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using BuildInspect.Models.Service.Imp;

namespace BuildInspect.Models.Authentication
{
    public class BasicAuthenticationMessageHandler : DelegatingHandler
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private const string UnauthorizedMessage = "Unauthorized request";
        private const string OldWebServiceMessage = "Please update your app to the latest version.";
        private const string CREDENTIAL_USER_TYPE = "User";
        private const string CREDENTIAL_SUBSCRIBER_TYPE = "Subscriber";
        //private IUserService userService ;
        //private ISubscriberService subscriberService;

        /// <summary>
        /// Validates the request. If successful pass the request to the controller.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string rest = request.Content.ReadAsStringAsync().Result;
            logger.Debug("Raw Http Message: {0}", rest);
            //if (request.RequestUri.AbsolutePath.ToLower().StartsWith(SettingsHelper.GetLatestWebServiceAbsolutePathSetting()))
            {
                //if (!(request.RequestUri.AbsolutePath.ToLower().Equals(SettingsHelper.GetLatestWebServiceAbsolutePathSetting() + "user/login")))
                {
                    var isAuthenticated = IsAuthenticated(request);
                    logger.Debug("isAuthenticated : " + isAuthenticated);
                    if (!isAuthenticated)
                    {
                        logger.Debug("return 401");
                        var response = request.CreateErrorResponse(HttpStatusCode.Unauthorized, UnauthorizedMessage);
                        response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(
                            Configuration.BasicAuthenticationScheme));
                        return response;
                    }
                }
            }
            //else
            //{
            //    var response = request.CreateErrorResponse(HttpStatusCode.Unauthorized, OldWebServiceMessage);
            //    return response;
            //}
            return await base.SendAsync(request, cancellationToken);
        }
        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        protected bool IsAuthenticated(HttpRequestMessage request)
        {

            bool authenticated = false;
            if (request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/"))
            {
                if (request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/bqaslogin") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallassessmentprojects") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getassessmentprojectsummary") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallassessmentlocations") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallmodules") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallmoduleprocess") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getalldirections") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getalljoints") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallleaks") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallwalls") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallwindows") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallwetareawatertightnesstestresults") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getassessmentmastersync") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getexternalwallheader") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getexternalwalldetail") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/saveexternalwall") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/updateexternalwall") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/deleteexternalwall") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getexternalworksheader") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getexternalworksdetail") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/saveexternalworks") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/updateexternalworks") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/deleteexternalworks") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getinternalfinishesheader") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getinternalfinishesdetail") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/saveinternalfinishes") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/updateinternalfinishes") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/deleteinternalfinishes") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getroofconstructionheader") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getroofconstructiondetail") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/saveroofconstruction") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/updateroofconstruction") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/deleteroofconstruction") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getfieldwindowwatertightnesstestheader") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getfieldwindowwatertightnesstestdetail") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/savefieldwindowwatertightnesstest") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/updatefieldwindowwatertightnesstest") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/deletefieldwindowwatertightnesstest") ||

                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getwetareawatertightnesstestheader") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getwetareawatertightnesstestdetail") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/savewetareawatertightnesstest") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/updatewetareawatertightnesstest") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/deletewetareawatertightnesstest") ||

                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallqcinspectionprojects") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallqcinspectionprojectsbygroupid") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallrfwidrawingreferencefiles") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallprojectfiles") ||
                    
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallqcinspectiontrades") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallqcinspectionsubcontractors") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallqcinspectiondefecttypes") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallrfwitrades") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallqcinspectiongeneralchecklists") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getrfwiformavailableslots") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallrtoinspectors") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallmeinspectors") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallstructureinspectors") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallotherinspectors") ||

                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getqcinspectionandrfwimastersync") ||
                    
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getqcinspectiondefectformlist") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getqcinspectiondefectformfilelist") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/saveqcinspectiondefectform") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/updateqcinspectiondefectform") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/deleteqcinspectiondefectform") ||
                    
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getrfwiformlist") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/saverfwiform") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/updaterfwiform") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/deleterfwiform") ||

                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/postuserimage") ||                    
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/user/postuserimage") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/sfmlogin" ) 
                    )
                {
                    authenticated = true;
                }
                else
                    if (request.Method.Equals(HttpMethod.Post) || (request.Method.Equals(HttpMethod.Get)))
                {

                    AuthenticationHeaderValue authValue = request.Headers.Authorization;
                    if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
                    {
                        Credentials parsedCredentials = ParseAuthorizationHeader(authValue.Parameter);

                        if (parsedCredentials != null)
                        {
                           
                            IUserServices userService = request.GetDependencyScope().GetService(typeof(IUserServices)) as IUserServices;
                            //var returnUser = userService.ValidateUserLogin(parsedCredentials.Username, parsedCredentials.Password);
                            //if (returnUser != null)
                            //{
                            //    if (returnUser.IsActive == 1)
                            //    {
                            //        authenticated = true;
                            //    }
                            //}

                                          


                        }
                    }
                }
            }
           

            return authenticated;
        }


        /// <summary>
        /// Returns the credentials of the request.
        /// </summary>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        private Credentials ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = Encoding.ASCII.GetString(Convert
                                                            .FromBase64String(authHeader))
                                                            .Split(
                                                            new[] { ':' });



            SecureString credPwd = new SecureString();
            try
            {

               // credPwd = CryptAlgorithm.GetEncryptedSecureString(credentials[1]);

                //logger.Info(string.Format("UserName:{0} - PassWord:{1}", credentials[0], encryptedPassword));
                logger.Info(string.Format("UserName:{0} - Type:{1}", credentials[0], credentials[2]));

                if (string.IsNullOrEmpty(credentials[0])
                    || string.IsNullOrEmpty(credentials[1])) return null;
                return new Credentials()
                {
                    Username = credentials[0],
                    Password = credentials[1],
                    Type = credentials[2]

                };

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                try
                {
                    credPwd.Dispose();
                }
                catch (Exception ex)
                {

                }

            }

        }
    }

    public class Configuration
    {
        public const string UsernameHeader = "X-ApiAuth-Username";
        public const string AuthenticationScheme = "ApiAuth";
        public const string BasicAuthenticationScheme = "Basic";
        public const int ValidityPeriodInMinutes = 5;
    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
    }
}