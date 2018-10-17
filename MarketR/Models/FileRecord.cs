//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarketR.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FileRecord
    {
        public int RecordID { get; set; }
        public Nullable<int> FileID { get; set; }
        public string DealType { get; set; }
        public string KondorID { get; set; }
        public string KTPID { get; set; }
        public string BalanceDeal { get; set; }
        public string TypeOfInstrument { get; set; }
        public string SubTypeOfInstrument { get; set; }
        public string LegType { get; set; }
        public string Asset { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string MaturityDate { get; set; }
        public string RateType { get; set; }
        public string FixingDate { get; set; }
        public string FixingFrequencyNumber { get; set; }
        public string FixingFrequencyPeriod { get; set; }
        public string Rate { get; set; }
        public string NPVDeltaILS { get; set; }
        public string Portfolio { get; set; }
        public string GammaILS { get; set; }
    
        public virtual FileHistory FileHistory { get; set; }
    }
}
