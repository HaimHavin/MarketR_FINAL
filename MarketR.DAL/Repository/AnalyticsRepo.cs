using MarketR.Common.Models;
using MarketR.DAL.Models;
using MarketR.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.Common.Repository
{
    public class AnalyticsRepo
    {
        MarketREntities marketREntities;
        public AnalyticsRepo()
        {
            marketREntities = new MarketREntities();
        }
        public FileHistory GetAnalyticsData(DateTime date)
        {
            SqlParameter dateParam = new SqlParameter();
            dateParam.ParameterName = "@date";
            dateParam.Value = date;
            dateParam.SqlDbType = System.Data.SqlDbType.DateTime;
            return marketREntities.Database.SqlQuery<FileHistory>("GetAnalyticsFileData @date", dateParam).FirstOrDefault();
        }
        public List<SelectList> GetFileVersions()
        {
            return marketREntities.Database.SqlQuery<SelectList>("GetFileVersions").ToList();
        }
        public ImportSetting GetImportSetting(fileType fileType)
        {
            return marketREntities.ImportSettings.Where(d => d.FileType == (int)fileType).FirstOrDefault();
        }
    }
}
