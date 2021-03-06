﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketR.Common.Models.Condor
{
    public class Simulate
    {
        public string DEAL_ID { get; set; }
        public string Deal_Type { get; set; }
        public string PROD_TYPE { get; set; }
        public string PAY_RECIEVE { get; set; }
        public string CCY { get; set; }
        public string MATURITY_DATE { get; set; }
        public string FIXING_DATE { get; set; }
        public double NOTIONAL { get; set; }
        public string INTEREST_TYPE { get; set; }
        public double INT_PRE { get; set; }
        public double NPV_DELTA_ILS { get; set; }
        public int Band { get; set; }
        public bool Sim_Liquidate_flag { get; set; }
    }
    public class SimulateModel
    {
        public List<SimViewUpdate> SimViewChanges { get; set; }
    }
    public class SimViewUpdate
    {
        public int RecordId { get; set; }
        public bool Status { get; set; }
    }

    public class DealSimulateModel
    {
        public DateTime StartDate { get; set; }
        public string Currency { get; set; }
        public int? FileVersion { get; set;}
        public List<DealSimulateItems> DealSimulate { get; set; }
    }
    public class DealSimulateItems
    {
        public string DealType { get; set; }
        public string CCY { get; set; }
        public int? Leg { get; set; }
        public string NPV { get; set; }
        public bool? Interest  { get; set; }
        public DateTime? MaturityDate { get; set; }
        public string Ref { get; set; }
    }
}