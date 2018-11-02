﻿using AutoMapper.Configuration;
using AutoMapper;
using MapperInit = MarketR.Mappers.MapperInit;
using MarketR.Models;
using MarketR.Models.Condor;
using System.Collections.Generic;
using MarketR.ViewModel;
using MarketR.Service.CsvParser.Models;
using System;
using System.Globalization;

namespace MarketR.MVC
{
    public class MapperConfig
    {
        public static void RegisterMapper()
        {
            try
            {
                var cfg = new MapperConfigurationExpression();
                MarketR.Mappers.MapperInit.Init(cfg);
                MapperInit.Init(cfg);

                Mapper.Initialize(cfga =>
                {

                    cfga.CreateMap<CondorDtoNew, FileRecord>().ForMember(t => t.RecordID, opt => opt.Ignore())
                                                            .ForMember(t => t.FileHistory, opt => opt.Ignore())
                                                            .ForMember(t => t.FileID, opt => opt.Ignore()).ReverseMap();
                    cfga.CreateMap<NewCondorDto, NewFileRecord>().ForMember(t => t.RecordID, opt => opt.Ignore())
                                                            .ForMember(t => t.FileHistory, opt => opt.Ignore())
                                                            .ForMember(t => t.FileID, opt => opt.Ignore()).ReverseMap();
                    cfga.CreateMap<NewCondorDto, NewFileRecordsCSV>().ForMember(t => t.RecordID, opt => opt.Ignore())
                                                            .ForMember(t => t.FileHistory, opt => opt.Ignore())
                                                            .ForMember(t => t.FileID, opt => opt.Ignore()).ReverseMap();
                                                             

                    cfga.CreateMap<ParseResult<CondorDtoNew>, ParseResult<FileRecord>>().ReverseMap();
                    cfga.CreateMap<FileCalculationViewModel, FileCalculation>().ForMember(x => x.FileHistory, opt => opt.Ignore()).ReverseMap();
                    cfga.CreateMap<ParseResult<Report1Dto>, ParseResult<TBL_Simulation_DATE_CCY>>().ReverseMap();
                    cfga.CreateMap<ParseResult<Report2Dto>, ParseResult<tbl_results_date_simulation>>().ReverseMap();
                });
                //Mapper.Initialize(cfg);
                Mapper.AssertConfigurationIsValid();
            }
            catch (Exception ex) { }
        }
    }
}