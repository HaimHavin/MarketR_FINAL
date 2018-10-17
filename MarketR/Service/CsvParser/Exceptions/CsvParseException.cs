using System;

namespace MarketR.Service.CsvParser.Exceptions
{
    public class CsvParseException : Exception
    {
        public CsvParseException()
        {

        }
        public CsvParseException(string message)
        : base(message) { }

        public CsvParseException(string format, params object[] args)
        : base(string.Format(format, args)) { }

        public CsvParseException(string message, Exception innerException)
        : base(message, innerException) { }

        public CsvParseException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException) { }
    }
}