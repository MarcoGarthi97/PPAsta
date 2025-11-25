using AutoMapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Helper;
using PPAsta.Repository.Services.Repositories.PP.Helper;
using PPAsta.Service.Models.PP.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Helper
{
    public interface ISrvHelperService : IForServiceCollectionExtension
    {
        Task<SrvHelper> GetHelperByKeyAsync(string key);
        Task InsertHelpersAsync(IEnumerable<MdlHelper> helpers);
        Task UpdateHelperAsync(SrvHelper helper);
    }
    public class SrvHelperService : ISrvHelperService
    {
        private readonly IMdlHelperRepository _helperRepository;
        private readonly IMapper _mapper;

        public SrvHelperService(IMdlHelperRepository helperRepository, IMapper mapper)
        {
            _helperRepository = helperRepository;
            _mapper = mapper;
        }

        public async Task<SrvHelper> GetHelperByKeyAsync(string key)
        {
            try
            {
                var helperRepository = await _helperRepository.GetHelperByKeyAsync(key);
                return _mapper.Map<SrvHelper>(helperRepository);
            }
            catch (Exception ex)
            {
                return new SrvHelper();
            }
        }

        public async Task InsertHelpersAsync(IEnumerable<MdlHelper> helpers)
        {
            var helpersRepository = _mapper.Map<IEnumerable<MdlHelper>>(helpers);
            await _helperRepository.InsertHelpersAsync(helpersRepository);
        }

        public async Task UpdateHelperAsync(SrvHelper helper)
        {
            var helperRepository = _mapper.Map<MdlHelper>(helper);
            await _helperRepository.UpdateHelperByKeyAsync(helperRepository);
        }
    }
}
