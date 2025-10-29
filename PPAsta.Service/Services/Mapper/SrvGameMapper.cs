using AutoMapper;
using PPAsta.Repository.Models.Entities.Game;
using PPAsta.Service.Models.Google;
using PPAsta.Service.Models.PP.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.Mapper
{
    public class SrvGameMapper : Profile
    {
        public SrvGameMapper()
        {
            CreateMap<SrvGame, MdlGame>().ReverseMap();

            CreateMap<SrvGameDetail, MdlGameDetail>().ReverseMap();

            CreateMap<SrvSpreadsheet, SrvGame>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(from => from.NomeGioco))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(from => from.Proprietario))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(from => from.Anno));
        }
    }
}
