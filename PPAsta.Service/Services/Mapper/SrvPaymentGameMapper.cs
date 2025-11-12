using AutoMapper;
using PPAsta.Repository.Models.Entities.Payment;
using PPAsta.Repository.Models.Entities.PaymentGame;
using PPAsta.Service.Models.PP.Payment;
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

    public class SrvPaymentMapper : Profile
    {
        public SrvPaymentMapper()
        {
            CreateMap<SrvPayment, MdlPayment>().ReverseMap();


            CreateMap<SrvPaymentGame, SrvPayment>()
                .ForMember(dest => dest.BuyerId, opt => opt.MapFrom(from => from.BuyerId))
                .ForMember(dest => dest.TotalPurchasePrice, opt => opt.MapFrom(from => from.PurchasePrice))
                .ForMember(dest => dest.TotalShareOwner, opt => opt.MapFrom(from => from.ShareOwner))
                .ForMember(dest => dest.TotalSharePP, opt => opt.MapFrom(from => from.SharePP))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentProcess, opt => opt.Ignore());
        }
    }
}
