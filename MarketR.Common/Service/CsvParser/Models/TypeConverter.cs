using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;

namespace MarketR.Common.Service.CsvParser.Models
{
    public class TypeConverter
    {
        public Type Type { get; set; }
        public TypeConverterOptions TypeConverterOptions { get; set; }
        //My methods
        public static TypeConvertSettings GetConvertSettings()
        {
            return new TypeConvertSettings
            {
                ConvertSettings = new List<TypeConverter>
                {
                    new TypeConverter
                    {
                        Type = typeof(DateTime),
                        TypeConverterOptions = new TypeConverterOptions
                        {
                            Format = "dd/MM/yyyy",
                        }
                    }
                }
            };
        }
    }
}