using MarketR.Common;
using MarketR.Common.Repository;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.DAL.ExcelParser
{
    public class Handler
    {
        private readonly FileHandler processer;
        public Handler(int FileId, fileType FileType)
        {
            switch (FileType)
            {
                case fileType.Csv:
                    processer = new CSVHandler(FileId);
                    break;

                case fileType.Excel:
                    processer = new ExcelHandler(FileId);
                    break;
            }
            processer.ImportData();
        }
    }
}
