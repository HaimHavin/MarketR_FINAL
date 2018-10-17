namespace MarketR.Service.CsvParser.Models
{
    public class ParseResultRow<TDataRecord> : ParseResultBase where TDataRecord : class, new()
    {
        public ParseResultRow()
        {
            Record = new TDataRecord();
        }
       public TDataRecord Record { get; set; }
    }
}