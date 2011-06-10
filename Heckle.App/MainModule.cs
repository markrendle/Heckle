using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Responses;
using Simple.Data;

namespace Heckle.App
{
    public class MainModule : NancyModule
    {
        private readonly dynamic _db = Database.OpenNamedConnection("appharbor");
        public MainModule()
        {
            Get["/"] = _ =>
                           {
                               dynamic model = new ExpandoObject();
                               model.Sessions = _db.Sessions.All().OrderBySlot().ThenByTrack();
                               model.SplitRow = new HtmlString("</tr><tr>");
                               return View["index", model];
                           };

            Get["/{Event}/{Slot}/{Track}/feedback"] =
                req =>
                    {
                        DateTime sinceDate = Request.Query.since != null
                                                 ? ParseJavaScriptUtcDate(Request.Query.since)
                                                 : new DateTime(1900, 1, 1);

                        var feedbacks = _db.Sessions.QueryByEventCodeAndSlotAndTrack(req.Event, req.Slot, req.Track)
                            .Feedback
                            .Where(_db.Feedback.Time > sinceDate)
                            .OrderByTimeDescending();

                        return new JsonResponse<object>(feedbacks);
                    };

            Get["/{Event}/{Slot}/{Track}"] =
                req =>
                    {
                        var session = _db.Sessions.FindByEventCodeAndSlotAndTrack(req.Event, req.Slot, req.Track);
                        if (session == null) return HttpStatusCode.NotFound;
                        return View["session", session];
                    };

            Post["/{Event}/{Slot}/{Track}/feedback"] =
                req =>
                    {
                        var session = _db.Sessions.FindByEventCodeAndSlotAndTrack(req.Event, req.Slot, req.Track);
                        if (session == null) return HttpStatusCode.NotFound;

                        if (Request.Form.Comment != null)
                        {
                            string comment = Request.Form.Comment;
                            string commenter = string.IsNullOrWhiteSpace(Request.Form.Commenter) ? "Anonymous" : Request.Form.Commenter;
                            string mood = Request.Form.Mood == null ? "Happy" : Request.Form.Mood;

                            _db.Feedback.Insert(SessionId: session.Id,
                                                Comment: comment,
                                                Mood: mood ?? "Happy",
                                                Commenter: commenter
                                );
                        }
                        return new RedirectResponse(Request.Uri.Replace("/feedback", "/"));
                    };

        }

        private static DateTime ParseJavaScriptUtcDate(string since)
        {
            since = since.Replace(" UTC", string.Empty);
            return DateTime.Parse(since, CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal);
        }
    }
}