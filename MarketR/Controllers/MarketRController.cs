using MarketR.ViewModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MarketR.Common.Models.Condor;
using MarketR.Common.Service;
using MarketR.Common.Service.CsvParser;
using MarketR.Common.Service.CsvParser.Exceptions;
using MarketR.Common.Service.CsvParser.Models;
using MarketR.Common.Service.CsvValidator;
using MarketR.Common;
using MarketR.Utilities;
using MarketR.Common.Repository;
using MarketR.Common.Models;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Reflection;
using OfficeOpenXml.Table;
using ExcelDataReader;
using MarketR.DAL.Models;

namespace MarketR.Controllers
{
    public class MarketRController : Controller
    {
        EventLog eventLog = new EventLog();
        private readonly ICsvParseService<DAL.Models.Condor.KONDOR_DATA> csvParseService;
        private readonly ICsvValidateService csvValidateService;
        //IRepository repository = new Repository(new MyFirstDbContex);
        IMarketRRepo marketRRepo = new MarketRRepo(new MarketREntities());
        AnalyticsRepo analyticsRepo = new AnalyticsRepo();

        public MarketRController()
        {
            csvParseService = new CsvParseService<DAL.Models.Condor.KONDOR_DATA>(TypeConverter.GetConvertSettings());
            csvValidateService = new CsvValidateService();
        }
        // GET: MarketR
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public ActionResult FileManager(HttpPostedFileBase files)
        {
            //   System.Threading.Thread.Sleep(4000);
            if (files != null && files.ContentLength > 0)
            {
                string ext = Path.GetExtension(files.FileName);

                if (ext != ".csv")
                {
                    return new JsonResult { Data = "Please select only .csv file" };
                }
                string fileName = System.IO.Path.GetFileName(files.FileName);
                string folder = Server.MapPath("~/files/");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string folderLog = Server.MapPath("~/Logfiles");
                if (!Directory.Exists(folderLog))
                {
                    Directory.CreateDirectory(folderLog);
                }
                string strDate = DateTime.Now.ToString("yyMMddhhmmss");
                fileName = strDate + "_" + fileName;
                string fullPath = Path.Combine(folder, fileName);
                files.SaveAs(fullPath);
                try
                {
                    MarketRModel.SaveFileName(fullPath);
                }
                catch (Exception ex)
                {
                    StringBuilder logDataRow = new StringBuilder();
                    string fullPathLog = Path.Combine(folderLog, fileName);
                    try
                    {
                        using (StreamReader sr = new StreamReader(fullPathLog.Replace(".csv", ".log.Error.Txt")))
                        {
                            string line;
                            int count = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                var rx = new System.Text.RegularExpressions.Regex("File Offset");
                                var array = rx.Split(line);
                                if (count == 0)
                                {
                                    logDataRow.Append("        " + array[0]);
                                }
                                else
                                {
                                    logDataRow.Append(",        " + array[0]);
                                }
                                count++;
                            }
                        }
                    }
                    catch (Exception exe)
                    {
                        return new JsonResult { Data = "There are some problem<br>" + exe.Message };
                    }
                    return new JsonResult { Data = "There are some problem below row <br>" + logDataRow };
                }
            }
            else
            {
                return new JsonResult { Data = "Please select file" };
            }
            return new JsonResult { Data = "Uploaded successfully" };
        }

        [Obsolete]
        //public ActionResult Upload(string user, string pass)
        public ActionResult Upload(string user)
        {
            ViewBag.User = Session["UserData"];
            return View();
        }

        [HttpPost]
        public JsonResult FileManagerUpload(HttpPostedFileBase files)
        {
            DateTime fileDate;
            var splitFileName = files.FileName.Split('_').ToList();
            if (splitFileName != null && splitFileName.Count > 1)
            {
                var fileDateStr = splitFileName.ElementAt(1).Substring(0, splitFileName.ElementAt(1).IndexOf('.')).Replace("-", "") + "00";
                string format = "yyyyMMddHHmmss";
                fileDate = DateTime.ParseExact(fileDateStr, format, CultureInfo.InvariantCulture);
                if (fileDate == DateTime.MinValue)
                {
                    return Json(new { Success = false, Message = "Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.xlsx" });
                }
            }
            else
            {
                return Json(new { Success = false, Message = "Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.xlsx" });
            }
            var calculatedData = new List<FileCalculationViewModel>();
            if (ModelState.IsValid)
            {
                if (files == null)
                {
                    return Json(new { Success = false, Message = "Please, choose CSV file" });
                }

                if (csvValidateService.FileIsEmpty(files.InputStream))
                {
                    eventLog.SaveEventLog(ConstantEvent.EmptyCsvFile, ConstantEvent.Failed);
                    return Json(new { Success = false, Message = "CSV file is empty" });
                }
                if (!csvValidateService.IsFileCsvXls(files.FileName))
                {
                    eventLog.SaveEventLog(ConstantEvent.InvalidFileType, ConstantEvent.Failed);
                    return Json(new { Success = false, Message = "Please, upload CSV file" });
                }
                try
                {
                    ParseResult<DAL.Models.Condor.KONDOR_DATA> KONDOR_DATAcsv = csvParseService.ParseData(files.InputStream, ",");
                    if (!KONDOR_DATAcsv.IsValid)
                    {
                        var builder = new StringBuilder("There are some problem \n");

                        foreach (var error in KONDOR_DATAcsv.Errors)
                        {
                            foreach (var subError in error.Value)
                            {
                                builder.Append(string.Format("{0}\n", subError));
                            }
                        }
                        eventLog.SaveEventLog(ConstantEvent.ErrorCsvParsing, ConstantEvent.Failed);
                        return Json(new { Success = false, Message = builder.ToString() });
                    }
                    #region Saving csv file and doing calculation
                    //saving file and it's path

                    var setting = analyticsRepo.GetImportSetting(fileType.Csv);
                    if (setting != null && !string.IsNullOrWhiteSpace(setting.FileSavePath))
                    {                       
                        string targetPath = Path.Combine(setting.FileSavePath, files.FileName);
                        files.SaveAs(targetPath);

                        KondorFileHistory fileHistory = new KondorFileHistory();
                        fileHistory.FileName = files.FileName;
                        fileHistory.FilePath = targetPath;
                        fileHistory.CreatedDate = DateTime.Now;
                        fileHistory.FileDate = fileDate;
                        marketRRepo.Add<KondorFileHistory>(fileHistory);
                        marketRRepo.UnitOfWork.SaveChanges();

                        var calculation = marketRRepo.Find<FileCalculation>(x => x.FileID == fileHistory.FileID).ToList();
                        calculatedData = AutoMapper.Mapper.Map<List<FileCalculation>, List<FileCalculationViewModel>>(calculation);
                    }
                    #endregion
                }
                catch (CsvParseException ex)
                {
                    eventLog.SaveEventLog(ConstantEvent.CsvParseExceptionWhileUploading, ConstantEvent.Failed);
                    return Json(new { Success = false, Message = "Could not read file!" });
                }
                catch (Exception ex)
                {
                    eventLog.SaveEventLog(ConstantEvent.ExceptionWhileUploading, ConstantEvent.Failed);
                    var t = ex.GetType();
                    //todo:must be logger
                    return Json(new { Success = false, Message = "Could not read file!" });
                }
            }
            eventLog.SaveEventLog(ConstantEvent.FileUploaded, ConstantEvent.Successful);
            //return Json(new { Success = true, Message = "Uploaded successfully" });
            //return Json(calculatedData, JsonRequestBehavior.AllowGet);
            return Json(new { calculatedData = calculatedData, Success = true }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// uploading the excel files.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        //public JsonResult MatchXlsColum(HttpPostedFileBase files)
        [HttpPost]
        public JsonResult FileManagerUploadExcel(HttpPostedFileBase files)
        {
            var calculatedData = new List<FileCalculationViewModel>();
            try
            {
                DateTime fileDate;
                var splitFileName = files.FileName.Split('_').ToList();
                if (splitFileName != null && splitFileName.Count > 1)
                {
                    string fileDateStr = String.Join("", splitFileName.ElementAt(0).Where(char.IsDigit)) + "00";
                    string format = "yyyyMMddHH";
                    fileDate = DateTime.ParseExact(fileDateStr, format, CultureInfo.InvariantCulture);
                    if (fileDate == DateTime.MinValue)
                    {
                        return Json(new { Success = false, Message = "Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.xlsx" });
                    }
                }
                else
                {
                    return Json(new { Success = false, Message = "Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.xlsx" });
                }
                var setting = analyticsRepo.GetImportSetting(fileType.Excel);
                if (setting != null && !string.IsNullOrWhiteSpace(setting.FileSavePath))
                {
                    if (!Directory.Exists(setting.FileSavePath)) Directory.CreateDirectory(setting.FileSavePath);
                    string targetPath = Path.Combine(setting.FileSavePath, files.FileName);
                    files.SaveAs(targetPath);

                    //Read excel file 
                    FileHistory fileHistory = new FileHistory();
                    fileHistory.FileName = files.FileName;
                    fileHistory.FilePath = targetPath;
                    fileHistory.CreatedDate = DateTime.Now;
                    fileHistory.FileDate = fileDate;
                    marketRRepo.Add<FileHistory>(fileHistory);
                    marketRRepo.UnitOfWork.SaveChanges();

                    var calculation = marketRRepo.Find<FileCalculation>(x => x.FileID == fileHistory.FileID).ToList();
                    calculatedData = AutoMapper.Mapper.Map<List<FileCalculation>, List<FileCalculationViewModel>>(calculation);
                }
                else
                {
                    return Json(new { Success = false, Message = "Import setting is not configured properly!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Could not read file!" });
            }
            return Json(new { calculatedData = calculatedData, Success = true }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult FileManagerUpload_Old(HttpPostedFileBase files)
        //{
        //    var calculatedData = new List<FileCalculationViewModel>();
        //    string fileExtension = Path.GetExtension(files.FileName).ToLower().ToString();
        //    if (fileExtension == ".xlsx" || fileExtension == ".xls")
        //    {
        //        return MatchXlsColum(files);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        if (files == null)
        //        {
        //            return Json(new { Success = false, Message = "Please, choose CSV file" });
        //        }

        //        if (csvValidateService.FileIsEmpty(files.InputStream))
        //        {
        //            eventLog.SaveEventLog(ConstantEvent.EmptyCsvFile, ConstantEvent.Failed);
        //            return Json(new { Success = false, Message = "CSV file is empty" });
        //        }
        //        if (!csvValidateService.IsFileCsvXls(files.FileName))
        //        {
        //            eventLog.SaveEventLog(ConstantEvent.InvalidFileType, ConstantEvent.Failed);
        //            return Json(new { Success = false, Message = "Please, upload CSV file" });
        //        }
        //        try
        //        {
        //            ParseResult<CondorDtoNew> condorDtoNew = csvParseService.ParseData(files.InputStream, ",");

        //            if (!condorDtoNew.IsValid)
        //            {
        //                var builder = new StringBuilder("There are some problem \n");

        //                foreach (var error in condorDtoNew.Errors)
        //                {
        //                    foreach (var subError in error.Value)
        //                    {
        //                        builder.Append(string.Format("{0}\n", subError));
        //                    }
        //                }
        //                eventLog.SaveEventLog(ConstantEvent.ErrorCsvParsing, ConstantEvent.Failed);
        //                return Json(new { Success = false, Message = builder.ToString() });
        //            }
        //            #region Saving csv file and doing calculation
        //            //saving file and it's path
        //            string targetFolder = Server.MapPath("~/Content/csv file");
        //            string targetPath = Path.Combine(targetFolder, files.FileName);
        //            files.SaveAs(targetPath);

        //            FileHistory fileHistory = new FileHistory();
        //            fileHistory.FileName = files.FileName;
        //            fileHistory.FilePath = targetPath;
        //            fileHistory.CreatedDate = DateTime.Now;
        //            marketRRepo.Add<FileHistory>(fileHistory);
        //            marketRRepo.UnitOfWork.SaveChanges();
        //            // Saving file data to database.
        //            //var aa = AutoMapper.Mapper.Map<ParseResult<CondorDtoNew>, ParseResult<FileRecord>>(condorDtoNew);
        //            foreach (var record in condorDtoNew.Records)
        //            {
        //                var fileRecord = AutoMapper.Mapper.Map<CondorDtoNew, FileRecord>(record);
        //                fileRecord.FileID = fileHistory.FileID;
        //                marketRRepo.Add<FileRecord>(fileRecord);
        //            }
        //            marketRRepo.UnitOfWork.SaveChanges();

        //            var calculation = marketRRepo.Find<FileCalculation>(x => x.FileID == fileHistory.FileID).ToList();
        //            calculatedData = AutoMapper.Mapper.Map<List<FileCalculation>, List<FileCalculationViewModel>>(calculation);
        //            #endregion
        //        }
        //        catch (CsvParseException ex)
        //        {
        //            eventLog.SaveEventLog(ConstantEvent.CsvParseExceptionWhileUploading, ConstantEvent.Failed);
        //            return Json(new { Success = false, Message = "Could not read file!" });
        //        }
        //        catch (Exception ex)
        //        {
        //            eventLog.SaveEventLog(ConstantEvent.ExceptionWhileUploading, ConstantEvent.Failed);
        //            var t = ex.GetType();
        //            //todo:must be logger
        //            return Json(new { Success = false, Message = "Could not read file!" });
        //        }
        //    }
        //    eventLog.SaveEventLog(ConstantEvent.FileUploaded, ConstantEvent.Successful);
        //    //return Json(new { Success = true, Message = "Uploaded successfully" });
        //    //return Json(calculatedData, JsonRequestBehavior.AllowGet);
        //    return Json(new { calculatedData = calculatedData, Success = true }, JsonRequestBehavior.AllowGet);
        //}

        //[Obsolete]
        //[HttpPost]
        //public ActionResult FileManagerUpload(HttpPostedFileBase files)
        //{

        //    System.Threading.Thread.Sleep(3000);
        //    string result = "";

        //    if (files != null && files.ContentLength > 0)
        //    {
        //        string ext = Path.GetExtension(files.FileName);

        //        if (ext != ".csv")
        //        {
        //            return new JsonResult { Data = "Please select only .csv file" };
        //        }

        //        string fileName = System.IO.Path.GetFileName(files.FileName);
        //        string folder = Server.MapPath("~/files/");
        //        if (!Directory.Exists(folder))
        //        {
        //            Directory.CreateDirectory(folder);
        //        }
        //        string folderLog = Server.MapPath("~/Logfiles");
        //        if (!Directory.Exists(folderLog))
        //        {
        //            Directory.CreateDirectory(folderLog);
        //        }
        //        string strDate = DateTime.Now.ToString("yyMMddhhmmss");
        //        fileName = strDate + "_" + fileName;
        //        string fullPath = Path.Combine(folder, fileName);
        //        files.SaveAs(fullPath);
        //        try
        //        {
        //            string fullPathLog = Path.Combine(folderLog, fileName);
        //            fullPathLog = fullPathLog.Replace(".csv", ".txt");
        //            DataTable dt = ConvertCSVtoDataTable(fullPath);
        //            CondorModel condorModel = new Models.CondorModel();
        //            result = condorModel.AddCondorData(dt, fullPathLog);
        //        }
        //        catch (Exception ex)
        //        {
        //            return new JsonResult { Data = "There are some problem<br>" + ex.Message };

        //        }
        //    }
        //    else
        //    {
        //        return new JsonResult { Data = "Please select file" };
        //    }

        //    return new JsonResult { Data = result };
        //}

        [Obsolete]
        public DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        //if (i == 8 || i == 10)
                        //{
                        //    if (rows[i] != null && rows[i].ToString() != "")
                        //    {
                        //        dr[i] = FormatDateddmmyyyytommddyyyy(rows[i]);
                        //    }
                        //}
                        //else
                        {
                            dr[i] = rows[i];
                        }

                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        [Obsolete]
        public string FormatDateddmmyyyytommddyyyy(string str)
        {
            if (str != null && str != string.Empty)
            {
                DateTime startDate;
                string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy",
                    "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy"};

                if (DateTime.TryParseExact(str, formats, null, DateTimeStyles.None, out startDate))
                {
                    return startDate.ToString("MM/dd/yyyy");
                }
                else
                {
                    return str;
                }
            }
            return string.Empty;
        }


        private void SaveCSVFile()
        {

        }

        [HttpPost]
        public JsonResult SaveCalculation(List<FileCalculationViewModel> fileCalculationViewModel)
        {
            foreach (var record in fileCalculationViewModel)
            {
                var fileRecord = AutoMapper.Mapper.Map<FileCalculation>(record);
                marketRRepo.Add<FileCalculation>(fileRecord);
            }
            marketRRepo.UnitOfWork.SaveChanges();
            return Json(new { Success = true, Message = "Calculation Saved" });
        }
    }
}