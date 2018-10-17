using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MarketR.Infrastructure;
using MarketR.Service.CsvParser;
using MarketR.Service.CsvParser.Infrastructure;
using NUnit.Framework;
using System.IO;
using System.Runtime.Remoting.Channels;
using CsvHelper.TypeConversion;
using MarketR.Service.CsvParser.Exceptions;
using MarketR.Service.CsvParser.Models;

namespace MarketR.Tests
{
    [TestFixture]
    public class CsvParseServiceTest
    {
        private CsvParseService<TestDto> csvParseService;

        [SetUp]
        public void Init()
        {
            csvParseService = new CsvParseService<TestDto>(new TypeConvertSettings
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
                    },
                    new TypeConverter
                    {
                        Type = typeof(decimal),
                        TypeConverterOptions = new TypeConverterOptions
                        {
                            CultureInfo = CultureInfo.InvariantCulture
                        }
                    }
                }
            });
        }

        [Test]
        public void ParseValidData()
        {
            const bool expected = true;
            Stream fs = File.OpenRead(@"D:\_projects\MarketR\src\MarketR\MarketR.Tests\TestData\ValidData.csv");

            var result = csvParseService.ParseData(fs, ",");

            Assert.AreEqual(result.IsValid, expected);
        }

        [Test]
        public void ParseInValidData()
        {
            const bool expected = false;
            Stream fs = File.OpenRead(@"D:\_projects\MarketR\src\MarketR\MarketR.Tests\TestData\InValidData.csv");

            var result = csvParseService.ParseData(fs, ",");

            Assert.AreEqual(result.IsValid, expected);
        }

        [Test]
        public void ParseEmptydData()
        {
            Stream fs = File.OpenRead(@"D:\_projects\MarketR\src\MarketR\MarketR.Tests\TestData\EmptyFile.csv");
            
            Assert.Throws<CsvParseException>(() => csvParseService.ParseData(fs, ","));
        }

        [Test]
        public void SetCovertSettings()
        {
            var settings = new TypeConvertSettings
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

    public class TestDto
    {
        [CsvField("Deal ID")]
        [Required]
        public string DealId { get; set; }

        [CsvField("Balance Off Balance")]
        [Required]
        public string BalanceOffBalance { get; set; }

        [CsvField("Main Type")]
        [Required]
        public string MainType { get; set; }

        [CsvField("Deal Type")]
        [Required]
        public string DealType { get; set; }

        [CsvField("Pay Recieve")]
        [Required]
        [PayReceive]
        public string PayRecieve { get; set; }

        [Required]
        public string Underlying { get; set; }

        [Required]
        public string CCY { get; set; }

        [Required]
        public decimal Notional { get; set; }

        [CsvField("Maturity Date")]
        [Required]
        public DateTime MaturityDate { get; set; }

        [CsvField("Interest Type")]
        [Required]
        public string InterestType { get; set; }

        [CsvField("Interest Change Date")]
        [Required]
        public DateTime InterestChangeDate { get; set; }

        [CsvField("Interest change N")]
        [Required]
        public string InterestChangeN { get; set; }

        [CsvField("Interest change freq")]
        [Required]
        public int InterestChangeFreq { get; set; }

        [CsvField("Interest percent")]
        [Required]
        public decimal InterestPercent { get; set; }

        [CsvField("NPL-DELTA ILS")]
        [Required]
        public decimal Npldeltails { get; set; }

        [Required]
        public string Portfolio { get; set; }

        [Required]
        [CsvField("GAMMA ILS")]
        public decimal GammaILS { get; set; }

        [CsvField("VEGA ILS")]
        [Required]
        public decimal VegaILS { get; set; }

        [Required]
        public string Couterparty { get; set; }
    }

}
