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

            Get["/feedback/{Event}/{Slot}/{Track}"] = req =>
            {
                var since = ((string)Request.Query.since).Replace(" UTC", string.Empty);
                var sinceDate = DateTime.Parse(since, CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal);
                var session = _db.Sessions.FindByEventCodeAndSlotAndTrack((string)req.Event,
                                                                      (int)req.Slot,
                                                                      (int)req.Track);
                if (session == null) return HttpStatusCode.NotFound;
                var feedbacks = session.Feedback.Where(_db.Feedback.Time > sinceDate).OrderByTimeDescending().ToList();
                return new JsonResponse(feedbacks);
            };

            Get["/{Event}/{Slot}/{Track}"] = req =>
                                                 {
                                                     var session = _db.Sessions.FindByEventCodeAndSlotAndTrack((string)req.Event,
                                                                                                           (int)req.Slot,
                                                                                                           (int)req.Track);
                                                     if (session == null) return HttpStatusCode.NotFound;
                                                     return View["session", session];
                                                 };


            Post["/{Event}/{Slot}/{Track}"] = req =>
                                                  {
                                                      var session =
                                                          _db.Sessions.FindByEventCodeAndSlotAndTrack(
                                                              (string) req.Event,
                                                              (int) req.Slot,
                                                              (int) req.Track);
                                                      if (session == null) return HttpStatusCode.NotFound;

                                                      _db.Feedback.Insert(SessionId: session.Id,
                                                                          Comment: Request.Form.Comment.Value,
                                                                          Mood: Request.Form.Mood.Value ?? "Smile",
                                                                          Commenter: Request.Form.Commenter.Value
                                                          );
                                                      return new RedirectResponse(Request.Uri);
                                                  };
        }
    }
}