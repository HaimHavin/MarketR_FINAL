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
    
    public partial class spFILE_UPLOAD_Result
    {
        public string Deal_ID { get; set; }
        public string Balance_Off_Balance { get; set; }
        public string Main_Type { get; set; }
        public string Deal_Type { get; set; }
        public Nullable<bool> Pay_Recieve { get; set; }
        public string Underlying { get; set; }
        public string CCY { get; set; }
        public Nullable<decimal> Notional { get; set; }
        public Nullable<System.DateTime> Maturity_Date { get; set; }
        public string Interest_Type { get; set; }
        public Nullable<System.DateTime> Interest_Change_Date { get; set; }
        public string Interest_change_N { get; set; }
        public Nullable<int> Interest_change_freq { get; set; }
        public Nullable<decimal> Interest_percent { get; set; }
        public Nullable<decimal> NPL_DELTA_ILS { get; set; }
        public string Portfolio { get; set; }
        public Nullable<decimal> GAMMA_ILS { get; set; }
        public Nullable<decimal> VEGA_ILS { get; set; }
        public string Couterparty { get; set; }
    }
}
