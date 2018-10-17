using MarketR.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using MarketR.Common;
using MarketR.Utilities;
using System.IO;
using MarketR.Email;

namespace MarketR.Controllers
{
    public class UserController : Controller
    {
        EventLog eventLog = new EventLog();
        private readonly UserManager _manager = UserManager.Create();
        MarketREntities context = new MarketREntities();
        /// <summary>
        /// shwoing list of all users
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var users = (from user in context.AspNetUsers
                         select new User
                         {
                             Id = user.Id,
                             UserName = user.UserName,
                             Email = user.Email,
                             Role = user.AspNetRoles.Select(x => x.Name).FirstOrDefault()
                         }).ToList();

            return View(users);
        }
        /// <summary>
        /// Showing edit page with user detail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Edit(string Id)
        {
            eventLog.SaveEventLog(ConstantEvent.UserEditRequest, ConstantEvent.Successful);
            AspNetUser aspNetUser = context.AspNetUsers.Find(Id);
            List<AspNetRole> roles = context.AspNetRoles.ToList();
            List<SelectListItem> roleList = new List<SelectListItem>();
            foreach (var item in roles)
            {
                SelectListItem list = new SelectListItem
                {
                    Value = item.Id,
                    Text = item.Name
                };
                roleList.Add(list);
            }

            User user = new User()
            {
                Id = aspNetUser.Id,
                UserName = aspNetUser.UserName,
                Email = aspNetUser.Email,
                PhoneNumber = aspNetUser.PhoneNumber,
                Roles = roleList
            };
            var userRole = aspNetUser.AspNetRoles.FirstOrDefault();
            if (userRole != null)
            {
                user.Role = userRole.Id;
            }
            return View(user);
        }

        /// <summary>
        /// creating new user 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult CreateUser()
        {
            eventLog.SaveEventLog(ConstantEvent.NewUserCreateRequest, ConstantEvent.Successful);
            return View(new AccountRegistrationModel());
        }
        /// <summary>
        /// Saving the new user detail
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateUser(AccountRegistrationModel viewModel)
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
                    eventLog.SaveEventLog(ConstantEvent.NewUserCreated, ConstantEvent.Failed);
                    return View(viewModel);
                }
                eventLog.SaveEventLog(ConstantEvent.NewUserCreated, ConstantEvent.Successful);
                return RedirectToAction("List");
            }
            catch (DbEntityValidationException ex)
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
                return View(viewModel);
            }
        }

        /// <summary>
        /// Showing user detail 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(string id)
        {

            AspNetUser aspNetUser = context.AspNetUsers.Find(id);
            if (!string.IsNullOrEmpty(id))
            {
                User user = new User()
                {
                    Id = aspNetUser.Id,
                    UserName = aspNetUser.UserName,
                    Email = aspNetUser.Email,
                    PhoneNumber = aspNetUser.PhoneNumber
                };
                var userRole = aspNetUser.AspNetRoles.FirstOrDefault();
                if (userRole != null)
                {
                    user.Role = userRole.Name;
                }
                eventLog.SaveEventLog(ConstantEvent.UserDetailRequested, ConstantEvent.Successful);
                return View(user);
            }
            else
            {
                eventLog.SaveEventLog(ConstantEvent.UserDetailRequested, ConstantEvent.Failed);
                return RedirectToAction("List");
            }
        }
        /// <summary>
        /// update user detail
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserName,Email,Role")]User user)
        {

            var userDetail = await _manager.FindByNameAsync(user.UserName);
            userDetail.UserName = user.UserName;
            userDetail.Email = user.Email;
            await _manager.UpdateAsync(userDetail);
            var userRoles = await _manager.GetRolesAsync(userDetail.Id);
            string newRole = context.AspNetRoles.Find(user.Role).Name.ToString();
            var roleName = "";
            if (userRoles.Count > 0)
            {
                roleName = userRoles.ToList()[0].ToString();
                await _manager.RemoveFromRolesAsync(userDetail.Id, roleName);
            }
            await _manager.AddToRolesAsync(userDetail.Id, newRole);
            eventLog.SaveEventLog(ConstantEvent.UserDetailUpdated, ConstantEvent.Successful);
            return RedirectToAction("List");
        }
        /// <summary>
        /// Delete the user based on userid
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(string Id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = context.AspNetUsers.Find(Id);
                    context.AspNetUsers.Remove(user);
                    context.SaveChanges();
                    eventLog.SaveEventLog(ConstantEvent.UserDeleted, ConstantEvent.Successful);
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    string mailSubjectForRegistration = "Deleting user Failure";
                    string mailBodyForRegistration = "";
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplate.html")))
                    {
                        mailBodyForRegistration = reader.ReadToEnd();
                    }
                    mailBodyForRegistration = mailBodyForRegistration.Replace("[ErrorDate]", DateTime.Now.ToString());
                    mailBodyForRegistration = mailBodyForRegistration.Replace("[ErrorDetail]", ex.StackTrace.ToString());
                    mailBodyForRegistration = mailBodyForRegistration.Replace("[MessageDetail]", "the system tried to delete user from the MarketR System.");
                    SendingEmail.SendMail(mailSubjectForRegistration, mailBodyForRegistration);
                }
            }
            return View();
        }

        /// <summary>
        /// searching user username, email
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SearchUser(string search)
        {
            // if (!string.IsNullOrEmpty(search))
            // {
            try
            {
                var users = (from user in context.AspNetUsers
                             where user.UserName.Contains(search) || user.Email.Contains(search)
                             select new User
                             {
                                 Id = user.Id,
                                 UserName = user.UserName,
                                 Email = user.Email,
                                 Role = (string)user.AspNetRoles.Select(x => x.Name).FirstOrDefault()
                             }).ToList();
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new HttpStatusCodeResult(400, "Ajax error test"), JsonRequestBehavior.AllowGet);
            }
            //}
            //var result = new { Success = false };
            // return Json(new HttpStatusCodeResult(400, "Ajax error test"), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Showing Nostro Deals
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult NostroDeals()
        {
            List<FolderPermission> folderPermission = new List<FolderPermission>();
            try
            {
                if (Session["UserId"] != null)
                {
                    if (Session["UserRole"].ToString() == "Admin" || Session["UserRole"].ToString() == "Middle")
                        folderPermission = context.FolderPermissions.ToList();
                    else if (Session["UserRole"].ToString() == "DR")
                        folderPermission = context.FolderPermissions.Where(x => x.Folder == "Non Nostro").ToList();
                    else if (Session["UserRole"].ToString() == "Nostro")
                        folderPermission = context.FolderPermissions.Where(x => x.Folder == "Nostro").ToList();
                }
            }
            catch (Exception ex)
            {
                string mailSubjectForRegistration = "Nostro/Non-Nostro deals loading Failure";
                string mailBodyForRegistration = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplate.html")))
                {
                    mailBodyForRegistration = reader.ReadToEnd();
                }
                mailBodyForRegistration = mailBodyForRegistration.Replace("[ErrorDate]", DateTime.Now.ToString());
                mailBodyForRegistration = mailBodyForRegistration.Replace("[ErrorDetail]", ex.StackTrace.ToString());
                mailBodyForRegistration = mailBodyForRegistration.Replace("[MessageDetail]", "the system tried to load all the deals into the MarketR System.");
                SendingEmail.SendMail(mailSubjectForRegistration, mailBodyForRegistration);
            }
            return View(folderPermission);
        }
    }
}
