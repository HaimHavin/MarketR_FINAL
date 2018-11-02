#region Using

using MarketR.Reports;
using System;
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
        public ActionResult GetResultView(bool? NPV, string currency, int? band)
        {
            var result = report.GetFilterResultView(NPV, currency, band);
            return PartialView("_SimView", result);
        }
        [HttpPost]
        public JsonResult Simulate(string startDate, string currencyFormat, string shortValue, string longValue, string bandval)
        {
            string checkvalue = "";
            if (bandval != null)
            {

            }

            return Json(checkvalue, JsonRequestBehavior.AllowGet);
        }
    }
}