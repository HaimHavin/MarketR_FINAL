using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MarketR.Common.Infrastructure;
using MarketR.Common.Service.CsvParser.Infrastructure;

namespace MarketR.Common.Models.Condor
{
    public class NewCondorDto
    {
        [CsvField("N")]
        public string N { get; set; }

        [CsvField("DEAL_ID")]
        public string DEAL_ID { get; set; }

        [CsvField("ON_OFF_BALANCE")]
        public string ON_OFF_BALANCE { get; set; }

        [CsvField("DEAL_TYPE")]
        public string DEAL_TYPE { get; set; }

        [CsvField("PROD_TYPE")]
        public string PROD_TYPE { get; set; }

        [CsvField("PAY_RECIEVE")]
        public string PAY_RECIEVE { get; set; }

        [CsvField("CCY")]
        public string CCY { get; set; }

        [CsvField("NOTIONAL")]
        public double NOTIONAL { get; set; }

        [CsvField("MATURITY_DATE")]
        public string MATURITY_DATE { get; set; }

        [CsvField("INTEREST_TYPE")]
        public string INTEREST_TYPE { get; set; }

        [CsvField("FIXING_DATE")]
        public string FIXING_DATE { get; set; }

        [CsvField("INT_CHANGE_FREQ")]
        public string INT_CHANGE_FREQ { get; set; }

        [CsvField("INT_CHAGE_TERM")]
        public string INT_CHAGE_TERM { get; set; }

        [CsvField("INT_PRE")]
        public double INT_PRE { get; set; }

        [CsvField("NPV_DELTA_ILS")]
        public double NPV_DELTA_ILS { get; set; }

        [CsvField("NETED")]
        public string NETED { get; set; }

        [CsvField("NETED_ID")]
        public string NETED_ID { get; set; }

        [CsvField("Portfolio")]
        public string Portfolio { get; set; }

        [CsvField("NETTING_COUNTER")]
        public string NETTING_COUNTER { get; set; }

        [CsvField("CONTRACT_MAT_DATE")]
        public string CONTRACT_MAT_DATE { get; set; }

        [CsvField("validity date")]
        public string validity_date { get; set; }

    }
}