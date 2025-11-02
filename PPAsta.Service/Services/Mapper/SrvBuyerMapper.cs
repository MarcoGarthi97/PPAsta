using AutoMapper;
using PPAsta.Repository.Models.Entities.Buyer;
using PPAsta.Service.Models.PP.Buyer;

namespace PPAsta.Service.Services.Mapper
{
    public class SrvBuyerMapper : Profile
    {
        public SrvBuyerMapper()
        {
            CreateMap<SrvBuyer, MdlBuyer>().ReverseMap();
        }
    }
}
