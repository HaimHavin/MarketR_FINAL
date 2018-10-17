using System;
using System.IO;
using MarketR.Service.CsvParser.Models;
using MarketR.Models.Condor;
using System.Collections.Generic;

namespace MarketR.Reports
{
    public interface IDashboardReports
    {
        IEnumerable<Report1Dto> GetReport1(string startDate, string currencyFormat);
        IEnumerable<Report2Dto> GetReport2(string startDate, string currencyFormat);
        void PerformCalculation(string startDate, string currencyFormat);
    }
}
