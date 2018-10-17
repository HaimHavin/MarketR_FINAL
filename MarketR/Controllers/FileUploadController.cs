using MarketR.Email;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarketR.Controllers
{
    public class FileUploadController : Controller
    {
        // GET: FileUpload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsyncUpload(IEnumerable<HttpPostedFileBase> files)
        {
            int count = 0;
            try
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                            file.SaveAs(path);
                            count++;
                        }
                    }
                }
                return new JsonResult { Data = "Successfully " + count + " file(s) uploaded" };
            }
            catch (Exception ex)
            {
                string mailSubjectForRegistration = "Creating user Failure";
                string mailBodyForRegistration = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplate.html")))
                {
                    mailBodyForRegistration = reader.ReadToEnd();
                }
                mailBodyForRegistration = mailBodyForRegistration.Replace("[ErrorDate]", DateTime.Now.ToString());
                mailBodyForRegistration = mailBodyForRegistration.Replace("[ErrorDetail]", ex.StackTrace.ToString());
                mailBodyForRegistration = mailBodyForRegistration.Replace("[MessageDetail]", "the system tried to create user into the MarketR System.");
                SendingEmail.SendMail(mailSubjectForRegistration, mailBodyForRegistration);
                throw ex;
            }
        }
    }
}