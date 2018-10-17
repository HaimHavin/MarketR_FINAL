using System;
using CsvHelper.TypeConversion;

namespace MarketR.Service.CsvParser.Models
{
    public class TypeConverter
    {
        public Type Type { get; set; }
        public TypeConverterOptions TypeConverterOptions { get; set; }
    }
}