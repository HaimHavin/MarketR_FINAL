using MarketR.Common.Repository;
using MarketR.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.DAL.ExcelParser
{
    public abstract class FileHandler
    {
        public IMarketRRepo marketRRepo { get; set; }
        public MarketREntities _Context { get; set; }
        public FileHistory fileInfo { get; set; }
        public abstract void ImportData();
        public FileHistory GetFileInfo(int FileId)
        {
            return marketRRepo.GetAll<FileHistory>().Where(f => f.FileID == FileId).FirstOrDefault();
        }
    }
}
