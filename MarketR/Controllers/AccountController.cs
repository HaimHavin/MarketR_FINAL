#region Using

using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MarketR.Models;
using static MarketR.AdAuthenticationService;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.DirectoryServices;
using MarketR.Common;
using System.Configuration;
using MarketR.Email;
using System.Web.Hosting;

#endregion

namespace MarketR.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        EventLog eventLog = new EventLog();

        // TODO: This should be moved to the constructor of the controller in combination with a DependencyResolver setup
        // NOTE: You can use NuGet to find a strategy for the various IoC packages out there (i.e. StructureMap.MVC5)
        private readonly UserManager _manager = UserManager.Create();

        // GET: /account/forgotpassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();
            return View();
        }

        // GET: /account/login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();
            // Store the originating URL so we can attach it to a form field
            var viewModel = new AccountLoginModel { ReturnUrl = returnUrl };
            return View(viewModel);
        }

        // POST: /account/login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(AccountLoginModel viewModel)
        {
            try
            {
                if (!viewModel.Switch)
                {
                    var userDetail = await _manager.FindAsync(password: viewModel.Password, userName: viewModel.Username);

                    if (userDetail != null)
                    {

                        var roles = await _manager.GetRolesAsync(userDetail.Id);

                        if (userDetail.UserName == "Admin" || userDetail.UserName == "Dr" || userDetail.UserName == "Mo")
                        {
                            await SignInAsync(userDetail, viewModel.RememberMe);
                            Session["UserRole"] = roles[0];
                            Session["UserId"] = userDetail.Id;
                            Session["UserData"] = Encrypt(viewModel.Username + ", pass =" + viewModel.Password);
                            eventLog.SaveEventLog("Login", "SucessFul");
                            return RedirectToAction("upload", "Marketr", new { user = Session["UserData"] });
                        }

                        if (!string.IsNullOrEmpty(roles[0]) && roles[0].ToLower() != "system_reciever")
                        {
                            var authService = new AdAuthenticationService();

                            // If a user was found
                            if (authService.SignIn(viewModel.Username, viewModel.Password).IsSuccess)
                            {

                                // Then create an identity for it and sign it in
                                string role = "";
                                var context = new PrincipalContext(ContextType.Domain, "192.168.1.201", viewModel.Username, viewModel.Password);
                                var searcher = context != null ? new PrincipalSearcher(new UserPrincipal(context)) : new PrincipalSearcher();
                                foreach (var result in searcher.FindAll())
                                {
                                    DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                                    string name = (string)de.Properties["samAccountName"].Value ?? "";
                                    if (name == viewModel.Username)
                                    {
                                        role = (string)de.Properties["title"].Value ?? "";
                                        break;
                                    }

                                }
                                //Session["UserRole"] =  _manager.GetRolesAsync(userDetail.Id).ToString();
                                // If the user came from a specific page, redirect back to it
                                // return RedirectToLocal(viewModel.ReturnUrl);
                                //return RedirectToAction("upload", "Marketr", new { user = user.UserName, pass = viewModel.Password });
                                //if (!string.IsNullOrEmpty(roles[0]) && roles[0] == role)
                                if (!string.IsNullOrEmpty(roles[0]) && roles[0] == role)
                                {
                                    await SignInAsync(userDetail, viewModel.RememberMe);
                                    Session["UserRole"] = roles[0];
                                    Session["UserId"] = userDetail.Id;
                                    Session["UserData"] = Encrypt(viewModel.Username + ", pass =" + viewModel.Password);
                                    eventLog.SaveEventLog("Login", "SucessFul");
                                    return RedirectToAction("upload", "Marketr", new { user = Session["UserData"] });
                                }

                            }
                        }
                    }
                }//
                else
                {
                    await AllSignInAsync(viewModel.Username, viewModel.RememberMe);
                    Session["UserRole"] = "mo";
                    Session["UserData"] = Encrypt(viewModel.Username + ", pass =" + viewModel.Password);
                    return RedirectToAction("upload", "Marketr", new { user = Session["UserData"] });
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", "Invalid username or password.");
            }
            // No existing user was found that matched the given criteria

            ModelState.AddModelError("", "Invalid username or password.");
            // If we got this far, something failed, redisplay form
            return View("Login", viewModel);
        }
        /// <summary>
        /// Encrypting the password and query string
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public string Encrypt(string clearText)
        {

            try
            {
                string EncryptionKey = ConfigurationManager.AppSettings["encryptionKey"].ToString();
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                    encryptor.Key = pdb.GetBytes(32);

                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {

                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {

                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }

                        clearText = Convert.ToBase64String(ms.ToArray());

                    }
                }

            }
            catch (Exception ex)
            {
                string errorFile1 = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog.txt");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(errorFile1, true))
                {
                    file.WriteLine(ex.Message.ToString());
                }
            }
            return clearText;
        }

        // GET: /account/error
        [AllowAnonymous]
        public ActionResult Error()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();
            return View();
        }

        // GET: /account/register
        [AllowAnonymous]
        public ActionResult Register()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();
            return View(new AccountRegistrationModel());
        }

        // POST: /account/register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AccountRegistrationModel viewModel)
        {
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
                return View(viewModel);
            // Prepare the identity with the provided information
            var user = new IdentityUser
            {
                UserName = viewModel.Username ?? viewModel.Email,
                Email = viewModel.Email
            };

            // Try to create a user with the given identity
            try
            {
                var result = await _manager.CreateAsync(user, viewModel.Password);
                // If the user could not be created
                if (!result.Succeeded)
                {
                    // Add all errors to the page so they can be used to display what went wrong
                    AddErrors(result);
                    return View(viewModel);
                }

                // If the user was able to be created we can sign it in immediately
                // Note: Consider using the email verification proces
                await SignInAsync(user, false);
                return RedirectToLocal();
            }
            catch (DbEntityValidationException ex)
            {
                // Add all errors to the page so they can be used to display what went wrong
                AddErrors(ex);
                string mailSubjectForRegistration = "User registration Failure";
                string mailBodyForRegistration = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplate.html")))
                {
                    mailBodyForRegistration = reader.ReadToEnd();
                }
                mailBodyForRegistration = mailBodyForRegistration.Replace("[ErrorDate]", DateTime.Now.ToString());
                mailBodyForRegistration = mailBodyForRegistration.Replace("[ErrorDetail]", ex.StackTrace.ToString());
                mailBodyForRegistration = mailBodyForRegistration.Replace("[MessageDetail]", "the system tried to register a user into the MarketR System.");
                SendingEmail.SendMail(mailSubjectForRegistration, mailBodyForRegistration);
                return View(viewModel);
            }
        }

        // POST: /account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            //Clearing all the seesion variable
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            // First we clean the authentication ticket like always
            FormsAuthentication.SignOut();
            // Second we clear the principal to ensure the user does not retain any authentication
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            // Last we redirect to a controller/action that requires authentication to ensure a redirect takes place
            // this clears the Request.IsAuthenticated flag since this triggers a new request
            return RedirectToLocal();
        }

        private ActionResult RedirectToLocal(string returnUrl = "")
        {
            // If the return url starts with a slash "/" we assume it belongs to our site
            // so we will redirect to this "action"
            if (!returnUrl.IsNullOrWhiteSpace() && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            // If we cannot verify if the url is local to our host we redirect to a default location
            return RedirectToAction("index", "home");
        }

        private void AddErrors(DbEntityValidationException exc)
        {
            foreach (var error in exc.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors.Select(validationError => validationError.ErrorMessage)))
            {
                ModelState.AddModelError("", error);
            }
        }

        private void AddErrors(IdentityResult result)
        {
            // Add all errors that were returned to the page error collection
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (Request.IsAuthenticated)
                Logout();
        }

        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            try
            {
                // Clear any lingering authencation data
                FormsAuthentication.SignOut();
                // Create a claims based identity for the current user
                var identity = await _manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                // Write the authentication cookie
                FormsAuthentication.SetAuthCookie(identity.Name, isPersistent);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task AllSignInAsync(string user, bool isPersistent)
        {
            try
            {
                // Clear any lingering authencation data
                FormsAuthentication.SignOut();
                // Create a claims based identity for the current user
                //var identity = await _manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                // Write the authentication cookie
                FormsAuthentication.SetAuthCookie(user, isPersistent);
            }
            catch (Exception ex)
            {

            }
        }


        // GET: /account/lock
        [AllowAnonymous]
        public ActionResult Lock()
        {
            return View();
        }
    }
}