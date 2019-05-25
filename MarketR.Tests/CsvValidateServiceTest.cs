using System.IO;
using NUnit.Framework;
using MarketR.Common.Service.CsvValidator;

namespace MarketR.Tests
{
    [TestFixture]
    public class CsvValidateServiceTest
    {
        private CsvValidateService csvValidateService;

        [SetUp]
        public void Init()
        {
           csvValidateService = new CsvValidateService();
        }
        
        [Test]
        public void ValidCsvFile()
        {
            const bool expected = true;

            Assert.AreEqual(expected,csvValidateService.IsFileCsv("file.csv"));
        }

        [Test]
        public void InValidCsvFile()
        {
            const bool expected = false;

            Assert.AreEqual(expected, csvValidateService.IsFileCsv("file.css"));
        }

        [Test]
        public void ValidCsvFileUppercaseExtensions()
        {
            const bool expected = true;

            Assert.AreEqual(expected, csvValidateService.IsFileCsv("file.CsV"));
        }

        [Test]
        public void FileIsEmpty()
        {
            const bool expected = true;

            Stream fs = File.OpenRead(@"D:\_projects\MarketR\src\MarketR\MarketR.Tests\TestData\EmptyFile.csv");

            Assert.AreEqual(expected, csvValidateService.FileIsEmpty(fs));
        }
        [Test]
        public void FileNoEmpty()
        {
            const bool expected = false;

            Stream fs = File.OpenRead(@"D:\_projects\MarketR\src\MarketR\MarketR.Tests\TestData\ValidData.csv");

            Assert.AreEqual(expected, csvValidateService.FileIsEmpty(fs));
        }
    }
}
