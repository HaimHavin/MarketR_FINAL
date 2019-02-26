using System.Collections.Generic;

namespace MarketR.Common.Service.CsvParser.Models
{
    public class ParseResult<TDataRecord> : ParseResultBase where TDataRecord : class
    {
        public ParseResult()
        {
            Records = new List<TDataRecord>();
        }
        public List<TDataRecord> Records { get; set; }
    }
}
