using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.Responses;
using TinyIoC;

namespace Heckle.App
{
    public class StaticContentProvider
    {
        private static readonly IDictionary<string, string> DefaultExtensions = new Dictionary<string, string>
                                                                                    {
                                                                                        { "jpg", "image/jpeg" },
                                                                                        { "png", "image/png" },
                                                                                        { "gif", "image/gif" },
                                                                                        { "css", "text/css" },
                                                                                        { "js", "text/javascript" }
                                                                                    };
        private readonly TinyIoCContainer _container;
        private readonly IDictionary<string, string> _fileExtensions;
        private readonly string[] _contentFolders;
        private readonly ConcurrentDictionary<string, Func<Response>> _knownResponses = new ConcurrentDictionary<string, Func<Response>>();

        public StaticContentProvider(TinyIoCContainer container) : this(container, "content")
        {
        }

        public StaticContentProvider(TinyIoCContainer container, params string[] contentFolders) : this(container, DefaultExtensions, contentFolders)
        {
        }

        public StaticContentProvider(TinyIoCContainer container, IDictionary<string, string> fileExtensions, params string[] contentFolders)
        {
            _container = container;
            _fileExtensions = fileExtensions;
            _contentFolders = contentFolders;
        }

        public Response Get(string uri)
        {
            var func = _knownResponses.GetOrAdd(uri, BuildContentFunc);
            return func();
        }

        private Func<Response> BuildContentFunc(string uri)
        {
            var rootPathProvider = _container.Resolve<IRootPathProvider>();
            var requestedExtension = Path.GetExtension(uri);
            if (!string.IsNullOrEmpty(requestedExtension))
            {
                var extensionWithoutDot =
                    requestedExtension.Substring(1);

                if (_fileExtensions.Keys.Any(x => x.Equals(extensionWithoutDot, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var fileName =
                        Path.GetFileName(uri);

                    if (fileName == null)
                    {
                        return null;
                    }

                    foreach (var contentFolder in _contentFolders)
                    {
                        var filePath =
                            Path.Combine(rootPathProvider.GetRootPath(), contentFolder, fileName);

                        if (File.Exists(filePath))
                        {
                            return () => new GenericFileResponse(filePath, _fileExtensions[extensionWithoutDot]);
                        }
                    }
                }
            }

            return () => null;
        }
    }
}