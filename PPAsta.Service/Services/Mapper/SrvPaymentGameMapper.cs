using AutoMapper;
using PPAsta.Repository.Models.Entities.PaymentGame;
using PPAsta.Service.Models.PP.PaymentGame;

namespace PPAsta.Service.Services.Mapper
{
    public class SrvPaymentGameMapper : Profile
    {
        public SrvPaymentGameMapper()
        {
            CreateMap<SrvPaymentGame, MdlPaymentGame>().ReverseMap();
        }
    }
}
