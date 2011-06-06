using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Responses;

namespace Heckle.App
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void InitialiseInternal(TinyIoC.TinyIoCContainer container)
        {
            base.InitialiseInternal(container);
            BeforeRequest += ctx =>
            {
                var rootPathProvider =
                    container.Resolve<IRootPathProvider>();

                var staticFileExtensions =
                    new Dictionary<string, string>
                    {
                        { "jpg", "image/jpeg" },
                        { "png", "image/png" },
                        { "gif", "image/gif" },
                        { "css", "text/css" },
                        { "js",  "text/javascript" }
                    };

                var requestedExtension =
                    Path.GetExtension(ctx.Request.Uri);

                if (!string.IsNullOrEmpty(requestedExtension))
                {
                    var extensionWithoutDot =
                        requestedExtension.Substring(1);

                    if (staticFileExtensions.Keys.Any(x => x.Equals(extensionWithoutDot, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var fileName =
                            Path.GetFileName(ctx.Request.Uri);

                        if (fileName == null)
                        {
                            return null;
                        }

                        var filePath =
                            Path.Combine(rootPathProvider.GetRootPath(), "content", fileName);

                        return !File.Exists(filePath) ? null : new GenericFileResponse(filePath, staticFileExtensions[extensionWithoutDot]);
                    }
                }

                return null;
            };
        }
    }
}