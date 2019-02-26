using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketR.Common.UnitOfWork
{
    public interface IMarketRUnitOfWork
    {
        void SaveChanges();
    }
}
