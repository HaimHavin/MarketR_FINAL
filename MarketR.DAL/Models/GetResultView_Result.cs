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
    
    public partial class GetResultView_Result
    {
        public int RecordID { get; set; }
        public string DEAL_ID { get; set; }
        public string DEAL_TYPE { get; set; }
        public string PROD_TYPE { get; set; }
        public string PAY_RECIEVE { get; set; }
        public string CCY { get; set; }
        public string MATURITY_DATE { get; set; }
        public string FIXING_DATE { get; set; }
        public Nullable<double> NOTIONAL { get; set; }
        public string INTEREST_TYPE { get; set; }
        public Nullable<double> INT_PRE { get; set; }
        public Nullable<double> NPV_DELTA_ILS { get; set; }
        public Nullable<int> Band { get; set; }
        public string Kondor_N { get; set; }
        public string CPName { get; set; }
        public Nullable<bool> Sim_Liquidate_flag { get; set; }
    }
}
