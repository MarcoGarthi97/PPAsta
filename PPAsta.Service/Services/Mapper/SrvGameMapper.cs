using AutoMapper;
using PPAsta.Repository.Models.Entities.Game;
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
        }
    }
}
