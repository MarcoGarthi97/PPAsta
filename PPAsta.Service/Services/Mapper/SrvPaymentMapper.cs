using AutoMapper;
using PPAsta.Repository.Models.Entities.Payment;
using PPAsta.Service.Models.PP.Payment;

namespace PPAsta.Service.Services.Mapper
{
    public class SrvPaymentMapper : Profile
    {
        public SrvPaymentMapper()
        {
            CreateMap<SrvPaymentGame, MdlPaymentGame>().ReverseMap();
        }
    }
}
