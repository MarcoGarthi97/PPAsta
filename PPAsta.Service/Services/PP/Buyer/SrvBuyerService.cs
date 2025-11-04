using AutoMapper;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Buyer;
using PPAsta.Repository.Services.Repositories.PP.Buyer;
using PPAsta.Service.Models.PP.Buyer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Buyer
{
    public interface ISrvBuyerService : IForServiceCollectionExtension
    {
        Task DeleteBuyerAsync(SrvBuyer buyer);
        Task<List<SrvBuyer>> GetAllBuyersAsync();
        Task<IEnumerable<SrvBuyer>> GetBuyerAsync(int number, int year);
        List<SrvBuyer> GetContentByCSV(string content);
        Task<int> GetNextNumberByYearAsync(int year);
        Task InsertBuyerAsync(SrvBuyer buyer);
        Task InsertBuyersAsync(IEnumerable<SrvBuyer> buyers);
        Task UpdateBuyerAsync(SrvBuyer buyer);
    }

    public class SrvBuyerService : ISrvBuyerService
    {
        private readonly IMapper _mapper;
        private readonly IMdlBuyerRepository _buyerRepository;

        public SrvBuyerService(IMdlBuyerRepository buyerRepository, IMapper mapper)
        {
            _buyerRepository = buyerRepository;
            _mapper = mapper;
        }

        public async Task<List<SrvBuyer>> GetAllBuyersAsync()
        {
            var buyersRepository = await _buyerRepository.GetAllBuyersAsync();
            return _mapper.Map<List<SrvBuyer>>(buyersRepository);
        }

        public async Task<IEnumerable<SrvBuyer>> GetBuyerAsync(int number, int year)
        {
            var buyersRepository = await _buyerRepository.GetBuyerAsync(number, year);
            return _mapper.Map<IEnumerable<SrvBuyer>>(buyersRepository);
        }

        public async Task InsertBuyerAsync(SrvBuyer buyer)
        {
            var buyerRepository = _mapper.Map<MdlBuyer>(buyer);
            buyerRepository.RCD = DateTime.Now;
            buyerRepository.RUD = DateTime.Now;

            await _buyerRepository.InsertBuyerAsync(buyerRepository);
        }

        public async Task InsertBuyersAsync(IEnumerable<SrvBuyer> buyers)
        {
            var buyersRepository = _mapper.Map<IEnumerable<MdlBuyer>>(buyers);
            foreach (var buyerRepository in buyersRepository)
            {
                buyerRepository.RCD = DateTime.Now;
                buyerRepository.RUD = DateTime.Now;
            }

            await _buyerRepository.InsertBuyersAsync(buyersRepository);
        }

        public async Task UpdateBuyerAsync(SrvBuyer buyer)
        {
            var buyerRepository = _mapper.Map<MdlBuyer>(buyer);
            buyerRepository.RUD = DateTime.Now;

            await _buyerRepository.UpdateBuyerAsync(buyerRepository);
        }

        public async Task DeleteBuyerAsync(SrvBuyer buyer)
        {
            var buyerRepository = _mapper.Map<MdlBuyer>(buyer);
            await _buyerRepository.DeleteBuyerAsync(buyerRepository);
        }

        public List<SrvBuyer> GetContentByCSV(string content)
        {
            var result = new List<SrvBuyer>();

            int skip = content.Contains("Nome")
                && content.Contains("Numero")
                && content.Contains("Anno") ? 1 : 0;

            var rows = content.Split("\n");
            foreach (var row in rows.Skip(skip))
            {
                var columns = row.Split(",");

                if (columns.Length >= 3)
                {
                    string name = !string.IsNullOrEmpty(columns[0]) ? columns[0] : null;
                    int? number = !string.IsNullOrEmpty(columns[1])
                        && Int32.TryParse(columns[1], out int n) ? n : null;
                    int? year = !string.IsNullOrEmpty(columns[2])
                        && Int32.TryParse(columns[2], out int y) ? y : null;

                    if (name != null && number != null && year != null)
                    {
                        result.Add(new SrvBuyer(name, number.Value, year.Value));
                    }
                }
            }

            return result;
        }

        public async Task<int> GetNextNumberByYearAsync(int year)
        {
            return await _buyerRepository.GetNextNumberByYearAsync(year);
        }
    }
}
