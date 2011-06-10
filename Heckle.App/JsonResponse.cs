using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Json;
using Nancy.Responses;

namespace Heckle.App
{
    public class JsonResponse : Response
    {
        public JsonResponse(object model)
        {
            this.Contents = GetJsonContents(model);
            this.ContentType = "application/json";
            this.StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetJsonContents(object model)
        {
            return stream =>
            {
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(model);

                var writer = new StreamWriter(stream);

                writer.Write(json);
                writer.Flush();
            };
        }
    }
}