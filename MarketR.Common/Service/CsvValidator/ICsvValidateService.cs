using System.IO;

namespace MarketR.Common.Service.CsvValidator
{
    public interface ICsvValidateService
    {
        bool IsFileCsv(string fileName);
        bool IsFileCsvXls(string fileName);
        bool FileIsEmpty(Stream file);
    }
}
