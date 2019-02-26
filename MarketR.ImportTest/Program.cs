using log4net;
using MarketR.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.ImportTest
{
    class Program
    {
        static ILog logger = LogManager.GetLogger("EventLogAppender");
        static void Main(string[] args)
        {
            try
            {
                logger.Info("MarketR import service has been started -->" + DateTime.Now);
                ImportHandler handler = new ImportHandler(logger);
                handler.ProcessFiles();
                logger.Info("MarketR import service has been completed -->" + DateTime.Now);
            }
            catch (Exception ex) { logger.Error(ex.Message); }
        }
    }
}
