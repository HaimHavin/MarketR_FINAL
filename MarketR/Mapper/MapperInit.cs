using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketR.Mappers
{
    public class MapperInit
    {
        public static void Init(IMapperConfigurationExpression cfg)
        {
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            cfg.AddProfiles(assemblyName);
        }
    }
}