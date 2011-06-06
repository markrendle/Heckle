using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Nancy;
using Simple.Data;

namespace Heckle.App
{
    public class MainModule : NancyModule
    {
        private readonly dynamic _db = Database.OpenNamedConnection("local");
        public MainModule()
        {
            Get["/"] = _ => View["index"];

            Get["/session/{Event}/{Slot}/{Track}"] = req =>
                                                 {
                                                     var session = _db.Sessions.FindByEventCodeAndSlotAndTrack((string)req.Event,
                                                                                                           (int)req.Slot,
                                                                                                           (int)req.Track);
                                                     return View["session", session];
                                                 };
        }
    }
}