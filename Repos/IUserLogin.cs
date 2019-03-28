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
    internal interface IUserLogin
    {
        int LogIn(string username, string password);
    }

    public class UserLogin : IUserLogin
    {
        public int LogIn(string username, string password)
        {

            ApplicationDbContext context = new ApplicationDbContext();


            //var user = context.Users.FirstOrDefault(u => u.UserName == username && u.PasswordHash == password);
            //if (user == null) return 0;
            //return user.p_Id == null ? 0 : user.p_Id.Value;
            return 0;

        }
    }
}