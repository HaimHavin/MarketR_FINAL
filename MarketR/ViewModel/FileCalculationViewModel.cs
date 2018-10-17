using MarketR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketR.ViewModel
{
    public class FileCalculationViewModel
    {
        public int CalculationID { get; set; }
        public int FileID { get; set; }
        public string DealType { get; set; }
        public string KondorID { get; set; }
        public DateTime? CreatedDate { get; set; } 
        //public FileHistory FileHistory { get; set; }
    }
}