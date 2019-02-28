using MarketR.Common;
using MarketR.Common.Models;
using MarketR.Common.Repository;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.Import
{
    public abstract class IFileProcesser
    {
        public IMarketRRepo marketRRepo { get; set; }
        public FileInfo[] GetAllFiles(string folderPath, List<string> extensions)
        {
            DirectoryInfo info = new DirectoryInfo(folderPath);
            FileInfo[] files = info.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
            return files;
        }
        public abstract void Import();
        public void HandleValidationErrorException(Exception ex, string notificationEmail, string fileName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(notificationEmail))
                    MailHelper.SendMail("MarketR import service exceptions with file --> " + fileName, ex.Message, notificationEmail);
            }
            catch { }
        }
    }
}
