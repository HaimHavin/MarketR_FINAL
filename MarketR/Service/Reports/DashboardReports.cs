﻿using MarketR.Models;
using MarketR.Models.Condor;
using MarketR.Repository;
using MarketR.Service.CsvParser;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarketR.Reports
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
        public void PerformCalculation(string startDate, string currencyFormat)
        {
            try
            {
                var result = dbEntity.sp_Simulate1(Convert.ToDateTime(startDate), currencyFormat);

            }
            catch (Exception ex)
            {

            }
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