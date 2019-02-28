using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using MarketR.Import;

namespace MarketR.ImportService
{
    public partial class ImportService : ServiceBase
    {
        static ILog logger = LogManager.GetLogger("EventLogAppender");
        Timer timer = new Timer();
        public ImportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("MarketR import service has been started -->" + DateTime.Now);
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 60000;
            timer.Enabled = true;
            timer.AutoReset = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                logger.Info("import process has been started -->" + DateTime.Now);
                ImportHandler handler = new ImportHandler(logger);
                handler.ProcessFiles();
                logger.Info("import process has been completed -->" + DateTime.Now);
                timer.Enabled = true;
            }
            catch (Exception ex) { logger.Error(ex.Message); this.Stop(); }
        }

        protected override void OnStop()
        {
            logger.Info("MarketR import service has been stopped -->" + DateTime.Now);
        }
    }
}
