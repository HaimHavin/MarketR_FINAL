﻿using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketR.Common.Models
{
    public class MarketRModel
    {
        public static string SaveFileName(string fileName)
        {
            MarketREntities DbContext = new MarketREntities();
            DbContext.spFILE_UPLOAD(fileName);            
            return "";
        }
    }
}