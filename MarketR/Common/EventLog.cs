using MarketR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketR.Common
{
    public class EventLog
    {
        MarketREntities context = new MarketREntities();

        public void SaveEventLog(string eventtoLog, string status)
        {
            string userId = "";
            if (HttpContext.Current.Session["UserId"] != null)
            {
                userId = HttpContext.Current.Session["UserId"].ToString();
            }

            tblEventLog eventLog = new tblEventLog()
            {
                UserId = userId,
                Event = eventtoLog,
                Status = status,
                LoggedDate = DateTime.Now
            };
            context.tblEventLogs.Add(eventLog);
            context.SaveChanges();
        }
    }
}