using System;
using System.IO;
using System.Collections.Generic;
using MarketR.Common.Models.Condor;
using MarketR.Common.Models;
using MarketR.DAL.Models;

namespace MarketR.Common.Reports
{
    public interface IDashboardReports
    {
        IEnumerable<Report1Dto> GetReport1(string startDate, string currencyFormat);
        IEnumerable<Report2Dto> GetReport2(string startDate, string currencyFormat);
        void PerformCalculation(string startDate, string currencyFormat);
        IEnumerable<GetResultView_Result> GetFilterResultView(bool? NPV, string currency, int? band,string FilterText);
        void UpdateSimLiquidate(SimulateModel model);
    }
}
