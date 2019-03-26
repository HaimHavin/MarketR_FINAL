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

                                    if (!Directory.Exists(setting.FileSavePath))
                                        Directory.CreateDirectory(setting.FileSavePath);
                                    if (File.Exists(Path.Combine(setting.FileSavePath, file.Name))) File.Delete(Path.Combine(setting.FileSavePath, file.Name));
                                    File.Copy(Path.Combine(setting.FolderPath, unzipFolderName, file.Name), Path.Combine(setting.FileSavePath, file.Name));

                                    FileHistory fileHistory = new FileHistory();
                                    fileHistory.FileName = file.Name;
                                    fileHistory.FilePath = Path.Combine(setting.FileSavePath, file.Name);
                                    fileHistory.CreatedDate = DateTime.Now;
                                    fileHistory.FileDate = fileDate;
                                    AddFileHistory(fileHistory);
                                }
                                else
                                    throw new Exception("Error! file name sholud be in formate DSC_KizuzCad_Z20190228_190303134946.zip");
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
        public void AddFileHistory(FileHistory fileHistory)
        {
            marketRRepo.Add<FileHistory>(fileHistory);
            marketRRepo.UnitOfWork.SaveChanges();
        }
    }
}
