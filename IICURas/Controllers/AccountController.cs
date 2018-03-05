using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using IICURas.Models;
using System.Net;
using System.Web.UI;
using System.Net.Mail;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using IICURas.Authorization;

namespace IICURas.Controllers
{
    [Restrict("Suspended")]
    public class AccountController : Controller
    {
        private readonly IICURasContext _db = new IICURasContext();

        [Authorize(Roles = "Administrator")]
        public ActionResult Users(string sortOrder = "CreateDate desc")
        {
            var userlist = (from u in _db.UserProfiles.Include("TrainingReviews").Include("TrainingReviews")
                            orderby u.UserName
                            select u).AsEnumerable()
                           .Select(u => new UserListViewModel
                           {
                               UserId = u.UserId,
                               UserName = u.UserName,
                               Name = u.ForeName + " " + u.SurName,
                               Institution = u.Institution,
                               Email = u.Email,
                               RoleName = string.Join(", ", Roles.GetRolesForUser(u.UserName)),
                               RegistrationDate = u.CreateDate.DateTime,
                               TrainingStart = u.TrainingReviews.Count(tr => tr.Status == "Current" ),
                               ReviewStart = u.ReviewCompletions.Count(tr => tr.Status == "Current"),
                               TrainingDone = u.TrainingReviews.Count(tr => tr.Status == "Current" && tr.Pass != null),
                               ReviewDone = u.ReviewCompletions.Count(tr => tr.Status == "Current" && tr.ComplesionDate != null)
                           });

            var result = userlist;

            switch (sortOrder)
            {
                case "UserName":
                    result = result.OrderBy(s => s.UserName).ThenBy(s => s.Name);
                    break;
                case "UserName desc":
                    result = result.OrderByDescending(s => s.UserName).ThenByDescending(s => s.Name);
                    break;
                case "Name":
                    result = result.OrderBy(s => s.Name).ThenBy(s => s.Name);
                    break;
                case "Name desc":
                    result = result.OrderByDescending(s => s.Name).ThenByDescending(s => s.Name);
                    break;
                case "Institution":
                    result = result.OrderBy(s => s.Institution).ThenBy(s => s.UserName);
                    break;
                case "Institution desc":
                    result = result.OrderByDescending(s => s.Institution).ThenByDescending(s => s.UserName);
                    break;
                case "RoleName":
                    result = result.OrderBy(s => s.RoleName).ThenBy(s => s.UserName);
                    break;
                case "RoleName desc":
                    result = result.OrderByDescending(s => s.RoleName).ThenByDescending(s => s.UserName);
                    break;
                case "TrainingDone":
                    result = result.OrderBy(s => s.TrainingDone).ThenBy(s => s.TrainingStart);
                    break;
                case "TrainingDone desc":
                    result = result.OrderByDescending(s => s.TrainingDone).ThenByDescending(s => s.TrainingStart);
                    break;
                case "RegistrationDate":
                    result = result.OrderBy(s => s.RegistrationDate).ThenBy(s => s.UserName);
                    break;
                case "RegistrationDate desc":
                    result = result.OrderByDescending(s => s.RegistrationDate).ThenBy(s => s.UserName);
                    break;
                case "ReviewDone":
                    result = result.OrderBy(s => s.ReviewDone).ThenBy(s => s.ReviewStart);
                    break;
                case "ReviewDone desc":
                    result = result.OrderByDescending(s => s.ReviewDone).ThenByDescending(s => s.ReviewStart);
                    break;
                default:
                    result = result.OrderBy(s => s.UserName);
                    break;
            }

            ViewBag.sortOrder = sortOrder;

            return View(result.ToList());
        }

        [Authorize]
        public ActionResult Edit(string username)
        {
            var userprofile = _db.UserProfiles.FirstOrDefault(u => u.UserName == username);

            if (userprofile == null)
            {
                return HttpNotFound();
            }

            UserViewModel userviewmodel = new UserViewModel()
            {
                ForeName = userprofile.ForeName,
                SurName = userprofile.SurName,
                UserName = userprofile.UserName,
                UserId = userprofile.UserId,
                Details = userprofile.Details,
                Email = userprofile.Email,
                Institution = userprofile.Institution
            };

            return View(userviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(UserViewModel userviewmodels)
        {
            var userprofile = _db.UserProfiles.FirstOrDefault(up => up.UserName == userviewmodels.UserName);

            if (!ModelState.IsValid) return View(userviewmodels);
            
                userprofile.ForeName = userviewmodels.ForeName;
                userprofile.SurName = userviewmodels.SurName;
                userprofile.Details = userviewmodels.Details;
                userprofile.Institution = userviewmodels.Institution;
                userprofile.Email = userviewmodels.Email;

                _db.SaveChanges();
                ViewBag.Status = "Updated successfully";
                return View(userviewmodels);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
             ViewBag.ReturnUrl = returnUrl;
            return View();         
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            ViewBag.ip = logUserAccess(model.UserName);

            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return  RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
           return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid) return View(model);
            // Attempt to register the user
            try
            {
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password, propertyValues: new {
                    SurName = model.SurName, 
                    ForeName = model.ForeName, 
                    Details = model.Details ,
                    Institution = model.Institution,
                    Email = model.Email
                });

                Roles.AddUserToRole(model.UserName, "Trainee");

                WebSecurity.Login(model.UserName, model.Password);

                return RedirectToAction("Index", "Home");
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult LostUsername()
        {
            ViewBag.sent = "no";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostUsername(LostUsernameModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user;

                var foundUserName = (from u in _db.UserProfiles
                                     where u.Email == model.Email
                                     select u.UserName).FirstOrDefault();

                user = foundUserName != null ? Membership.GetUser(foundUserName.ToString()) : null;

                if (user != null)
                {
                    var username = user.UserName;
                    var mail = new MailMessage();

                    mail.To.Add(new MailAddress(model.Email));
                    mail.From = new MailAddress("IICARus.Project@ed.ac.uk");
                    mail.Bcc.Add(new MailAddress("multipart.camarades@gmail.com"));
                    mail.Subject = "Request username for IICARus website";
                    mail.Body = "Someone has requrest username for this account on IICARus website. Please ignore this email if the request was not sent by you. <br/> Your username is:" + username + "<br><br/> Thank you, <br/>IICARus Group";

                    mail.IsBodyHtml = true;

                    var smtp = new SmtpClient();

                    // Attempt to send the email
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Issue sending email: " + e.Message + "<br/>");
                    }
                }
            }
            ViewBag.sent = "yes";
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            ViewBag.sent = "no";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(LostPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user;

                    var foundUserName = (from u in _db.UserProfiles
                                         where u.Email == model.Email
                                         select u.UserName).FirstOrDefault();

                    user = foundUserName != null ? Membership.GetUser(foundUserName.ToString()) : null;

                if (user != null)
                {
                    // Generae password token that will be used in the email link to authenticate user
                    var token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                    // Generate the html link sent via email 
                    var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { rt = token }, "http") + "'>Reset Password Link</a><br/>";

                    var mail = new MailMessage();

                    mail.To.Add(new MailAddress(model.Email));
                    mail.From = new MailAddress("IICARus.Project@ed.ac.uk");
                    mail.Bcc.Add(new MailAddress("multipart.camarades@gmail.com"));
                    mail.Subject = "Reset your password for IICARus website";
                    mail.Body = "Someone has requrest to reset password for this account on IICARus website. Please ignore this email if the request was not sent by you. <br/> If you want to reset your password, please click on the link: " + resetLink + "<br>,<br/> Thank you, <br/>IICARus Group";

                    mail.IsBodyHtml = true;

                    var smtp = new SmtpClient();

                    // Attempt to send the email
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Issue sending email: " + e.Message + "<br/>");
                    }
                }
            }
            ViewBag.sent = "yes";
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string rt)
        {
            ViewBag.Message = "";
            ResetPasswordModel model = new ResetPasswordModel {ReturnToken = rt};
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var resetResponse = WebSecurity.ResetPassword(model.ReturnToken, model.Password);
            ViewBag.Message = resetResponse ? "Successfully Changed" : "Something went wrong!";
            return View(model);
        }


        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (!ModelState.IsValid) return View(model);
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                var state = ModelState["OldPassword"];
                state?.Errors.Clear();

                if (!ModelState.IsValid) return View(model);
                try
                {
                    WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                    return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("",
                        $"Unable to create local account. An account with the name \"{User.Identity.Name}\" may already exist.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ManageRoles(int id = 0)
        {
            UserProfile userprofile = _db.UserProfiles.Find(id);

            if (userprofile == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserName = userprofile.UserName;
            ViewBag.UserId = userprofile.UserId;
            if (TempData["ResultMessage"] != null) ViewBag.ResultMessage = TempData["ResultMessage"];

            var rolesForThisUser = Roles.GetRolesForUser(userprofile.UserName);
            ViewBag.RolesForThisUser = rolesForThisUser;

            var allroles = new SelectList(Roles.GetAllRoles()).ToList();
            if (User.Identity.Name != "jliao") { allroles = allroles.Where(r => r.Text != "Funder").ToList(); }
            ViewBag.Roles = allroles;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult AddRoles(string roleName, string userName, string userId)
        {
            if (roleName == "") return RedirectToAction("ManageRoles", new {id = Convert.ToInt32(userId)});
            if (Roles.IsUserInRole(userName, roleName))
            {
                TempData["ResultMessage"] = "This user is already in this role.";
            }
            else
            {
                Roles.AddUserToRole(userName, roleName);
                TempData["ResultMessage"] = "Role added for this user successfully!";
            }
            //return View("ManageRoles");
            return RedirectToAction("ManageRoles", new { id = Convert.ToInt32(userId) });
        }

        //
        // POST: /Review/Delete/5

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteRole(string userId, string userName, string roleName)
        {
            if (Roles.IsUserInRole(userName, roleName))
            {
                Roles.RemoveUserFromRole(userName, roleName);
                TempData["ResultMessage"] = "Role removed from this user successfully!";
            }
            else
            {
                TempData["ResultMessage"] = "This user doesn't belong to selected role.";
            }

            ViewBag.RolesForThisUser = Roles.GetRolesForUser(userName);
            var list = new SelectList(Roles.GetAllRoles());
            TempData["AllRoles"] = list;
            ViewBag.Roles = list;

            //return View("ManageRoles"); 

            return RedirectToAction("ManageRoles", new { id = Convert.ToInt32(userId) });
        }

        //
        //POST: /Account/SuspendUser/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult SuspendUser(int id)
        {
            var userprofile = _db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }

            Roles.AddUserToRole(userprofile.UserName, "Suspended");

            return RedirectToAction("Users");
        }

        //
        //POST: /Account/UnSuspendUser/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult UnSuspendUser(int id)
        {
            var userprofile = _db.UserProfiles.Find(id);

            if (userprofile == null)
            {
                return HttpNotFound();
            }

            Roles.RemoveUserFromRole(userprofile.UserName, "Suspended");

            return RedirectToAction("Users");
        }

        public int Heartbeat(string userName)
        {
            var user = (from u in _db.UserProfiles where userName == u.UserName select u)
                        .FirstOrDefault();
            if (user != null)
            {
                user.LastHeartbeat = DateTime.Now;
                user.CurrentlyLogged = true;
                var count = _db.SaveChanges();
                return count;
            }
            return 0;
        }

        public ActionResult ActiveUsers()
        {
            return View();
        }

        public string LoadOnlineUsers()
        {
            var onlineusers = LoadActiveUsers();

            var table = "<table class='table table-bordered table-striped '> <thead> <tr> <th>User </th> <th>Latest Active Time</th><th></th></tr></thead><tbody id='user'>";

            foreach (var user in onlineusers)
            {
                table += "<tr><td>" + user.UserName + "</td>";
                table += "<td>" + user.LastHeartbeat + "</td></tr>";
            }

            table += "</tbody></table>";

            return table;

        }

        public int NumberOfActiveUsers()
        {
            return LoadActiveUsers().Count();
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers

        public IEnumerable<UserProfile> LoadActiveUsers()
        {
            Int32 expirationTime = 60 * 60;
            var allusers = _db.UserProfiles;
            foreach (var user in allusers)
            {
                if ((DateTime.Now - user.LastHeartbeat).TotalSeconds > expirationTime) user.CurrentlyLogged = false;

            }
            _db.SaveChanges();
            var onlineusers = allusers.Where(a => a.CurrentlyLogged == true);
            return onlineusers;
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public JsonResult ValidateUserName(string UserName)
        {
            return Json(!_db.UserProfiles.Any(m => m.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        private string getipaddress()
        {
            string szRemoteAddr = Request.UserHostAddress;
            string szXForwardedFor = Request.ServerVariables["X_FORWARDED_FOR"];
            string szIP = "";

            if (szXForwardedFor == null)
            {
                szIP = szRemoteAddr;
            }
            else
            {
                szIP = szXForwardedFor;
                if (szIP.IndexOf(",") > 0)
                {
                    string[] arIPs = szIP.Split(',');

                    foreach (string item in arIPs)
                    {
                        if (!IsPrivateIpAddress(item))
                        {
                            return item;
                        }
                    }
                }
            }
            return szIP;
        }

        private string logUserAccess(string username)
        {
            var ip = getipaddress();
            var filename = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "logs\\" + "logs.txt";
            var sw = new System.IO.StreamWriter(filename, true);

            try
            {
                sw.WriteLine(DateTime.Now.ToString() + "               " + ip + "               " + username);
            }
            catch (Exception e)
            {
                throw new Exception("writing to logs error: " + username + " : Error : " + e);
            }
            finally
            {
                sw.Close();
            }

            return ip;
        }

        #endregion
    }
}
