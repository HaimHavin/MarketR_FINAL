using System;
using System.IO;
using MarketR.Service.CsvParser.Models;

namespace MarketR.Service.CsvParser
{
    public interface ICsvParseService<TDataRecord> where TDataRecord : class
    {
        ParseResult<TDataRecord> ParseData(Stream fileStream, string separator);
    }
}
