using ExcelDataReader;
using MarketR.Common.Repository;
using MarketR.DAL.ExcelParser;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.DAL.ExcelParser
{
    public class ExcelHandler : FileHandler
    {
        public ExcelHandler(int FileId)
        {
            _Context = new MarketREntities();
            marketRRepo = new MarketRRepo(_Context);
            fileInfo = GetFileInfo(FileId);
        }
        public override void ImportData()
        {
            if (fileInfo != null && !string.IsNullOrWhiteSpace(fileInfo.FilePath))
            {
                List<string> columnNames = new List<string>();
                IExcelDataReader reader = null;

                using (FileStream stream = File.Open(fileInfo.FilePath, FileMode.Open))
                {
                    if (fileInfo.FileName.ToLower().EndsWith(".xls"))
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else if (fileInfo.FileName.ToLower().EndsWith(".xlsx"))
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    DataSet result = reader.AsDataSet();
                    reader.Close();
                    if (result.Tables.Count > 0)
                    {
                        var noOfCol = result.Tables[0].Columns.Count;
                        var noOfRow = result.Tables[0].Rows.Count;

                        if (noOfCol < 20) throw new Exception("Some of columns are missing in excel");

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
                            newRecord.FileID = fileInfo.FileID;
                            newList.Add(newRecord);
                        }
                        if (newList.Count > 0)
                        {
                            _Context.Database.ExecuteSqlCommand("truncate table NewFileRecords");
                            marketRRepo.AddRange(newList);
                            marketRRepo.UnitOfWork.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
