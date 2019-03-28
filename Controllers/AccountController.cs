using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using TravelAndTourismWebsite.Models;
using TravelAndTourismWebsite.WebsiteLanguages;
using TravelAndTourismWebsite.Resources;
using System.Security.Cryptography;
using System.Text;
using TravelAndTourismWebsite.Repository;

namespace TravelAndTourismWebsite.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null && user.Active)
                {
                    await SignInAsync(user, model.RememberMe);
                    ApplicationUser CurrentUser = db.Users.Find(user.Id);
                    CurrentUser.LoginCount++;
                    Session.Add("LastLoginDateTime", CurrentUser.LastLoginDateTime);
                    CurrentUser.LastLoginDateTime = DateTime.Now;
                    db.SaveChanges();
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login2(LoginViewModel model)
        {
              //Finding a user with the provided username and password
                var user = await UserManager.FindAsync(model.UserName, model.Password);

                //Checking if the user was found and it's active
                if (user != null && user.Active)
                {
                    //Signing in
                    await SignInAsync(user, isPersistent: false);

                    //Calculating logins count and saving last login date and time
                    ApplicationUser CurrentUser = db.Users.Find(user.Id);
                    CurrentUser.LoginCount++;
                    Session.Add("LastLoginDateTime", CurrentUser.LastLoginDateTime);
                    CurrentUser.LastLoginDateTime = DateTime.Now;
                    db.SaveChanges();


                    return Json(true);

                }
            

            return Json(false);
        }


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            try
            {
                //Getting list of countries
                List<Country> countries = db.Countries.Where(x => x.Visible).ToList();

                //Displaying countries in current website language
                switch (SiteLanguages.GetCurrentLanguageCulture())
                {
                    case "en-US":
                        ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                        break;
                    case "ar-SA":
                        ViewBag.Country = new SelectList(countries, "Id", "Name_Ar");
                        break;
                    default:
                        ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                        break;
                }
                //ViewBag.ReturnUrl = returnUrl;
            }
            catch { }
            return View();
        }

        /*
         * Get cities list in a country 
         */
        [AllowAnonymous]
        public JsonResult Citylist(int id)
        {
           
            List<SelectListItem> li = new List<SelectListItem>();
            List<City> cities = db.Cities.Where(x => x.Country.Id == id).ToList();

            foreach (City c in cities)
            {
                switch (SiteLanguages.GetCurrentLanguageCulture())
                {
                    case "en-US":
                        li.Add(new SelectListItem() { Text = c.Name_En, Value = c.Id.ToString() });
                        break;
                    case "ar-SA":
                        li.Add(new SelectListItem() { Text = c.Name_Ar, Value = c.Id.ToString() });
                        break;

                    default:
                        li.Add(new SelectListItem() { Text = c.Name_En, Value = c.Id.ToString() });
                        break;
                }
            }

            return Json(li, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
           

            //Getting list of countries to display if registration failed
            List<Country> countries = db.Countries.ToList();

            if (ModelState.IsValid)
            {
                //Checking if the email is already used
                var user = db.Users.Where(x => x.Email == model.Email).SingleOrDefault();
                if (user != null)
                {
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                    ModelState.AddModelError("EmailUsed", Resource.EmailUsed);
                    return View(model);
                }

                //Checking if the username is already used
                user = db.Users.Where(x => x.UserName == model.UserName).SingleOrDefault();
                if (user != null)
                {
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                    ModelState.AddModelError("UsernameUsed", Resource.UsernameUsed);
                    return View(model);
                }

                //Creating a new user
                user = new ApplicationUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Country = db.Countries.Find(int.Parse(model.Country)).Name_En,
                    City = db.Cities.Find(int.Parse(model.City)).Name_En,
                    UserName = model.UserName,
                    Active = true,
                    LoginCount = 0,
                    RegistrationDateTime = DateTime.Now,
                };
                var result = await UserManager.CreateAsync(user, model.Password);

                var roleStore = new RoleStore<IdentityRole>(db);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);

                switch (SiteLanguages.GetCurrentLanguageCulture())
                {
                    case "en-US":
                        user.PreferredInterfaceLanguage = "English";
                        break;
                    case "ar-SA":
                        user.PreferredInterfaceLanguage = "Arabic";
                        break;
                    default:
                        user.PreferredInterfaceLanguage = "English";
                        break;
                }
                db.SaveChanges();
                
                if (!roleManager.RoleExists("User"))
                {
                    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                    role.Name = "User";
                    roleManager.Create(role);
                }
                

                //assigning user to role [User]
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "User");
                    Session.Add("newUser", true);
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            //Displaying countries in current website language
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                    break;
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? Resource.PasswordChanged
                : message == ManageMessageId.Error ? Resource.ErrorOccured
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            ViewBag.Alert = "alert";
            return View();
        }


        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /*
         * Update User information
         */
        public ActionResult UpdateAccount()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            UpdateAccountViewModel ua = new UpdateAccountViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                City = user.City
            };

            List<Country> countries = db.Countries.Where(x => x.Visible).ToList();
           
            Country country = db.Countries.SingleOrDefault(x => x.Name_En == user.Country);
            List<City> cities = db.Cities.Where(x => x.Country.Id == country.Id).ToList();
            City city = db.Cities.SingleOrDefault(x => x.Name_En == user.City);

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En", country.Id);
                    ViewBag.city = new SelectList(cities, "Id", "Name_En", city.Id);
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_Ar", country.Id);
                    ViewBag.city = new SelectList(cities, "Id", "Name_Ar", city.Id);
                    break;
                default:
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En", country.Id);
                    ViewBag.city = new SelectList(cities, "Id", "Name_En", city.Id);
                    break;
            }

            return View(ua);
        }

        [HttpPost]
        public ActionResult UpdateAccount(UpdateAccountViewModel model)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            List<Country> countries = db.Countries.Where(x => x.Visible).ToList();

            Country country = db.Countries.SingleOrDefault(x => x.Name_En == user.Country);
            List<City> cities = db.Cities.Where(x => x.Country.Id == country.Id).ToList();
            City city = db.Cities.SingleOrDefault(x => x.Name_En == user.City);

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En", country.Id);
                    ViewBag.city = new SelectList(cities, "Id", "Name_En", city.Id);
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_Ar", country.Id);
                    ViewBag.city = new SelectList(cities, "Id", "Name_Ar", city.Id);
                    break;
                default:
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En", country.Id);
                    ViewBag.city = new SelectList(cities, "Id", "Name_En", city.Id);
                    break;
            }

            try
            {

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Gender = model.Gender;
                user.Country = db.Countries.Find(int.Parse(model.Country)).Name_En;
                user.City = db.Cities.Find(int.Parse(model.City)).Name_En;

                db.SaveChanges();

                ViewBag.Message = Resource.AccountUpdated;
                ViewBag.Alert = "alert alert-success";
                return View(model);
            }

            catch
            {

            }

            ViewBag.Message = Resource.ErrorUpdateAccount;
            ViewBag.Alert = "alert alert-danger";

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }


        /*
         * Recover user account password
         */
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = db.Users.SingleOrDefault(x => x.Email == model.Email);
                if (user != null)
                {
                    //Creating a new password for user
                    string newPassword = "";
                    int passwordSize = 12;
                    char[] chars = new char[62];
                    chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                    byte[] data = new byte[1];

                    using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                    {
                        crypto.GetNonZeroBytes(data);
                        data = new byte[passwordSize];
                        crypto.GetNonZeroBytes(data);
                    }
                    StringBuilder result = new StringBuilder(passwordSize);
                    foreach (byte b in data)
                    {
                        result.Append(chars[b % (chars.Length)]);
                    }
                    newPassword = result.ToString();

                    //Hashing the new password
                    string newPasswordHashed = UserManager.PasswordHasher.HashPassword(newPassword);
                    user.PasswordHash = newPasswordHashed;
                    db.SaveChanges();

                    //Sending the new password to user by email
                    string subject = "", body = "";
                    switch (user.PreferredInterfaceLanguage)
                    {
                        case "English":
                            subject = "Travel And Tourism - Password Changed!";
                            body = "Dear User: " + user.UserName + ". \n" +
                                   "Your new password is:\n [ " + newPassword + " ] \n" +
                                   "for a safer account, please change it when you login to your account.\n" +
                                   "\n\n\n DSH Travel And Tourism team.";
                            break;
                        case "Arabic":
                            subject = "السياحة والسفر - تم تغيير كلمة المرور";
                            body = "المستخدم العزيز: " + user.UserName + ". \n" +
                                   "كلمة المرور الجديدة الخاصة بك هي: \n[ " + newPassword + " ] \n" +
                                   "الرجاء تغييرها عند تسجيل دخولك القادم من أجل حساب اكثر امانا.\n" +
                                   "\n\n\n فريق ادارة موقع السياحة والسفر.";
                            break;
                        default:
                            subject = "Travel And Tourism: Password Changed!";
                            body = "Dear User: " + user.UserName + ". \n" +
                                   "Your new password is:\n [ " + newPassword + " ] \n" +
                                   "for a safer account, please change it when you login to your account.\n" +
                                   "\n\n\n DSH Travel And Tourism team.";
                            break;
                    }

                    Email email = new Email(model.Email, subject, body);
                    email.Send();

                    ViewBag.PwdSent = true;
                    ViewBag.Message = Resource.PasswordChangedMsg;
                }
                else
                {
                    ViewBag.PwdSent = false;
                    ViewBag.Alert = "alert";
                    ViewBag.Message = Resource.PasswordChangedMsgEmailNotFound;
                }

                return PartialView("_ResetPasswordPartial");
            }
            
              ViewBag.PwdSent = false;
              ViewBag.Alert = "alert";
              ViewBag.Message = Resource.EnterVaildEmail;
              return PartialView("_ResetPasswordPartial");
        }


        /*
         * Recharge user credit
         */
        [Authorize(Roles = "User")]
        public ActionResult RechargeCredit()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            CreditCardViewModel credit = new CreditCardViewModel();
            List<string> cardsList = new List<string>();
            cardsList.Add("Visa");
            cardsList.Add("Master Card");
            cardsList.Add("American Express");
            cardsList.Add("Discover Network");

            ViewBag.CardTypes = new SelectList(cardsList);

            List<int> months = new List<int>();
            for (int i = 01; i <= 12; i++)
                months.Add(i);

            List<int> years = new List<int>();
            for (int i = DateTime.Now.Year; i <= DateTime.Now.AddYears(10).Year; i++)
                years.Add(i);

            ViewBag.Month = new SelectList(months);
            ViewBag.Year = new SelectList(years);

            RechargeViewModel rvm = new RechargeViewModel()
            {
                CurrentCredit = user.Credit,
                CreditCard = credit
            };

            return View(rvm);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public PartialViewResult RechargeCredit(RechargeViewModel model)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
           
            try
            {
                /* 
                 * Payment Process
                 */

                if (model.NewCredit > 0)
                    user.Credit += model.NewCredit;
                db.SaveChanges();
                ViewBag.Message = Resource.CreditRechargeSuccess + user.Credit + " $" ;
                ViewBag.AlertSuccess = "alert alert-success";
                ViewBag.Success = true;
                return PartialView("_RechargeCreditPartial");

            }
            catch
            {

            }

            CreditCardViewModel credit = new CreditCardViewModel();
            List<string> cardsList = new List<string>();
            cardsList.Add("Visa");
            cardsList.Add("Master Card");
            cardsList.Add("American Express");
            cardsList.Add("Discover Network");

            ViewBag.CardTypes = new SelectList(cardsList);

            List<int> months = new List<int>();
            for (int i = 01; i <= 12; i++)
                months.Add(i);

            List<int> years = new List<int>();
            for (int i = DateTime.Now.Year; i <= DateTime.Now.AddYears(10).Year; i++)
                years.Add(i);

            ViewBag.Month = new SelectList(months);
            ViewBag.Year = new SelectList(years);

            RechargeViewModel rvm = new RechargeViewModel()
            {
                CurrentCredit = user.Credit,
                CreditCard = credit
            };

            return PartialView("_RechargeCreditPartial",rvm);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
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

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}