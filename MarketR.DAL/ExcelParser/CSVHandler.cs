using MarketR.Common.Repository;
using MarketR.Common.Service.CsvParser;
using MarketR.Common.Service.CsvParser.Models;
using MarketR.Common.Service.CsvValidator;
using MarketR.DAL.Models;
using MarketR.DAL.Models.Condor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.DAL.ExcelParser
{
    public class CSVHandler : FileHandler
    {
        private readonly ICsvValidateService csvValidateService;
        private readonly ICsvParseService<Models.Condor.KONDOR_DATA> csvParseService;
        public CSVHandler(int FileId)
        {
            _Context = new MarketREntities();
            marketRRepo = new MarketRRepo(_Context);
            fileInfo = GetFileInfo(FileId);
            csvValidateService = new CsvValidateService();
            csvParseService = new CsvParseService<Models.Condor.KONDOR_DATA>(TypeConverter.GetConvertSettings());
        }
        public override void ImportData()
        {
            if (fileInfo != null && !string.IsNullOrWhiteSpace(fileInfo.FilePath))
            {
                using (FileStream stream = File.Open(fileInfo.FilePath, FileMode.Open))
                {
                    if (csvValidateService.FileIsEmpty(stream))
                    {
                        throw new Exception("CSV file is empty");
                    }
                    if (!csvValidateService.IsFileCsvXls(fileInfo.FileName))
                    {
                        throw new Exception("Please, upload CSV file");
                    }
                    ParseResult<Models.Condor.KONDOR_DATA> KONDOR_DATAcsv = csvParseService.ParseData(stream, ",");
                    IList<Models.KONDOR_DATA> csvRecordList = new List<Models.KONDOR_DATA>();
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
                        throw new Exception(builder.ToString());
                    }
                    foreach (var record in KONDOR_DATAcsv.Records)
                    {
                        var fileRecord = new Models.Condor.KONDOR_DATA().MapToKONDOR_DATAEntity(record);
                        csvRecordList.Add(fileRecord);
                    }
                    if (csvRecordList.Count > 0)
                    {
                        _Context.Database.ExecuteSqlCommand("truncate table KONDOR_DATA");
                        marketRRepo.AddRange(csvRecordList);
                        marketRRepo.UnitOfWork.SaveChanges();
                    }
                }
            }
        }
    }
}
