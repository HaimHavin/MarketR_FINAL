using System.IO;

namespace MarketR.Service.CsvValidator
{
    public interface ICsvValidateService
    {
        bool IsFileCsv(string fileName);
        bool IsFileCsvXls(string fileName);
        bool FileIsEmpty(Stream file);
    }
}
