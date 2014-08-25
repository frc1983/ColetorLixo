using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace ColetorLixo.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Content/Scripts/jquery-2.1.1.min.js")
                .Include("~/Content/Scripts/functions.js"));

            bundles.Add(new StyleBundle("~/Content/Css").Include("~/Content/Css/Style.css"));
        }
    }
}
