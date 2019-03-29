
using MarketR.Common.Models;
using MarketR.Common.Models.Condor;
using MarketR.Common.Repository;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarketR.Common.Reports
{
    public class DashboardReports : IDashboardReports
    {
        IMarketRRepo marketRRepo = new MarketRRepo(new MarketREntities());
        MarketREntities dbEntity = new MarketREntities();
        public IEnumerable<Report1Dto> GetReport1(string startDate, string currencyFormat)
        {
            //IEnumerable<Report1Dto> report1;
            var report = marketRRepo.GetAll<TBL_Simulation_DATE_CCY>();
            var reportData = AutoMapper.Mapper.Map<IEnumerable<TBL_Simulation_DATE_CCY>, IEnumerable<Report1Dto>>(report);
            return reportData;
        }
        public IEnumerable<Report2Dto> GetReport2(string startDate, string currencyFormat)
        {
            var report = marketRRepo.GetAll<tbl_results_date_simulation>();
            var reportData = AutoMapper.Mapper.Map<IEnumerable<tbl_results_date_simulation>, IEnumerable<Report2Dto>>(report);
            return reportData;
        }


        public void PerformCalculation(string startDate, string currencyFormat, int fileId)
        {
            dbEntity.Database.CommandTimeout = 1200;
            dbEntity.sp_Simulate(Convert.ToDateTime(startDate), currencyFormat, fileId);
        }

        public IEnumerable<GetResultView_Result> GetFilterResultView(bool? NPV, string currency, int? band, string FilterText)
        {
            try
            {
                var result = dbEntity.GetResultView(currency, band, NPV, (string.IsNullOrWhiteSpace(FilterText) ? null : FilterText));
                return result;
            }
            catch (Exception ex)
            {

            }
            return new List<GetResultView_Result>();
        }
        public void UpdateSimLiquidate(SimulateModel model)
        {
            try
            {
                foreach (var item in model.SimViewChanges)
                {
                    dbEntity.UpdateSimLiquidate(item.RecordId, item.Status);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void PerformCompareCalculation(string startDate, string currencyFormat, int fileId)
        {
            dbEntity.sp_compare(Convert.ToDateTime(startDate), currencyFormat, fileId);
        }
        public IEnumerable<Report2Dto> GetVersion1()
        {
            var report = marketRRepo.GetAll<tbl_comp1>();
            var reportData = AutoMapper.Mapper.Map<IEnumerable<tbl_comp1>, IEnumerable<Report2Dto>>(report);
            return reportData;
        }
        public IEnumerable<Report2Dto> GetVersion2()
        {
            var report = marketRRepo.GetAll<tbl_comp2>();
            var reportData = AutoMapper.Mapper.Map<IEnumerable<tbl_comp2>, IEnumerable<Report2Dto>>(report);
            return reportData;
        }


        public IEnumerable<Report2Dto> GetVersion3()
        {
            var report = marketRRepo.GetAll<tbl_comp3>();
            var reportData = AutoMapper.Mapper.Map<IEnumerable<tbl_comp3>, IEnumerable<Report2Dto>>(report);
            return reportData;
        }


        //public  IEnumerable<CondorDto> GetSimulate()
        //{
        //    try
        //    {
        //        var report = marketRRepo.GetAll<ST_1_REMOVE_FUTURES>();
        //        var reportData = AutoMapper.Mapper.Map<IEnumerable<tbl_results_date_simulation>, IEnumerable<Report2Dto>>(report);
        //    }
        //    catch(Exception ex)
        //    {
        //    }
        //}
    }
}