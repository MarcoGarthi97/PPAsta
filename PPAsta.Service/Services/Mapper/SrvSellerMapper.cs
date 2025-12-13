using AutoMapper;
using PPAsta.Repository.Models.Entities.Seller;
using PPAsta.Service.Models.PP.Seller;

namespace PPAsta.Service.Services.Mapper
{
    public class SrvSellerMapper : Profile
    {
        public SrvSellerMapper()
        {
            CreateMap<SrvSeller, MdlSeller>().ReverseMap();

            CreateMap<SrvSellerDetail, MdlSellerDetail>().ReverseMap();
        }
    }
}
