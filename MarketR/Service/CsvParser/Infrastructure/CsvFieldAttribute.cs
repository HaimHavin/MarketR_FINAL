using System;

namespace MarketR.Service.CsvParser.Infrastructure
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