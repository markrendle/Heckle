using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace Heckle.App
{
    public class StaticContentModule : NancyModule
    {
        public static StaticContentProvider Provider;
        public StaticContentModule() : base("/content")
        {
            Get[".*"] = req =>
                           {
                               return Provider.Get(Request.Uri);
                           };
        }
    }
}