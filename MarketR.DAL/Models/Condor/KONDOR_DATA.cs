using MarketR.Common.Service.CsvParser.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.DAL.Models.Condor
{
    public class KONDOR_DATA
    {
        public KONDOR_DATA() { }
        [CsvField("DealType")]
        //[Required]
        public string DealType { get; set; }

        [CsvField("KondorId")]
        //[Required]
        public string KondorId { get; set; }

        [CsvField("KTPId")]
        //[Required]
        public string KTPId { get; set; }

        [CsvField("BalanceDeal")]
        //[Required]
        public string BalanceDeal { get; set; }

        [CsvField("TypeOfInstrument")]
        //[Required]
        public string TypeOfInstrument { get; set; }

        [CsvField("SubTypeOfInstrument")]
        //[Required]
        public string SubTypeOfInstrument { get; set; }

        [CsvField("LegType")]
        //[Required]
        public string LegType { get; set; }

        [CsvField("Asset")]
        //[Required]
        public string Asset { get; set; }

        [CsvField("Currency")]
        //[Required]
        public string Currency { get; set; }

        [CsvField("Amount")]
        //[Required]
        public string Amount { get; set; }

        [CsvField("MaturityDate")]
        //[Required]
        public string MaturityDate { get; set; }

        [CsvField("RateType")]
        //[Required]
        public string RateType { get; set; }

        [CsvField("FixingDate")]
        //[Required]
        public string FixingDate { get; set; }

        [CsvField("FixingFrequencyNumber")]
        //[Required]
        public string FixingFrequencyNumber { get; set; }

        [CsvField("FixingFrequencyPeriod")]
        //[Required]
        public string FixingFrequencyPeriod { get; set; }

        [CsvField("Rate")]
        //[Required]
        public string Rate { get; set; }

        [CsvField("NPVDeltaILS")]
        //[Required]
        public string NPVDeltaILS { get; set; }

        [CsvField("Portfolio")]
        //[Required]
        public string Portfolio { get; set; }

        [CsvField("GammaILS")]
        //[Required]
        public string GammaILS { get; set; }

        [CsvField("VegaILS")]
        //[Required]
        public string VegaILS { get; set; }

        [CsvField("Cpty")]
        //[Required]
        public string Cpty { get; set; }

        [CsvField("UserName")]
        //[Required]
        public string UserName { get; set; }

        [CsvField("TradeDate")]
        //[Required]
        public string TradeDate { get; set; }

        [CsvField("CalcA")]
        //[Required]
        public string CalcA { get; set; }

        [CsvField("CalcB")]
        //[Required]
        public string CalcB { get; set; }

        [CsvField("CalcC")]
        //[Required]
        public string CalcC { get; set; }


        public KONDOR_DATACSV MapToKONDOR_DATACSV(KONDOR_DATA model)
        {
            double outAmount;
            DateTime outMaturityDate;
            DateTime outFixingDate;
            decimal outRate;
            double outNPVDeltaILS;
            DateTime outTradeDate;
            double? newAmount = double.TryParse(model.Amount, out outAmount) ? outAmount : default(double?);
            DateTime? newMaturityDate = DateTime.TryParse(model.MaturityDate, out outMaturityDate) ? outMaturityDate : default(DateTime?);
            DateTime? newFixingDate = DateTime.TryParse(model.FixingDate, out outFixingDate) ? outFixingDate : default(DateTime?);
            decimal? newRate = decimal.TryParse(model.Rate, out outRate) ? outRate : default(decimal?);
            double? newNPVDeltaILS = double.TryParse(model.NPVDeltaILS, out outNPVDeltaILS) ? outNPVDeltaILS : default(double?);
            DateTime? newTradeDate = DateTime.TryParse(model.TradeDate, out outTradeDate) ? outTradeDate : default(DateTime?);
            return new KONDOR_DATACSV()
            {
                DealType = model.DealType,
                KondorId = model.KondorId,
                KTPId = model.KTPId,
                BalanceDeal = model.BalanceDeal,
                TypeOfInstrument = model.TypeOfInstrument,
                SubTypeOfInstrument = model.SubTypeOfInstrument,
                LegType = model.LegType,
                Asset = model.Asset,
                Currency = model.Currency,
                Amount = newAmount,
                MaturityDate = newMaturityDate,
                RateType = model.RateType,
                FixingDate = newFixingDate,
                FixingFrequencyNumber = model.FixingFrequencyNumber,
                FixingFrequencyPeriod = model.FixingFrequencyPeriod,
                Rate = newRate,
                NPVDeltaILS = newNPVDeltaILS,
                Portfolio = model.Portfolio,
                GammaILS = model.GammaILS,
                VegaILS = model.VegaILS,
                Cpty = model.Cpty,
                UserName = model.UserName,
                TradeDate = newTradeDate,
                CalcA = model.CalcA,
                CalcB = model.CalcB,
                CalcC = model.CalcC
            };
        }
    }
}
