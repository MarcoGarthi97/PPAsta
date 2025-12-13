using AutoMapper;
using PPAsta.Repository.Models.Entities.Export;
using PPAsta.Service.Models.PP.Export;

namespace PPAsta.Service.Services.Mapper
{
    public class SrvExportCsvMapper : Profile
    {
        public SrvExportCsvMapper()
        {
            CreateMap<SrvExportCsv, MdlExportCsv>().ReverseMap();
        }
    }
}
