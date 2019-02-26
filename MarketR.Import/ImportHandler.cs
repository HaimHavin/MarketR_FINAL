using log4net;
using MarketR.Common.Repository;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.Import
{
    public class ImportHandler
    {
        private IMarketRRepo marketRRepo;
        private readonly ILog _logger;
        public ImportHandler(ILog logger)
        {
            marketRRepo = new MarketRRepo(new MarketREntities());
            _logger = logger;
        }
        private FileProcesser obj;
        public void ProcessFiles()
        {
            var importSettings = marketRRepo.GetAll<ImportSetting>().Where(d => d.Active).ToList();
            foreach (var setting in importSettings)
            {
                if (!setting.LastRun.HasValue || (DateTime.Now.Subtract(setting.LastRun.Value).TotalMinutes == setting.Interval))
                {
                    obj = new FileProcesser(setting, marketRRepo);
                    obj.Import();
                    setting.LastRun = DateTime.Now;
                    marketRRepo.UnitOfWork.SaveChanges();
                }
            }
        }
    }
}
