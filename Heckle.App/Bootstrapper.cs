﻿using System.Collections.Generic;
using System.Web;
using Nancy;

namespace Heckle.App
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);

            var staticContentHandler = new StaticContentProvider(container);

            BeforeRequest += ctx => staticContentHandler.Get(ctx.Request.Uri);
        }
    }
}