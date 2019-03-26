using MarketR.Common.Models;
using MarketR.Common.Models.Condor;
using MarketR.Common.Repository;
using MarketR.Common.Service.CsvParser;
using MarketR.Common.Service.CsvParser.Exceptions;
using MarketR.Common.Service.CsvParser.Models;
using MarketR.Common.Service.CsvValidator;
using MarketR.DAL.Models;
using MarketR.DAL.Models.Condor;
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
        private readonly ICsvParseService<DAL.Models.Condor.KONDOR_DATA> csvParseService;
        public CSVFileProcessor(ImportSetting importSetting, IMarketRRepo Repo)
        {
            setting = importSetting;
            marketRRepo = Repo;
            csvValidateService = new CsvValidateService();
            csvParseService = new CsvParseService<DAL.Models.Condor.KONDOR_DATA>(TypeConverter.GetConvertSettings());
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
                        IList<DAL.Models.KONDOR_DATA> csvRecordList = new List<DAL.Models.KONDOR_DATA>();
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
                        bool IsValid = false;
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
                            ParseResult<DAL.Models.Condor.KONDOR_DATA> KONDOR_DATAcsv = csvParseService.ParseData(stream, ",");

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
                            else IsValid = true;
                        }
                        if (IsValid)
                        {
                            if (!Directory.Exists(setting.FileSavePath))
                                Directory.CreateDirectory(setting.FileSavePath);
                            if (File.Exists(Path.Combine(setting.FileSavePath, file.Name))) File.Delete(Path.Combine(setting.FileSavePath, file.Name));
                            File.Copy(Path.Combine(setting.FolderPath, file.Name), Path.Combine(setting.FileSavePath, file.Name));

                            KondorFileHistory fileHistory = new KondorFileHistory();
                            fileHistory.FileName = file.Name;
                            fileHistory.FilePath = Path.Combine(setting.FileSavePath, file.Name);
                            fileHistory.CreatedDate = DateTime.Now;
                            fileHistory.FileDate = fileDate;
                            AddFileHistory(fileHistory);
                        }
                        if (!Directory.Exists(setting.BackupFolderPath)) Directory.CreateDirectory(setting.BackupFolderPath);
                        File.Move(setting.FolderPath + "/" + file, setting.BackupFolderPath + "/" + file);

                    }
                    catch (Exception ex)
                    {
                        HandleValidationErrorException(ex, setting.NotificationEmail, file.Name);
                        throw ex;
                    }
                }
            }
        }
        public void AddFileHistory(KondorFileHistory fileHistory)
        {
            marketRRepo.Add<KondorFileHistory>(fileHistory);
            marketRRepo.UnitOfWork.SaveChanges();
        }
    }
}
