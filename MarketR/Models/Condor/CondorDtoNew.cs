using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketR.Infrastructure;
using MarketR.Service.CsvParser.Infrastructure;

namespace MarketR.Models.Condor
{
    public class CondorDtoNew
    {
        [CsvField("DealType")]
        [Required]
        public string DealType { get; set; }

        [CsvField("KondorId")]
        [Required]
        public string KondorId { get; set; }

        [CsvField("KTPId")]
        [Required]
        public string KTPId { get; set; }

        [CsvField("BalanceDeal")]
        [Required]
        public string BalanceDeal { get; set; }

        [CsvField("TypeOfInstrument")]
        [Required]
        //[PayReceive]
        public string TypeOfInstrument { get; set; }

        [Required]
        public string SubTypeOfInstrument { get; set; }

        [Required]
        public string LegType { get; set; }

        [Required]
        public string Asset { get; set; }

        [CsvField("Currency")]
        [Required]
        public string Currency { get; set; }

        [CsvField("Amount")]
        [Required]
        public string Amount { get; set; }

        [CsvField("MaturityDate")]
        [Required]
        public string MaturityDate { get; set; }

        [CsvField("RateType")]
        [Required]
        public string RateType { get; set; }

        [CsvField("FixingDate")]
        //[Required]
        public string FixingDate { get; set; }

        [CsvField("FixingFrequencyNumber")]
        //[Required]
        public string FixingFrequencyNumber { get; set; }

        [CsvField("FixingFrequencyPeriod")]
       // [Required]
        public string FixingFrequencyPeriod { get; set; }

       // [Required]
        public string Rate { get; set; }

        //[Required]
        [CsvField("NPVDeltaILS")]
        public string NPVDeltaILS { get; set; }

        [CsvField("Portfolio")]
        //[Required]
        public string Portfolio { get; set; }

       // [Required]
        public string GammaILS { get; set; }
    }
}