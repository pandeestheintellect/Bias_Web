﻿using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;

namespace BuildInspect.Models.Utility
{
    public static class AppSettings
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static string DateTimeFormat = "DateTimeFormat";
        private static string MAIL_SETTINGS_SECTION = "system.net/mailSettings";

        private static string MAIL_SETTINGS_MKV = "mailSettings/mkv";
        private static string MAIL_SETTINGS_HEC = "mailSettings/hec";
        private static string MAIL_SETTINGS_PPL = "mailSettings/ppl";

        private static string DEFAULT_FROM_MAIL_ADDRESS = ConfigurationManager.AppSettings["defaultEmail"];
        private static string WEB_CONFIG_FILE = "~\\Web.config";

        private static string DEFAULT_FROM = ConfigurationManager.AppSettings["defaultFrom"];

        public static string GetDateFormat()
        {
            return ConfigurationManager.AppSettings[DateTimeFormat];
        }

        public static void sendEmail(string subject, string body, string toEmailAddress, string ccEmailAddress, string attachments, int companyid)
        {
            
            MailMessage mail = new MailMessage();

            try
            {
                mail.IsBodyHtml = true;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                var mConfigurationFile = WebConfigurationManager.OpenWebConfiguration(WEB_CONFIG_FILE);
                //MailSettingsSectionGroup mMailSettings = mConfigurationFile.GetSectionGroup(MAIL_SETTINGS_SECTION) as MailSettingsSectionGroup;
                SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection(MAIL_SETTINGS_MKV);
                if (companyid == 1)
                    {
                    smtpSection = (SmtpSection)ConfigurationManager.GetSection(MAIL_SETTINGS_MKV);
                    //mMailSettings = mConfigurationFile.GetSectionGroup(MAIL_SETTINGS_MKV) as MailSettingsSectionGroup;
                }
                    if (companyid == 2)
                    {
                        smtpSection = (SmtpSection)ConfigurationManager.GetSection(MAIL_SETTINGS_HEC);
                        //mMailSettings = mConfigurationFile.GetSectionGroup(MAIL_SETTINGS_HEC) as MailSettingsSectionGroup;
                    }
                    if (companyid == 3)
                    {
                       smtpSection = (SmtpSection)ConfigurationManager.GetSection(MAIL_SETTINGS_PPL);
                    //mMailSettings = mConfigurationFile.GetSectionGroup(MAIL_SETTINGS_PPL) as MailSettingsSectionGroup;
                }
                //set the addresses
                //if (mMailSettings != null)

                if (smtpSection != null)
                {
                    mail.From = new MailAddress(smtpSection.From, DEFAULT_FROM);
                }
                else
                {
                    mail.From = new MailAddress(DEFAULT_FROM_MAIL_ADDRESS, DEFAULT_FROM);
                }
                
                string[] TOId = toEmailAddress.Split(';');
                foreach (string TOEmail in TOId)
                {
                    mail.To.Add(new MailAddress(TOEmail)); //Adding Multiple To email Id
                }
                //mail.To.Add(toEmailAddress);
                

                string[] CCId = ccEmailAddress.Split(',');

                foreach (string CCEmail in CCId)
                {
                    mail.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                }

                if (attachments != "Reminder")
                {
                    string[] AttId = attachments.Split(';');
                    foreach (string attach in AttId)
                    {
                        if (attach != "" && attach != null)
                            mail.Attachments.Add(new Attachment(attach)); //Adding Multiple Attachements
                    }
                }

               
                    // mail.Attachments.Add(emailAttachment); //Adding Multiple Attachements
                    // mail.Attachments.Add(new Attachment("F:\\Proj-Pandees\\EBI\\BuildInspect_UITest\\BuildInspect\\images\\CLPdfFiles\\CL_1.pdf"));

                    mail.Subject = subject;

                if (attachments == "Reminder")
                {
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    mail.AlternateViews.Add(plainView);
                }
                else
                {
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(body, null, "text/plain");
                    mail.AlternateViews.Add(plainView);
                }

                using (SmtpClient client = new SmtpClient(smtpSection.Network.Host, smtpSection.Network.Port))
                {
                     //var emailuser = ConfigurationManager.AppSettings["sEmailuser"];
                     //var emailpass = ConfigurationManager.AppSettings["sEmailpass"];

                    client.Credentials = new System.Net.NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                    if (companyid == 1 )
                    {
                    client.EnableSsl = true;
                    }
                    if (companyid == 3)
                    {
                        client.EnableSsl = true;
                    }


                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                
                logger.Error("Error occured in SendMail():");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                throw ex;
                
                
            }
            finally
            {
                mail.Dispose();
            }
        }

       

        public static void getWorkTimeHours(decimal nwh, decimal wwh, decimal lbh, string startTime, string endTime, bool wehFlag, bool lbFlag, out decimal workHours, out decimal otHours)
        {
            var normal_work_hours = nwh;
            var weekend_work_hours = wwh;
            var lunch_break_hours = lbh;
            decimal normal_ot_work_hours = normal_work_hours;
            decimal weekend_ot_work_hours = weekend_work_hours;

            TimeSpan workhours = Convert.ToDateTime(endTime) - Convert.ToDateTime(startTime);

            var totWorkHours = Convert.ToDecimal(workhours.TotalHours);

            if(wehFlag)
            {
                normal_work_hours = wwh;

            }

            if(totWorkHours <= normal_work_hours)
            {
                workHours = totWorkHours;
                otHours = 0;
            }
            else
            {
                if (lbFlag)
                {
                    workHours = normal_work_hours;
                    otHours = totWorkHours - normal_work_hours - lunch_break_hours;
                   
                }
                else
                {
                    workHours = normal_work_hours;
                    otHours = totWorkHours - normal_work_hours;
                }

            }
                      

        }

        public static DateTime GetNextQuartelry(DateTime currentMonthDate)
        {
            var curmon = currentMonthDate.Month;
            var curyear = currentMonthDate.Year;
            DateTime nextquarter;            
            string dt;
            nextquarter = DateTime.ParseExact("30/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (curmon <= 3)
            {
                dt = "31/03/" + curyear.ToString();
                nextquarter = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture); 
            }
            if (curmon >= 4 && curmon <= 6)
            {
                dt = "30/06/" + curyear.ToString();
                nextquarter = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (curmon >= 7 && curmon <= 9)
            {
                dt = "30/09/" + curyear.ToString();
                nextquarter = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (curmon >= 10)
            {
                dt = "31/12/" + curyear.ToString();
                nextquarter = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            return nextquarter;
        }

        public static DateTime GetNextHalfYearly(DateTime currentMonthDate)
        {
            var curmon = currentMonthDate.Month;
            var curyear = currentMonthDate.Year;
            DateTime nexthy;
            string dt;
            nexthy = DateTime.ParseExact("30/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (curmon <= 6)
            {
                dt = "30/06/" + curyear.ToString();
                nexthy = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (curmon >= 7)
            {
                dt = "31/12/" + curyear.ToString();
                nexthy = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }            
            return nexthy;
        }

        public static DateTime GetNextYearly(DateTime currentMonthDate)
        {
            var curmon = currentMonthDate.Month;
            var curyear = currentMonthDate.Year;
            DateTime nexty;
            string dt;
            nexty = DateTime.ParseExact("30/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (curmon <= 6)
            {
                dt = "30/06/" + curyear.ToString();
                nexty = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            if (curmon >= 7)
            {
                curyear += 1;
                dt = "30/06/" + curyear.ToString();
                nexty = DateTime.ParseExact(dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            return nexty;
        }



        public static string NumberToCurrencyText(decimal number, MidpointRounding midpointRounding)
        {
            // Round the value just in case the decimal value is longer than two digits
            number = decimal.Round(number, 2, midpointRounding);

            string wordNumber = string.Empty;

            // Divide the number into the whole and fractional part strings
            string[] arrNumber = number.ToString().Split('.');

            // Get the whole number text
            long wholePart = long.Parse(arrNumber[0]);
            string strWholePart = NumberToText(wholePart);

            // For amounts of zero dollars show 'No Dollars...' instead of 'Zero Dollars...'
            //wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? " Singapore Dollar and " : " Singapore Dollars and ");
           // wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? " and " : " and ");

            wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? " " : " "); ;

            // If the array has more than one element then there is a fractional part otherwise there isn't
            // just add 'No Cents' to the end
            if (arrNumber.Length > 1)
            {
                // If the length of the fractional element is only 1, add a 0 so that the text returned isn't,
                // 'One', 'Two', etc but 'Ten', 'Twenty', etc.
                long fractionPart = long.Parse((arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1]));
                string strFarctionPart = NumberToText(fractionPart);
                //wordNumber += (fractionPart == 0 ? " No " : strFarctionPart) + (fractionPart == 1 ? " Cent" : " Cents");

                if (fractionPart != 0)
                wordNumber += " and " + (fractionPart == 0 ? "" : strFarctionPart) + (fractionPart == 1 ? " Cent" : " Cents");
            }
            else
                wordNumber += "";
            //wordNumber += "No Cents";

            return wordNumber;
        }


        public static string NumberToText(long number)
        {
            StringBuilder wordNumber = new StringBuilder();

            string[] powers = new string[] { "Thousand ", "Million ", "Billion " };
            string[] tens = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            string[] ones = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                                       "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

            if (number == 0) { return "Zero"; }
            if (number < 0)
            {
                wordNumber.Append("Negative ");
                number = -number;
            }

            long[] groupedNumber = new long[] { 0, 0, 0, 0 };
            int groupIndex = 0;

            while (number > 0)
            {
                groupedNumber[groupIndex++] = number % 1000;
                number /= 1000;
            }

            for (int i = 3; i >= 0; i--)
            {
                long group = groupedNumber[i];

                if (group >= 100)
                {
                    wordNumber.Append(ones[group / 100 - 1] + " Hundred ");
                    group %= 100;

                    if (group == 0 && i > 0)
                        wordNumber.Append(powers[i - 1]);
                }

                if (group >= 20)
                {
                    if ((group % 10) != 0)
                        wordNumber.Append(tens[group / 10 - 2] + " " + ones[group % 10 - 1] + " ");
                    else
                        wordNumber.Append(tens[group / 10 - 2] + " ");
                }
                else if (group > 0)
                    wordNumber.Append(ones[group - 1] + " ");

                if (group != 0 && i > 0)
                    wordNumber.Append(powers[i - 1]);
            }

            return wordNumber.ToString().Trim();
        }


        //static void SendPushNotificationTest()
        //{
        //    string serverKey = "AAAASOigIK8:APA91bEvUjJvHiK62Jp8Zw1UizkmMATm9tEt4rSvxY0EImq5HgfsggJNfde8H71Am7s5yCuEVAny7NjIKOKWzxSf5gZskCslDjS0HvUmbdKuVWKvqELPCPtGxK9pIKnhnRJRmtogVFJO";
        //    string senderid = "313140453551";
        //    try
        //    {
        //        var result = "-1";
        //        var webAddr = "https://fcm.googleapis.com/fcm/send";

        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
        //        httpWebRequest.ContentType = "application/json";
        //        httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
        //        httpWebRequest.Method = "POST";

        //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //        {
        //            string json = "{\"to\": \"Your device token\",\"data\": {\"message\": \"This is a Firebase Cloud Messaging Topic Message!\",}}";
        //            streamWriter.Write(json);
        //            streamWriter.Flush();
        //        }

        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            result = streamReader.ReadToEnd();
        //        }

        //        // return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        //  Response.Write(ex.Message);
        //    }
        //}


        public static int SendPushNotification(string deviceid, string body)
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAASOigIK8:APA91bEvUjJvHiK62Jp8Zw1UizkmMATm9tEt4rSvxY0EImq5HgfsggJNfde8H71Am7s5yCuEVAny7NjIKOKWzxSf5gZskCslDjS0HvUmbdKuVWKvqELPCPtGxK9pIKnhnRJRmtogVFJO"));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", "313140453551"));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                to = deviceid,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = body,
                    title = "SBI CL Reminder",
                    badge = 1
                },
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                    }
                }
            }
            return 1;
        }

        public static String ConvertFileToBase64String(string FileFullPath,string FileName)
        {
            if(File.Exists(FileFullPath))
            {
                Byte[] bytes = File.ReadAllBytes(FileFullPath);
                return (FileName.Split('.')[1].ToLower() == "png" ? "data:image/png;base64," : (FileName.Split('.')[1].ToLower() == "jpeg" || FileName.Split('.')[1].ToLower() == "jpg" ? "data:image/jpg;base64," : "")) + Convert.ToBase64String(bytes);
            }
            return null;
        }

        public static void ConvertBase64StringToFile(string FileBase64String, string FileFullPath)
        {
            Byte[] bytes = Convert.FromBase64String(FileBase64String.Split(',')[1]);
            File.WriteAllBytes(FileFullPath, bytes);
        }

    }


}

    

   


