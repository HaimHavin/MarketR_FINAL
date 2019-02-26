using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketR.Common.Infrastructure;
using MarketR.Common.Service.CsvParser.Infrastructure;

namespace MarketR.Common.Models.Condor
{
    public class CondorDto
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