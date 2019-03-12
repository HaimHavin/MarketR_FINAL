//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarketR.DAL.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class KONDOR_DATACSV
    {
        public int Id { get; set; }
        public string DealType { get; set; }
        public string KondorId { get; set; }
        public string KTPId { get; set; }
        public string BalanceDeal { get; set; }
        public string TypeOfInstrument { get; set; }
        public string SubTypeOfInstrument { get; set; }
        public string LegType { get; set; }
        public string Asset { get; set; }
        public string Currency { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<System.DateTime> MaturityDate { get; set; }
        public string RateType { get; set; }
        public Nullable<System.DateTime> FixingDate { get; set; }
        public string FixingFrequencyNumber { get; set; }
        public string FixingFrequencyPeriod { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<double> NPVDeltaILS { get; set; }
        public string Portfolio { get; set; }
        public string GammaILS { get; set; }
        public string VegaILS { get; set; }
        public string Cpty { get; set; }
        public string UserName { get; set; }
        public Nullable<System.DateTime> TradeDate { get; set; }
        public string CalcA { get; set; }
        public string CalcB { get; set; }
        public string CalcC { get; set; }
    }
}
