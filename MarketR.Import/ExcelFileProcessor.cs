using ExcelDataReader;
using MarketR.Common.Models;
using MarketR.Common.Repository;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Ionic.Zip;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.Import
{
    public class ExcelFileProcessor : IFileProcesser
    {
        private readonly ImportSetting setting;
        public ExcelFileProcessor(ImportSetting importSetting, IMarketRRepo Repo)
        {
            setting = importSetting;
            marketRRepo = Repo;
        }
        public override void Import()
        {
            string unZipFolderPath = "";
            string unzipFolderName = "";
            string zipFileFullName = "";
            string currentExcelFile = "";
            try
            {
                FileInfo[] zipFiles = GetAllFiles(setting.FolderPath, new List<string> { ".zip" });
                if (zipFiles != null && zipFiles.Length > 0)
                {
                    foreach (var zipFile in zipFiles)
                    {
                        using (ZipFile zip = ZipFile.Read(zipFile.FullName))
                        {
                            unzipFolderName = zipFile.Name.Replace(".zip", "");
                            zipFileFullName = zipFile.Name;
                            if (zip.EntriesSorted.Count > 0)
                            {
                                if (zip.EntriesSorted.FirstOrDefault().FileName.Contains("/"))
                                {
                                    unzipFolderName = zip.EntriesSorted.FirstOrDefault().FileName.Substring(0, zip.EntriesSorted.FirstOrDefault().FileName.LastIndexOf("/"));
                                    unZipFolderPath = setting.FolderPath;
                                }
                                else
                                {
                                    unZipFolderPath = setting.FolderPath + "\\" + unzipFolderName;
                                }
                                zip.ExtractAll(unZipFolderPath, ExtractExistingFileAction.DoNotOverwrite);
                            }
                            else
                            {
                                throw new Exception("empty zip file");
                            }
                        }

                        FileInfo[] files = GetAllFiles(setting.FolderPath + "\\" + unzipFolderName, new List<string> { ".xls", ".xlsx" });
                        files = files.Where(f => f.Name.ToLower().Replace(".xls", "").Replace(".xlsx", "").EndsWith("all")).ToArray();
                        if (files.Length > 0)
                        {
                            foreach (FileInfo file in files)
                            {
                                currentExcelFile = file.Name;
                                DateTime fileDate = DateTime.MinValue;
                                var splitFileName = zipFileFullName.Split('_').ToList();
                                if (splitFileName != null && splitFileName.Count > 1)
                                {
                                    var fileDateStr = splitFileName.ElementAt(2).ToLower().Replace("z", "") + splitFileName.ElementAt(3).ToLower().Substring(0, 4) + "00";
                                    string format = "yyyyMMddHHmmss";
                                    try { fileDate = DateTime.ParseExact(fileDateStr, format, CultureInfo.InvariantCulture); }
                                    catch (Exception ex) { throw new Exception("Error! file name sholud be in formate DSC20190228_KIZUZ_CAD_ALL.XLS or DSC20190228_KIZUZ_CAD_ALL.XLSX"); }
                                }
                                else
                                    throw new Exception("Error! file name sholud be in formate DSC_KizuzCad_Z20190228_190303134946.zip");

                                List<string> columnNames = new List<string>();
                                IExcelDataReader reader = null;

                                using (FileStream stream = File.Open(setting.FolderPath + "\\" + unzipFolderName + "/" + file.Name, FileMode.Open))
                                {
                                    if (file.Name.ToLower().EndsWith(".xls"))
                                    {
                                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                                    }
                                    if (file.Name.ToLower().EndsWith(".xlsx"))
                                    {
                                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                                    }
                                    DataSet result = reader.AsDataSet();
                                    reader.Close();
                                    if (result.Tables.Count > 0)
                                    {
                                        var noOfCol = result.Tables[0].Columns.Count;
                                        var noOfRow = result.Tables[0].Rows.Count;

                                        if (noOfCol < 20) throw new Exception("Some of columns are missing in excel");

                                        //Read excel file 
                                        FileHistory fileHistory = new FileHistory();
                                        fileHistory.FileName = file.Name;
                                        fileHistory.FilePath = setting.FolderPath + "/" + file.Name;
                                        fileHistory.CreatedDate = DateTime.Now;
                                        fileHistory.FileDate = fileDate;

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
                                            newRecord.NPV_DELTA_ILS = object.ReferenceEquals(result.Tables[0].Rows[row][14], DBNull.Value) ? 0 : Convert.ToDouble(result.Tables[0].Rows[row][14]);
                                            newRecord.NETED = result.Tables[0].Rows[row][15] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][15]);
                                            newRecord.NETED_ID = result.Tables[0].Rows[row][16] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][16]);
                                            newRecord.Portfolio = result.Tables[0].Rows[row][17] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][17]);
                                            newRecord.NETTING_COUNTER = result.Tables[0].Rows[row][18] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][18]);
                                            newRecord.CONTRACT_MAT_DATE = result.Tables[0].Rows[row][18] == null ? "" : Convert.ToString(result.Tables[0].Rows[row][19]);
                                            newRecord.validity_date = result.Tables[0].Rows[0][3] == null ? "" : Convert.ToString(result.Tables[0].Rows[0][3]);
                                            // newRecord.FileID = fileHistory.FileID;
                                            newRecord.FileHistory = fileHistory;
                                            newList.Add(newRecord);
                                        }
                                        if (newList.Count > 0)
                                            SaveImportData(newList);
                                    }
                                }
                            }
                        }
                        else
                            throw new Exception("no xls or xlsx file find in zip file");

                        if (!Directory.Exists(setting.BackupFolderPath)) Directory.CreateDirectory(setting.BackupFolderPath);
                        if (File.Exists(setting.BackupFolderPath + "/" + zipFileFullName)) File.Delete(setting.BackupFolderPath + "/" + zipFileFullName);
                        File.Move(setting.FolderPath + "/" + zipFileFullName, setting.BackupFolderPath + "/" + zipFileFullName);

                        foreach (string Allfiles in Directory.GetFiles(setting.FolderPath + "/" + unzipFolderName))
                        {
                            FileInfo fileInfo = new FileInfo(Allfiles);
                            fileInfo.Delete();
                        }
                        Directory.Delete(setting.FolderPath + "/" + unzipFolderName);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(zipFileFullName))
                {
                    HandleValidationErrorException(ex, setting.NotificationEmail, zipFileFullName + ((string.IsNullOrEmpty(currentExcelFile) ? "" : "->" + currentExcelFile)));
                }
                throw ex;
            }
        }
        public void SaveImportData(IList<NewFileRecord> records)
        {
            marketRRepo.AddRange(records);
            marketRRepo.UnitOfWork.SaveChanges();
        }
    }
}
