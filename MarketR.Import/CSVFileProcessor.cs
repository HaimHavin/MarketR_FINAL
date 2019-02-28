using MarketR.Common.Models;
using MarketR.Common.Models.Condor;
using MarketR.Common.Repository;
using MarketR.Common.Service.CsvParser;
using MarketR.Common.Service.CsvParser.Exceptions;
using MarketR.Common.Service.CsvParser.Models;
using MarketR.Common.Service.CsvValidator;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.Import
{
    public class CSVFileProcessor : IFileProcesser
    {
        private readonly ImportSetting setting;
        private readonly ICsvValidateService csvValidateService;
        private readonly ICsvParseService<NewCondorDto> csvParseService;
        public CSVFileProcessor(ImportSetting importSetting, IMarketRRepo Repo)
        {
            setting = importSetting;
            marketRRepo = Repo;
            csvValidateService = new CsvValidateService();
            csvParseService = new CsvParseService<NewCondorDto>(TypeConverter.GetConvertSettings());
        }
        public override void Import()
        {
            FileInfo[] files = GetAllFiles(setting.FolderPath, new List<string> { ".csv" });
            if (files.Length > 0)
            {
                foreach (FileInfo file in files)
                {
                    try
                    {
                        DateTime fileDate;
                        IList<NewFileRecordsCSV> csvRecordList = new List<NewFileRecordsCSV>();
                        var splitFileName = file.Name.Split('_').ToList();
                        if (splitFileName != null && splitFileName.Count > 1)
                        {
                            var fileDateStr = splitFileName.ElementAt(1).Substring(0, splitFileName.ElementAt(1).IndexOf('.')).Replace("-", "") + "00";
                            string format = "yyyyMMddHHmmss";
                            try
                            {
                                fileDate = DateTime.ParseExact(fileDateStr, format, CultureInfo.InvariantCulture);
                            }
                            catch (Exception ex) { throw new Exception("Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.csv"); }
                        }
                        else
                        {
                            throw new Exception("Error! file name sholud be in formate MarketRisk_20180630-0331.xls or MarketRisk_20180630-0331.csv");
                        }
                        if (file == null)
                        {
                            throw new Exception("Please, choose CSV file");
                        }
                        using (FileStream stream = File.Open(setting.FolderPath + "/" + file.Name, FileMode.Open))
                        {
                            if (csvValidateService.FileIsEmpty(stream))
                            {
                                throw new Exception("CSV file is empty");
                            }
                            if (!csvValidateService.IsFileCsvXls(file.Name))
                            {
                                throw new Exception("Please, upload CSV file");
                            }
                            ParseResult<NewCondorDto> condorDtoNew = csvParseService.ParseData(stream, ",");

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
                                throw new Exception(builder.ToString());
                            }

                            FileHistory fileHistory = new FileHistory();
                            fileHistory.FileName = file.Name;
                            fileHistory.FilePath = setting.FolderPath + "/" + file.Name;
                            fileHistory.CreatedDate = DateTime.Now;
                            fileHistory.FileDate = fileDate;
                            DateTime dateTime1 = DateTime.Now;
                            foreach (var record in condorDtoNew.Records)
                            {
                                var fileRecord = AutoMapper.Mapper.Map<NewCondorDto, NewFileRecordsCSV>(record);
                                fileRecord.FileHistory = fileHistory;
                                csvRecordList.Add(fileRecord);
                            }
                        }
                        if (!Directory.Exists(setting.BackupFolderPath)) Directory.CreateDirectory(setting.BackupFolderPath);
                        File.Move(setting.FolderPath + "/" + file, setting.BackupFolderPath + "/" + file);
                        if (csvRecordList.Count > 0)
                            SaveImportData(csvRecordList);
                    }
                    catch (Exception ex)
                    {
                        HandleValidationErrorException(ex, setting.NotificationEmail, file.Name);
                        throw ex;
                    }
                }
            }
        }
        public void SaveImportData(IList<NewFileRecordsCSV> records)
        {
            marketRRepo.AddRange(records);
            marketRRepo.UnitOfWork.SaveChanges();
        }
    }
}
