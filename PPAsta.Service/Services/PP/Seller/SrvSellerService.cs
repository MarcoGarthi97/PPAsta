using AutoMapper;
using Microsoft.UI.Xaml;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Seller;
using PPAsta.Repository.Services.Repositories.PP.Seller;
using PPAsta.Service.Models.PP.PaymentGame;
using PPAsta.Service.Models.PP.Seller;
using PPAsta.Service.Services.PP.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Seller
{
    public interface ISrvSellerService : IForServiceCollectionExtension
    {
        Task DeleteSellerByPayementGameIdAsync(int paymentGameId);
        Task DeleteSellerByPayementGameIdsAsync(IEnumerable<int> paymentGameIds);
        Task<IEnumerable<SrvSellerDetail>> GetAllSellersAsync();
        Task<IEnumerable<SrvSeller>> GetSellerByGameIdsAsync(IEnumerable<int> gameIds);
        Task<SrvSellerDetail> GetSellerByPayementGameIdAsync(int paymentGameId);
        Task<IEnumerable<SrvSeller>> GetSellerByPayementGameIdsAsync(IEnumerable<int> paymentGameIds);
        Task InsertSellerAsync(int paymentGameId, int year);
        Task UpdateSellersAsync(IEnumerable<SrvSeller> sellers);
    }

    public class SrvSellerService : ISrvSellerService
    {
        private readonly IMapper _mapper;
        private readonly IMdlSellerRepository _sellerRepository;

        public SrvSellerService(IMapper mapper, IMdlSellerRepository sellerRepository)
        {
            _mapper = mapper;
            _sellerRepository = sellerRepository;
        }

        public async Task<IEnumerable<SrvSellerDetail>> GetAllSellersAsync()
        {
            var sellersCheckRepository = await _sellerRepository.GetAllSellersCheckAsync();
            var groupsSeller = sellersCheckRepository.GroupBy(x => (x.Owner, x.Year));

            var sellersDetailRepository = await _sellerRepository.GetAllSellersDetailAsync();
            var sellersDetail = _mapper.Map<IEnumerable<SrvSellerDetail>>(sellersDetailRepository);

            var sellersDetailDictionary = sellersDetail.ToDictionary(x => (x.Owner, x.Year));

            foreach (var groupSeller in groupsSeller)
            {
                if (!sellersDetailDictionary.TryGetValue(groupSeller.Key, out var detail))
                    continue;

                if (groupSeller.All(x => x.PaymentSellerProcess == PaymentSellerProcess.PaidToSeller))
                {
                    detail.PaymentSellerProcess = PaymentSellerProcess.PaidToSeller;
                }
                else if (groupSeller.All(x => x.PaymentSellerProcess == PaymentSellerProcess.NotPaid))
                {
                    detail.PaymentSellerProcess = PaymentSellerProcess.NotPaid;
                }
                else
                {
                    detail.PaymentSellerProcess = PaymentSellerProcess.PaidByBuyer;
                }
            }

            return sellersDetail;
        }

        public async Task<SrvSellerDetail> GetSellerByPayementGameIdAsync(int paymentGameId)
        {
            var sellerRepository = await _sellerRepository.GetSellerByPayementGameIdAsync(paymentGameId);
            return _mapper.Map<SrvSellerDetail>(sellerRepository);
        }

        public async Task<IEnumerable<SrvSeller>> GetSellerByPayementGameIdsAsync(IEnumerable<int> paymentGameIds)
        {
            var sellerRepository = await _sellerRepository.GetSellerByPayementGameIdsAsync(paymentGameIds);
            return _mapper.Map<IEnumerable<SrvSeller>>(sellerRepository);
        }

        public async Task<IEnumerable<SrvSeller>> GetSellerByGameIdsAsync(IEnumerable<int> gameIds)
        {
            var sellerRepository = await _sellerRepository.GetSellerByGameIdsAsync(gameIds);
            return _mapper.Map<IEnumerable<SrvSeller>>(sellerRepository);
        }

        public async Task InsertSellerAsync(int paymentGameId, int year)
        {
            var seller = new SrvSeller
            {
                PaymentGameId = paymentGameId,
                PaymentSellerProcess = Abstraction.Models.Enums.PaymentSellerProcess.NotPaid,
                Year = year,
                RCD = DateTime.Now,
                RUD = DateTime.Now
            };

            var sellerRepository = _mapper.Map<MdlSeller>(seller);
            await _sellerRepository.InsertSeller(sellerRepository);
        }

        public async Task UpdateSellersAsync(IEnumerable<SrvSeller> sellers)
        {
            var sellerRepository = _mapper.Map<IEnumerable<MdlSeller>>(sellers);
            await _sellerRepository.UpdateSellersAsync(sellerRepository);
        }

        public async Task DeleteSellerByPayementGameIdAsync(int paymentGameId)
        {
            await _sellerRepository.DeleteSellerByPayementGameIdAsync(paymentGameId);
        }

        public async Task DeleteSellerByPayementGameIdsAsync(IEnumerable<int> paymentGameIds)
        {
            await _sellerRepository.DeleteSellerByPayementGameIdsAsync(paymentGameIds);
        }
    }
}
