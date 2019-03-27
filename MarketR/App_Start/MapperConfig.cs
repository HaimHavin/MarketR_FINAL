using AutoMapper.Configuration;
using AutoMapper;
using MapperInit = MarketR.Common.Mappers.MapperInit;
using MarketR.ViewModel;
using System;
using MarketR.Common.Service.CsvParser.Models;
using MarketR.Common.Models.Condor;
using MarketR.Common.Models;
using MarketR.DAL.Models;

namespace MarketR.MVC
{
    public class MapperConfig
    {
        public static void RegisterMapper()
        {
            try
            {
                var cfg = new MapperConfigurationExpression();
                MarketR.Common.Mappers.MapperInit.Init(cfg);
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
                    cfga.CreateMap<ParseResult<Report2Dto>, ParseResult<tbl_comp1>>().ReverseMap();
                    cfga.CreateMap<ParseResult<Report2Dto>, ParseResult<tbl_comp2>>().ReverseMap();
                    cfga.CreateMap<ParseResult<Report2Dto>, ParseResult<tbl_comp3>>().ReverseMap();
                });
                //Mapper.Initialize(cfg);
                Mapper.AssertConfigurationIsValid();
            }
            catch (Exception ex) { }
        }
    }
}