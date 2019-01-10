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
using CsvHelper.TypeConversion;
using MarketR.Models.Condor;
using MarketR.Service;
using MarketR.Service.CsvParser;
using MarketR.Service.CsvParser.Exceptions;
using MarketR.Service.CsvParser.Models;
using MarketR.Service.CsvValidator;
using MarketR.Common;
using MarketR.Utilities;
using MarketR.Repository;
using MarketR.Models;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using CsvHelper;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Reflection;
using OfficeOpenXml.Table;

using ExcelDataReader;

namespace MarketR.Controllers
{
    public class MarketRController : Controller
    {
        EventLog eventLog = new EventLog();
        private readonly ICsvParseService<NewCondorDto> csvParseService;
        private readonly ICsvValidateService csvValidateService;
        //IRepository repository = new Repository(new MyFirstDbContex);
        IMarketRRepo marketRRepo = new MarketRRepo(new MarketREntities());

        public MarketRController()
        {
            csvParseService = new CsvParseService<NewCondorDto>(GetConvertSettings());
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
                    ParseResult<NewCondorDto> condorDtoNew = csvParseService.ParseData(files.InputStream, ",");

                    if (!condorDtoNew.IsValid)
                    {
                        var builder = new StringBuilder("There are some problem \n");

                        foreach (var error in condorDtoNew.Errors)
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
                    string targetFolder = Server.MapPath("~/Content/csv file");
                    string targetPath = Path.Combine(targetFolder, files.FileName);
                    files.SaveAs(targetPath);

                    FileHistory fileHistory = new FileHistory();
                    fileHistory.FileName = files.FileName;
                    fileHistory.FilePath = targetPath;
                    fileHistory.CreatedDate = DateTime.Now;
                    fileHistory.FileDate = fileDate;
                    marketRRepo.Add<FileHistory>(fileHistory);
                    marketRRepo.UnitOfWork.SaveChanges();
                    // Saving file data to database.
                    //var aa = AutoMapper.Mapper.Map<ParseResult<CondorDtoNew>, ParseResult<FileRecord>>(condorDtoNew);

                    IList<NewFileRecordsCSV> csvRecordList = new List<NewFileRecordsCSV>();
                    DateTime dateTime1 = DateTime.Now;
                    foreach (var record in condorDtoNew.Records)
                    {
                        var fileRecord = AutoMapper.Mapper.Map<NewCondorDto, NewFileRecordsCSV>(record);
                        fileRecord.FileID = fileHistory.FileID;
                        csvRecordList.Add(fileRecord);
                    }
                    marketRRepo.AddRange(csvRecordList);
                    marketRRepo.UnitOfWork.SaveChanges();

                    DateTime dateTime2 = DateTime.Now;

                    TimeSpan diff = dateTime2 - dateTime1;

                    var logData = $"Start Import- {dateTime1}. End Import - {dateTime2}. Total time {diff}. Total Record Import-{csvRecordList.Count()}";
                    var fileName = $"C://Temp/CSVlogfile{DateTime.Now.Ticks}.txt";
                    System.IO.File.WriteAllText(fileName, logData);

                    var calculation = marketRRepo.Find<FileCalculation>(x => x.FileID == fileHistory.FileID).ToList();
                    calculatedData = AutoMapper.Mapper.Map<List<FileCalculation>, List<FileCalculationViewModel>>(calculation);
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
                if(splitFileName != null && splitFileName.Count > 1)
                {
                    var fileDateStr = splitFileName.ElementAt(1).Substring(0,splitFileName.ElementAt(1).IndexOf('.')).Replace("-","")+"00";
                    string format = "yyyyMMddHHmmss";
                    fileDate = DateTime.ParseExact(fileDateStr, format, CultureInfo.InvariantCulture);
                    if(fileDate == DateTime.MinValue)
                    {
                        return Json(new { Success = false, Message = "Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.xlsx" });
                    }
                }
                else
                {
                    return Json(new { Success = false, Message = "Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.xlsx" });
                }

                string targetFolder = Server.MapPath("~/Content/csv file");
                string targetPath = Path.Combine(targetFolder, files.FileName);
                files.SaveAs(targetPath);
                List<string> columnNames = new List<string>();
                FileInfo newFile = new FileInfo(targetPath);
                IExcelDataReader reader = null;
                if (files.FileName.ToLower().EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(files.InputStream);
                }
                else if (files.FileName.ToLower().EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(files.InputStream);
                }
                else
                {
                    eventLog.SaveEventLog(ConstantEvent.InvalidFileType, ConstantEvent.Failed);
                    return Json(new { Success = false, Message = "Please, upload xls or xlsx file" });
                }

                DataSet result = reader.AsDataSet();
                reader.Close();
                if (result.Tables.Count > 0)
                {
                    var noOfCol = result.Tables[0].Columns.Count;
                    var noOfRow = result.Tables[0].Rows.Count;

                    if (noOfCol != 20) return Json(new { Success = false, Message = "Some of columns are missing in excel" });
                    
                    //Read excel file 
                    FileHistory fileHistory = new FileHistory();
                    fileHistory.FileName = files.FileName;
                    fileHistory.FilePath = targetPath;
                    fileHistory.CreatedDate = DateTime.Now;
                    fileHistory.FileDate = fileDate;
                    marketRRepo.Add<FileHistory>(fileHistory);
                    marketRRepo.UnitOfWork.SaveChanges();

                    IList<NewFileRecord> newList = new List<NewFileRecord>();
                    DateTime dateTime1 = DateTime.Now;
                    for (int row = 1; row <= noOfRow - 1; row++)
                    {
                        if (row == 1 || row == 2 || row == 3 || row == 4) continue;
                        NewFileRecord newRecord = new NewFileRecord();

                        newRecord.N = result.Tables[0].Rows[row][0] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][0]);
                        newRecord.DEAL_ID = result.Tables[0].Rows[row][1] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][1]);
                        newRecord.ON_OFF_BALANCE = result.Tables[0].Rows[row][2] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][2]);
                        newRecord.DEAL_TYPE = result.Tables[0].Rows[row][3] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][3]);
                        newRecord.PROD_TYPE = result.Tables[0].Rows[row][4] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][4]);
                        newRecord.PAY_RECIEVE = result.Tables[0].Rows[row][5] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][5]);
                        newRecord.CCY = result.Tables[0].Rows[row][6] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][6]);
                        newRecord.NOTIONAL = result.Tables[0].Rows[row][7] == null ? 0 : Convert.ToDouble(result.Tables[0].Rows[row][7]);
                        newRecord.MATURITY_DATE = result.Tables[0].Rows[row][8] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][8]);
                        newRecord.INTEREST_TYPE = result.Tables[0].Rows[row][9] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][9]);
                        newRecord.FIXING_DATE = result.Tables[0].Rows[row][10] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][10]);
                        newRecord.INT_CHANGE_FREQ = result.Tables[0].Rows[row][11] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][11]);
                        newRecord.INT_CHAGE_TERM = result.Tables[0].Rows[row][12] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][12]);
                        newRecord.INT_PRE = result.Tables[0].Rows[row][13] == null ? 0 : Convert.ToDouble(result.Tables[0].Rows[row][13]);
                        newRecord.NPV_DELTA_ILS = result.Tables[0].Rows[row][14] == null ? 0 : Convert.ToDouble(result.Tables[0].Rows[row][14]);
                        newRecord.NETED = result.Tables[0].Rows[row][15] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][15]);
                        newRecord.NETED_ID = result.Tables[0].Rows[row][16] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][16]);
                        newRecord.Portfolio = result.Tables[0].Rows[row][17] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][17]);
                        newRecord.NETTING_COUNTER = result.Tables[0].Rows[row][18] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][18]);
                        newRecord.CONTRACT_MAT_DATE = result.Tables[0].Rows[row][18] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][19]);
                        newRecord.validity_date = result.Tables[0].Rows[0][3] == null ? "" : Convert.ToString(result.Tables[0].Rows[0][3]);
                        newRecord.FileID = fileHistory.FileID;
                        newList.Add(newRecord);
                    }

                    marketRRepo.AddRange<NewFileRecord>(newList);
                    marketRRepo.UnitOfWork.SaveChanges();
                    DateTime dateTime2 = DateTime.Now;
                    TimeSpan diff = dateTime2 - dateTime1;

                    var logData = $"Start Import- {dateTime1}. End Import - {dateTime2}. Total time {diff}. Total Record Import-{newList.Count()}";
                    var fileName = $"C://Temp/Excellogfile{DateTime.Now.Ticks}.txt";
                    System.IO.File.WriteAllText(fileName, logData);

                    var calculation = marketRRepo.Find<FileCalculation>(x => x.FileID == fileHistory.FileID).ToList();
                    calculatedData = AutoMapper.Mapper.Map<List<FileCalculation>, List<FileCalculationViewModel>>(calculation);
                }
                /*
                using (var package = new ExcelPackage(newFile))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    //get header
                    for (int i = 1; i <= workSheet.Dimension.End.Column; i++)
                    {
                        if (workSheet.Cells[5, i].Value == null)
                        {
                            columnNames.Add(workSheet.Cells[1, 1].Value.ToString().Trim());
                            break;
                        }
                        columnNames.Add(workSheet.Cells[5, i].Value.ToString().Trim());
                        if (i == workSheet.Dimension.End.Column)
                        {
                            columnNames.Add(workSheet.Cells[1, 1].Value.ToString().Trim());
                        }
                    }
                    //match column();
                    NewCondorDto dto = new NewCondorDto();
                    Type t = dto.GetType();
                    PropertyInfo[] prop = t.GetProperties();
                    bool columnMatched = false;
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        //columnMatched = false;
                        foreach (var item in prop)
                        {
                            if (columnNames[i].ToString().ToLower().Trim() == item.Name.ToString().ToLower().Trim())
                            {
                                columnMatched = true;
                                break;
                            }
                        }
                        if (!columnMatched)
                            throw new ArgumentException("File not correct");
                    }
                    //Read excel file 
                    FileHistory fileHistory = new FileHistory();
                    fileHistory.FileName = files.FileName;
                    fileHistory.FilePath = targetPath;
                    fileHistory.CreatedDate = DateTime.Now;
                    marketRRepo.Add<FileHistory>(fileHistory);
                    marketRRepo.UnitOfWork.SaveChanges();

                    IList<NewFileRecord> newList = new List<NewFileRecord>();

                    DateTime dateTime1 = DateTime.Now;

                    for (int row = 1; row <= noOfRow; row++)
                    {
                        if (row == 1 || row == 2 || row == 3 || row == 4 || row == 5) continue;
                        NewFileRecord newRecord = new NewFileRecord();

                        newRecord.N = workSheet.Cells[row, 1].Value == null ? "" : Convert.ToString(workSheet.Cells[row, 1].Value);
                        newRecord.DEAL_ID = workSheet.Cells[row, 2].Value == null ? "" : workSheet.Cells[row, 2].Value.ToString();
                        newRecord.ON_OFF_BALANCE = workSheet.Cells[row, 3].Value == null ? "" : workSheet.Cells[row, 3].Value.ToString();
                        newRecord.DEAL_TYPE = workSheet.Cells[row, 4].Value == null ? "" : workSheet.Cells[row, 4].Value.ToString();
                        newRecord.PROD_TYPE = workSheet.Cells[row, 5].Value == null ? "" : workSheet.Cells[row, 5].Value.ToString();
                        newRecord.PAY_RECIEVE = workSheet.Cells[row, 6].Value == null ? "" : Convert.ToString(workSheet.Cells[row, 6].Value);
                        newRecord.CCY = workSheet.Cells[row, 7].Value == null ? "" : workSheet.Cells[row, 7].Value.ToString();
                        newRecord.NOTIONAL = Convert.ToDouble(workSheet.Cells[row, 8].Value);
                        newRecord.MATURITY_DATE = Convert.ToString(workSheet.Cells[row, 9].Value);
                        newRecord.INTEREST_TYPE = workSheet.Cells[row, 10].Value == null ? "" : workSheet.Cells[row, 10].Value.ToString();
                        newRecord.FIXING_DATE = Convert.ToString(workSheet.Cells[row, 11].Value);
                        newRecord.INT_CHANGE_FREQ = workSheet.Cells[row, 12].Value == null ? "" : Convert.ToString(workSheet.Cells[row, 12].Value);
                        newRecord.INT_CHAGE_TERM = workSheet.Cells[row, 13].Value == null ? "" : workSheet.Cells[row, 13].Value.ToString();
                        newRecord.INT_PRE = Convert.ToDouble(workSheet.Cells[row, 14].Value);
                        newRecord.NPV_DELTA_ILS = Convert.ToDouble(workSheet.Cells[row, 15].Value);
                        newRecord.NETED = workSheet.Cells[row, 16].Value == null ? "" : Convert.ToString(workSheet.Cells[row, 16].Value);
                        newRecord.NETED_ID = workSheet.Cells[row, 17].Value == null ? "" : workSheet.Cells[row, 17].Value.ToString();
                        newRecord.Portfolio = workSheet.Cells[row, 18].Value == null ? "" : workSheet.Cells[row, 18].Value.ToString();
                        newRecord.NETTING_COUNTER = workSheet.Cells[row, 19].Value == null ? "" : Convert.ToString(workSheet.Cells[row, 19].Value);
                        newRecord.CONTRACT_MAT_DATE = Convert.ToString(workSheet.Cells[row, 20].Value);
                        newRecord.validity_date = Convert.ToString(workSheet.Cells[1, 4].Value);
                        newRecord.FileID = fileHistory.FileID;
                        newList.Add(newRecord);
                    }


                    marketRRepo.AddRange<NewFileRecord>(newList);

                    marketRRepo.UnitOfWork.SaveChanges();
                    DateTime dateTime2 = DateTime.Now;

                    TimeSpan diff = dateTime2 - dateTime1;

                    var logData = $"Start Import- {dateTime1}. End Import - {dateTime2}. Total time {diff}. Total Record Import-{newList.Count()}";
                    var fileName = $"C://Temp/Excellogfile{DateTime.Now.Ticks}.txt";
                    System.IO.File.WriteAllText(fileName, logData);

                    var calculation = marketRRepo.Find<FileCalculation>(x => x.FileID == fileHistory.FileID).ToList();
                    calculatedData = AutoMapper.Mapper.Map<List<FileCalculation>, List<FileCalculationViewModel>>(calculation);
                }
                */
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

        //My methods
        private TypeConvertSettings GetConvertSettings()
        {
            return new TypeConvertSettings
            {
                ConvertSettings = new List<TypeConverter>
                {
                    new TypeConverter
                    {
                        Type = typeof(DateTime),
                        TypeConverterOptions = new TypeConverterOptions
                        {
                            Format = "dd/MM/yyyy",
                        }
                    }
                }
            };
        }

        private void SaveCSVFile()
        {

        }

        [HttpPost]
        public JsonResult SaveCalculation(List<FileCalculationViewModel> fileCalculationViewModel)
        {
            foreach (var record in fileCalculationViewModel)
            {
                var fileRecord = AutoMapper.Mapper.Map<FileCalculationViewModel, FileCalculation>(record);
                marketRRepo.Add<FileCalculation>(fileRecord);
            }
            marketRRepo.UnitOfWork.SaveChanges();
            return Json(new { Success = true, Message = "Calculation Saved" });
        }
    }
}