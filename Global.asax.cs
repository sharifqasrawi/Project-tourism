﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TravelAndTourismWebsite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //      Application.Add("OnlineVisitors", 0);
            Application.Add("TotalVisitors", 0);

        }

        void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            //   Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] + 1;
            Application["TotalVisitors"] = (int)Application["TotalVisitors"] + 1;
            Application.UnLock();


        }

        void Session_End(object sender, EventArgs e)
        {
            //  Application.Lock();
            //  Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] - 1;
            //   Application.UnLock();

        }
    }
}
