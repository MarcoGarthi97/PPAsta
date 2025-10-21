using AutoMapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Services.Repositories.PP.Version;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Version
{
    public interface ISrvVersionService : IForServiceCollectionExtension
    {
        Task CreateTableVersionAsync();
        Task<string> GetVersionAsync();
        Task InsertVersionAsync(string version = "1.0.0");
    }

    public class SrvVersionService : ISrvVersionService
    {
        private readonly IMdlVersionRepository _versionRepository;
        private readonly IMapper _mapper;

        public SrvVersionService(IMdlVersionRepository versionRepository, IMapper mapper)
        {
            _versionRepository = versionRepository;
            _mapper = mapper;
        }

        public async Task CreateTableVersionAsync()
        {
            await _versionRepository.CreateTableVersionAsync();
        }

        public async Task<string> GetVersionAsync()
        {
            return await _versionRepository.GetVersionAsync();
        }

        public async Task InsertVersionAsync(string version = "1.0.0")
        {
            await _versionRepository.InsertVersionAsync(version);
        }
    }
}
