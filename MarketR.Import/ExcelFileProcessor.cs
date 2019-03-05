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
            FileInfo[] files = GetAllFiles(setting.FolderPath, new List<string> { ".xlsx", ".xls" });
            if (files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    try
                    {
                        DateTime fileDate = DateTime.MinValue;
                        var splitFileName = file.Name.Split('_').ToList();
                        if (splitFileName != null && splitFileName.Count > 1)
                        {
                            var fileDateStr = splitFileName.ElementAt(1).Substring(0, splitFileName.ElementAt(1).IndexOf('.')).Replace("-", "") + "00";
                            string format = "yyyyMMddHHmmss";
                            try { fileDate = DateTime.ParseExact(fileDateStr, format, CultureInfo.InvariantCulture); }
                            catch (Exception ex) { throw new Exception("Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.xlsx"); }
                        }
                        else
                            throw new Exception("Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.xlsx");

                        List<string> columnNames = new List<string>();
                        IExcelDataReader reader = null;

                        using (FileStream stream = File.Open(setting.FolderPath + "/" + file.Name, FileMode.Open))
                        {
                            if (file.Name.ToLower().EndsWith(".xls"))
                            {
                                reader = ExcelReaderFactory.CreateBinaryReader(stream);
                            }
                            else if (file.Name.ToLower().EndsWith(".xlsx"))
                            {
                                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                            }
                            else
                                throw new Exception("Please, upload xls or xlsx file");

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
                                if (!Directory.Exists(setting.BackupFolderPath)) Directory.CreateDirectory(setting.BackupFolderPath);
                                File.Move(setting.FolderPath + "/" + file, setting.BackupFolderPath + "/" + file);
                                if (newList.Count > 0)
                                    SaveImportData(newList);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleValidationErrorException(ex, setting.NotificationEmail, file.Name);
                        throw ex;
                    }
                }
            }
        }
        public void SaveImportData(IList<NewFileRecord> records)
        {
            marketRRepo.AddRange(records);
            marketRRepo.UnitOfWork.SaveChanges();
        }
    }
}
