using AutoMapper;
using PPAsta.Repository.Models.Entities.Helper;
using PPAsta.Service.Models.PP.Helper;

namespace PPAsta.Service.Services.Mapper
{
    public class SrvHelperMapper : Profile
    {
        public SrvHelperMapper()
        {
            CreateMap<SrvHelper, MdlHelper>().ReverseMap();
        }
    }
}
