using System;
using System.Collections.Generic;
using System.IO;

namespace MarketR.Common.Service.CsvValidator
{
    public class CsvValidateService : ICsvValidateService
    {
        public bool IsFileCsv(string fileName)
        {
            string ext = Path.GetExtension(fileName);

            return string.Equals(ext, ".csv", StringComparison.OrdinalIgnoreCase);
        }
        public bool IsFileCsvXls(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            List<string> fileTypes = new List<string>()
            {
                ".csv",
                ".xls",
                ".xlsx"
            };
            foreach (string fileType in fileTypes)
            {
                if (!string.Equals(ext, fileType, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                else
                {
                    return true;
                }
            }
            return false;
            
        }

        public bool FileIsEmpty(Stream file)
        {
            return file.Length <= 0;
        }
    }
}