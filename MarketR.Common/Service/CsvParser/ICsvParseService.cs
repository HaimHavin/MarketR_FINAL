using MarketR.Common.Service.CsvParser.Models;
using System;
using System.IO;

namespace MarketR.Common.Service.CsvParser
{
    public interface ICsvParseService<TDataRecord> where TDataRecord : class
    {
        ParseResult<TDataRecord> ParseData(Stream fileStream, string separator);
    }
}
