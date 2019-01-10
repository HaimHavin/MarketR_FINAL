#region Using

using MarketR.Models;
using MarketR.Models.Condor;
using MarketR.Reports;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

#endregion

namespace MarketR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        IDashboardReports report;
        public HomeController(IDashboardReports report1)
        {
            this.report = report1;
        }
        // GET: home/index
        public ActionResult Index()
        {
            //Session["user"] = "demo@email.com";
            return View();
        }

        public ActionResult Social()
        {
            return View();
        }

        // GET: home/inbox
        public ActionResult Inbox()
        {
            return View();
        }

        // GET: home/widgets
        public ActionResult Widgets()
        {
            return View();
        }

        // GET: home/chat
        public ActionResult Chat()
        {
            return View();
        }
        [HttpPost]
        public JsonResult CalculateCurrency(string startDate, string currencyFormat)
        {
            report.PerformCalculation(startDate, currencyFormat);
            var report1 = report.GetReport1(startDate, currencyFormat);
            var report2 = report.GetReport2(startDate, currencyFormat);
            return Json(new { report1, report2 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetResultView(bool? NPV, string currency, int? band, string FilterText)
        {
            var result = report.GetFilterResultView(NPV, currency, band, FilterText);
            return PartialView("_SimView", result);
        }
        [HttpPost]
        public JsonResult Simulate(SimulateModel model)
        {
            if (model != null && model.SimViewChanges.Count > 0)
                report.UpdateSimLiquidate(model);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Analytics()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetAnalyticsFileData(DateTime date)
        {
            MarketREntities marketREntities = new MarketREntities();
            SqlParameter dateParam = new SqlParameter();
            dateParam.ParameterName = "@date";
            dateParam.Value = date;
            dateParam.SqlDbType = System.Data.SqlDbType.DateTime;
            var result= marketREntities.Database.SqlQuery<FileHistory>("GetAnalyticsFileData @date",
                dateParam).FirstOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}