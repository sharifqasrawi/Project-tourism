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

namespace TravelAndTourismWebsite.Repos
{
    public class AuthRepository : IDisposable
    {
        private ApplicationDbContext _ctx;
        private UserManager<ApplicationUser> _userManager;
        ApplicationUser usera;
        string uid;

        public AuthRepository()
        {
            _ctx = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(RegisterViewModel userModel)
        {

            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email

            };

            var result = new IdentityResult();

            if (_ctx.Users.Where(e => e.Email == user.Email).Any())
            {
                result = IdentityResult.Failed("Email Is allready Exist!! try with other email..");
                return result;

            }
            usera = new ApplicationUser()
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Gender = userModel.Gender,
                Email = userModel.Email,
                PhoneNumber = userModel.PhoneNumber,
                Country = userModel.Country,
                City = userModel.City,
                UserName = userModel.UserName,
                Active = true,
                LoginCount = 0,
                RegistrationDateTime = DateTime.Now
            };


            

            result = await _userManager.CreateAsync(usera, userModel.Password);
            var roleStore = new RoleStore<IdentityRole>(_ctx);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<ApplicationUser>(_ctx);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!roleManager.RoleExists("User"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }
            if (result.Succeeded)
            {
                userManager.AddToRole(usera.Id, "User");
                uid = usera.Id;
                updatemodel(usera);
                return result;

            }

            return result;
        }

        public void updatemodel(ApplicationUser u)
        {
            var t = _ctx.Users.Find(uid);
            t.FirstName = u.FirstName;
            t.LastName = u.LastName;
            t.Gender = u.Gender;
            t.Email = u.Email;
            t.PhoneNumber = u.PhoneNumber;
            t.RegistrationDateTime = u.RegistrationDateTime;
            t.Country = u.Country;
            t.City = u.City;
            t.Credit = u.Credit;

            _ctx.SaveChanges();

        }


        public async Task<IdentityResult> UpdateUser(ApiModelsView.UpdateAccountViewModel u)
        {
            var t = _ctx.Users.Find(u.UserId);

            if (u.FirstName != null)
            {
                t.FirstName = u.FirstName;
            }
            if (u.LastName != null)
            {
                t.LastName = u.LastName;
            }
            if (u.Gender != null)
            {
                t.Gender = u.Gender;
            }
            if (u.Email != null)
            {
                t.Email = u.Email;
            }
            if (u.PhoneNumber != null)
            {
                t.PhoneNumber = u.PhoneNumber;
            }
            if (u.Country != null)
            {
                t.Country = u.Country;
            }
            if (u.City != null)
            {
                t.City = u.City;
            }

            var result = await _userManager.UpdateAsync(t);
            if (result.Succeeded)
            {
                _ctx.SaveChanges();
                return result;


            }
            result = IdentityResult.Failed("Update Failed, please make sure in your information and try again..");
            return result;

        }
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            ApplicationUser CurrentUser = _ctx.Users.Find(user.Id);
            CurrentUser.LoginCount++;
            CurrentUser.LastLoginDateTime = DateTime.Now;
            _ctx.SaveChanges();
            return user;
        }
        public async Task<IdentityResult> ChangeMyPassword(ApiModelsView.ChangePasswordViewModel model)
        {
            //  ApplicationUser user = _ctx.Users.SingleOrDefault(x => x.Email == model.Email);
            ApplicationUser user = _ctx.Users.Find(model.UserId);
            IdentityResult result;
            if ((user != null) && (model.CurrentPassword != null) && (model.NewPassword != null))
            {
                result = await _userManager.ChangePasswordAsync(user.Id, model.CurrentPassword, model.NewPassword);
                return result;
            }
            result = IdentityResult.Failed("New password or current password can not be empty!!");
            return result;
        }
        public Boolean ResetMyPassword(ApiModelsView.ResetPasswordViewModel model) 
        {

            ApplicationUser user = _ctx.Users.SingleOrDefault(x => x.Email == model.Email);
            bool result1 = false;
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
                string newPasswordHashed = _userManager.PasswordHasher.HashPassword(newPassword);
                user.PasswordHash = newPasswordHashed;
                _ctx.SaveChanges();
                //Sending the new password to user by email
                string subject = "", body = "";
                switch (user.PreferredInterfaceLanguage)
                {
                    case "English":
                        subject = "Travel And Tourism - Password Changed!";
                        body = "Dear User: " + user.UserName + ". \n" +
                               "Your new password is:\n [ " + newPassword + " ] \n" +
                               "for a safer account, please change it when you login to your account.\n" +
                               "\n\n\n Travel And Tourism team.";
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
                               "\n\n\n Travel And Tourism team.";
                        break;
                }

                Email email = new Email(model.Email, subject, body);

                email.Send();

                result1 = true;
                return result1;
            }
            else
            {
                result1 = false;
                return result1;
            }

          
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }


}