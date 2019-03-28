using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelAndTourismWebsite.Models;
using Microsoft.AspNet.Identity;
using TravelAndTourismWebsite.WebsiteLanguages;
using System.IO;

namespace TravelAndTourismWebsite.Controllers
{
    public class BaseController : Controller
    {
        //Setting website language
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string language = null;
            HttpCookie LanguageCookie = Request.Cookies["culture"];
            if (LanguageCookie != null)
            {
                language = LanguageCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                if (userLang != "")
                {
                    language = userLang;
                }
                else
                {
                    language = SiteLanguages.GetDefaultLanguage();
                }
            }

            new SiteLanguages().SetLanguage(language);

            return base.BeginExecuteCore(callback, state);
        }

        //Changing website language
        public ActionResult ChangeLanguage(string lang)
        {
            new SiteLanguages().SetLanguage(lang);
            ApplicationDbContext db = new ApplicationDbContext();

            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
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
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }
        [ChildActionOnly]
        protected bool checkFileType(string fileName)
        {
            string fileExtention = Path.GetExtension(fileName);

            switch (fileExtention.ToLower())
            {
                case ".png":
                    return true;
                case ".jpg":
                    return true;
                case ".jpeg":
                    return true;
                default:
                    return false;
            }
        }
        
	}
}