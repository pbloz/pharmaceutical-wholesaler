﻿using Hurtownia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Hurtownia
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

           // HtmlHelper.ClientValidationEnabled = true;
            //HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
        }
    }
}
