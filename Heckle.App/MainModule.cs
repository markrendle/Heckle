using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Responses;
using Simple.Data;

namespace Heckle.App
{
    public class MainModule : NancyModule
    {
        private readonly dynamic _db = Database.OpenNamedConnection("local");
        public MainModule()
        {
            Get["/"] = _ => View["index"];

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
                                                      return
                                                          new RedirectResponse(Request.Uri);
                                                              //string.Format("/{0}/{1}/{2}",
                                                              //              session.EventCode, session.Slot,
                                                              //              session.Track));
                                                  };
        }
    }
}