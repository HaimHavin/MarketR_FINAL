using MarketR.Common.Repository;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.Import
{
    public class FileProcesser : IFileProcesser
    {
        private readonly IFileProcesser processer;      
        public FileProcesser(ImportSetting setting, IMarketRRepo Repo)
        {
            switch (setting.FileType)
            {
                case 1:
                    processer = new CSVFileProcessor(setting, Repo);
                    break;

                case 2:
                    processer = new ExcelFileProcessor(setting, Repo);
                    break;
            }           
        }
        public override void Import()
        {
            processer.Import();
        }
    }
}
