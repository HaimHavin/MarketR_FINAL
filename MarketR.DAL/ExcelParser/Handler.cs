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
        public Handler(int FileId, fileType FileType, fileVersion file)
        {
            switch (FileType)
            {
                case fileType.Csv:
                    processer = new CSVHandler(FileId);
                    processer.ImportData();
                    break;
                case fileType.Excel:
                    switch (file)
                    {
                        case fileVersion.Version1:
                            processer = new ExcelHandler(FileId);
                            processer.ImportData();
                            break;
                        case fileVersion.Version2:
                            processer = new ExcelHandler(FileId);
                            processer.ImportData2();
                            break;
                    }
                    break;
            }
        }
    }
}
