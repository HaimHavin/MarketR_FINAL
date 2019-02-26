using System;

namespace MarketR.Common.Service.CsvParser.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvFieldAttribute : Attribute
    {
        public CsvFieldAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}