using AutoMapper;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Repository.Models.Entities.Export;
using PPAsta.Repository.Services.Repositories.PP.Game;
using PPAsta.Service.Models.PP.Export;
using PPAsta.Service.Models.PP.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Export
{
    public interface ISrvExportCsvService : IForServiceCollectionExtension
    {
        void ExportCsvGameDetails(IEnumerable<SrvExportCsv> data, string filePath);
        Task<IEnumerable<SrvExportCsv>> GetDataForExportByGameIdsAsync(IEnumerable<int> gamesIds);
    }

    public class SrvExportCsvService : ISrvExportCsvService
    {
        private readonly IMapper _mapper;
        private readonly IMdlGameRepository _gameRepository;

        public SrvExportCsvService(IMapper mapper, IMdlGameRepository gameRepository)
        {
            _mapper = mapper;
            _gameRepository = gameRepository;
        }


        public async Task<IEnumerable<SrvExportCsv>> GetDataForExportByGameIdsAsync(IEnumerable<int> gamesIds)
        {
            var exportRepository = await _gameRepository.GetDataForExportByGameIdsAsync(gamesIds);
            return _mapper.Map<IEnumerable<SrvExportCsv>>(exportRepository);
        }


        public void ExportCsvGameDetails(IEnumerable<SrvExportCsv> data, string filePath)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Articolo; Venditore; Prezzo di partenza; Acquirente; Pagato dal compratore; Tipo pagamento compratore; Prezzo Acquisto; Quota PP; Quota Venditore; Pagato al venditore; Tipo pagamento venditore");

            foreach (var item in data)
            {
                string isPaid = item.PaymentProcess == PaymentGameProcess.Paid ? "Si" : "No";
                string typePaymentBuyer = ConvertPaymentTypeEnum(item.PaymentType);
                
                string isPaidForSeller = "";
                if (item.PaymentSellerProcess.HasValue)
                {
                    isPaidForSeller = item.PaymentSellerProcess == PaymentSellerProcess.PaidToSeller ? "Si" : "No";
                }
                string typePaymentSeller = ConvertPaymentTypeEnum(item.PaymentTypeForSeller);

                sb.AppendLine($"{item.Name}; {item.Owner}; {item.SellingPrice}; {item.Buyer}; {isPaid}; {typePaymentBuyer}; {item.PurchasePrice}; {item.SharePP}; {item.ShareOwner}; {isPaidForSeller}; {typePaymentSeller}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public string ConvertPaymentTypeEnum(PaymentType? paymentType)
        {
            if (paymentType.HasValue)
            {
                switch (paymentType)
                {
                    case PaymentType.Cash:
                        return "Contante";
                    case PaymentType.Paypal:
                        return "Paypal";
                    case PaymentType.BankTransfer:
                        return "Bonifico";
                    default:
                        return "Altro";
                }
            }

            return "";
        }
    }
}
