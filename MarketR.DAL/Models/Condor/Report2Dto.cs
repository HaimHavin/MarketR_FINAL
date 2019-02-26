namespace MarketR.Common.Models.Condor
{
    using System;
    using System.Collections.Generic;
    
    public class Report2Dto
    {
        public int ID { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public Nullable<double> ILS { get; set; }
        public Nullable<double> USD { get; set; }
        public Nullable<double> EUR { get; set; }
        public Nullable<double> GBP { get; set; }
        public Nullable<double> CHF { get; set; }
        public Nullable<double> JPY { get; set; }
        public Nullable<double> Other { get; set; }
        public Nullable<double> Total { get; set; }
    }
}
