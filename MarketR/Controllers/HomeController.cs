#region Using

using MarketR.Common;
using MarketR.Common.Models.Condor;
using MarketR.Common.Reports;
using MarketR.Common.Repository;
using MarketR.DAL.ExcelParser;
using System;
using System.Collections.Generic;
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
        public JsonResult CalculateCurrency(string startDate, string currencyFormat, int fileId)
        {
            try
            {
                new Handler(fileId, fileType.Excel, fileVersion.Version1);
                report.PerformCalculation(startDate, currencyFormat, fileId);
                var report1 = report.GetReport1(startDate, currencyFormat);
                var report2 = report.GetReport2(startDate, currencyFormat);
                return Json(new { Success = true, report1, report2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : "") }, JsonRequestBehavior.AllowGet);
            }
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
            AnalyticsRepo repository = new AnalyticsRepo();
            var result = repository.GetAnalyticsData(date);
            if (result != null) return Json(result, JsonRequestBehavior.AllowGet);
            else return Json("", JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetFileVersions()
        {
            AnalyticsRepo repository = new AnalyticsRepo();
            var result = repository.GetFileVersions();
            if (result != null && result.Count > 0)
            {
                var newData = result.GroupBy(
                    k => k.Text,
                    g => new { g.Value },
                    (key, g) => new { Text = key, ids = g.ToArray() }
                    );
                var selectList = new List<DAL.Repository.SelectList>();
                foreach (var item in newData)
                {
                    if (item.ids.Count() > 0)
                    {
                        var selectListItem = new DAL.Repository.SelectList();
                        selectListItem.Text = item.Text;
                        selectListItem.Value = item.ids.Select(n => n.Value).Max();
                        selectList.Add(selectListItem);
                    }
                }
                return Json(selectList, JsonRequestBehavior.AllowGet);
            }
            else return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Compare()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CalculateCompare(string startDate, string currencyFormat, int Version1Id, int Version2Id)
        {
            try
            {
                new Handler(Version1Id, fileType.Excel, fileVersion.Version1);
                new Handler(Version2Id, fileType.Excel, fileVersion.Version2);
                report.PerformCompareCalculation(currencyFormat, Version1Id, Version2Id);
                var version1 = report.GetVersion1();
                var version2 = report.GetVersion2();
                var version3 = report.GetVersion3();
                return Json(new { Success = true, version1, version2, version3 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}