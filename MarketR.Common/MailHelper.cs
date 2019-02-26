using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.Common
{
    public static class MailHelper
    {
        public static void SendMail(string subject, string body, string toEmail)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {                  
                    mail.To.Add(new MailAddress(toEmail));
                    mail.IsBodyHtml = true;
                    mail.Subject = subject;
                    mail.Body = body;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential();
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
