using MarketR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace MarketR.Email
{
    public static class SendingEmail
    {
        /// <summary>
        /// sending email to dba for any failure in website
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendMail(string subject, string body)
        {
            try
            {
                var context = new MarketREntities();
                var toEmail = context.AspNetRoles.Where(ru => ru.Name.ToLower() == "system_reciever")
                    .SelectMany(rp => rp.AspNetUsers.Select(r => r.Email)).ToList();

                string from = ConfigurationManager.AppSettings["fromMail"].ToString();
                string fromPassword = ConfigurationManager.AppSettings["fromMailPassword"].ToString();
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(from);
                    for (int i = 0; i <= 1; i++)
                    { 
                        mail.To.Add(new MailAddress(toEmail[i].ToString()));
                    }
                    mail.IsBodyHtml = true;
                    mail.Subject = subject;
                    mail.Body = body;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential(from, fromPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex) { }
        }
    }
}