
using MarketR.Common.Models;
using MarketR.Common.Models.Condor;
using MarketR.Common.Repository;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
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

        public IEnumerable<GetResultView_Result> GetFilterResultView(bool? NPV, string currency, int? band, string FilterText, string MaturityDate)
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

        public void PerformCompareCalculation(string currencyFormat, int version1, int version2)
        {
            dbEntity.sp_compare(currencyFormat, version1, version2);
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

        public void DealSimulation(DealSimulateModel model)
        {
            List<ObjectParameter> dealTypeParam = new List<ObjectParameter>();
            List<ObjectParameter> ccyTypeParam = new List<ObjectParameter>();
            List<ObjectParameter> npvTypeParam = new List<ObjectParameter>();
            List<ObjectParameter> legTypeParam = new List<ObjectParameter>();
            List<ObjectParameter> interestTypeParam = new List<ObjectParameter>();
            List<ObjectParameter> maturityTypeParam = new List<ObjectParameter>();
            List<ObjectParameter> refTypeParam = new List<ObjectParameter>();

            List<ObjectParameter> finalParam = new List<ObjectParameter>();

            var startDateParameter = model.StartDate != null ?
                new ObjectParameter("StartDate", model.StartDate) :
                new ObjectParameter("StartDate", typeof(DateTime));

            var currencyParameter = model.Currency != null ?
                new ObjectParameter("Currency", model.Currency) :
                new ObjectParameter("Currency", typeof(string));

            var fileidParameter = model.FileVersion != null ?
                new ObjectParameter("FileId", model.FileVersion) :
                new ObjectParameter("FileId", typeof(int));


            for (int i = 0; i < 10; i++)
            {
                dealTypeParam.Add(model.DealSimulate[i].DealType != null ? new ObjectParameter("Deal_Type" + i.ToString(), model.DealSimulate[i].DealType) : new ObjectParameter("Deal_Type" + i.ToString(), typeof(string)));
                ccyTypeParam.Add(model.DealSimulate[i].CCY != null ? new ObjectParameter("CCY" + i.ToString(), model.DealSimulate[i].CCY) : new ObjectParameter("CCY" + i.ToString(), typeof(string)));
                legTypeParam.Add(model.DealSimulate[i].Leg != null ? new ObjectParameter("Leg" + i.ToString(), model.DealSimulate[i].Leg) : new ObjectParameter("Leg" + i.ToString(), typeof(int)));
                npvTypeParam.Add(model.DealSimulate[i].NPV != null ? new ObjectParameter("NPV" + i.ToString(), model.DealSimulate[i].NPV) : new ObjectParameter("NPV" + i.ToString(), typeof(string)));
                interestTypeParam.Add(model.DealSimulate[i].Interest != null ? new ObjectParameter("Interest" + i.ToString(), model.DealSimulate[i].Interest) : new ObjectParameter("Interest" + i.ToString(), typeof(bool)));
                maturityTypeParam.Add(model.DealSimulate[i].MaturityDate.HasValue ? new ObjectParameter("MaturityDate" + i.ToString(), model.DealSimulate[i].MaturityDate) : new ObjectParameter("MaturityDate" + i.ToString(), typeof(DateTime)));
                refTypeParam.Add(model.DealSimulate[i].Ref != null ? new ObjectParameter("Ref" + i.ToString(), model.DealSimulate[i].Ref) : new ObjectParameter("Ref" + i.ToString(), typeof(string)));
            }
            finalParam.Add(startDateParameter);
            finalParam.Add(currencyParameter);
            finalParam.Add(fileidParameter);

            finalParam.AddRange(dealTypeParam);
            finalParam.AddRange(ccyTypeParam);
            finalParam.AddRange(legTypeParam);
            finalParam.AddRange(npvTypeParam);
            finalParam.AddRange(interestTypeParam);
            finalParam.AddRange(maturityTypeParam);
            finalParam.AddRange(refTypeParam);
            ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("run_deal_simulation", finalParam.ToArray());

            // dbEntity.run_deal_simulation(model.StartDate,model.Currency,model.FileVersion,);           
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