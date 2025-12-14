using AutoMapper;
using CsvHelper;
using Microsoft.Extensions.Logging;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.Google;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Models.PP.PaymentGame;
using PPAsta.Service.Services.PP.Game;
using PPAsta.Service.Services.PP.Payment;
using PPAsta.Service.Services.PP.PaymentGame;
using PPAsta.Service.Services.PP.Seller;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Spreadsheet
{
    public interface ISrvSpreadsheetService : IForServiceCollectionExtension
    {
        Task ImportToDatabaseAsync(string data, bool isDelete);
    }

    public class SrvSpreadsheetService : ISrvSpreadsheetService
    {
        private readonly ILogger<SrvSpreadsheetService> _logger;
        private readonly IMapper _mapper;

        private readonly ISrvGameService _gameService;
        private readonly ISrvPaymentGameService _paymentGameService;
        private readonly ISrvPaymentService _paymentService;
        private readonly ISrvSellerService _sellerService;

        public SrvSpreadsheetService(ILogger<SrvSpreadsheetService> logger, IMapper mapper, ISrvGameService gameService, ISrvPaymentGameService paymentGameService, ISrvPaymentService srvPaymentService, ISrvSellerService sellerService)
        {
            _logger = logger;
            _mapper = mapper;
            _gameService = gameService;
            _paymentGameService = paymentGameService;
            _paymentService = srvPaymentService;
            _sellerService = sellerService;
        }

        public async Task ImportToDatabaseAsync(string data, bool isDelete)
        {
            var rows = GetSpreadsheetAsync(data);

            if (isDelete)
            {
                var years = rows.Select(x => x.Anno).GroupBy(x => x).Select(x => x.Key);
                await DeleteAsync(years);
            }

            await ImportAsync(rows);
        }

        private async Task DeleteAsync(IEnumerable<int> years)
        {
            var games = await _gameService.GetGamesByYearsAsync(years);
            var paymentGames = await _paymentGameService.GetPaymentGameAsyncByGameIdsAsync(games.Select(x => x.Id));
            
            await _gameService.DeleteGameByYearsAsync(years);
            await _paymentGameService.DeletePaymentGamesByGameIdsAsync(games.Select(x => x.Id));
            await _paymentService.DeletePaymentByBuyerIdsAsync(paymentGames.Where(x => x.BuyerId != null).Select(x => x.BuyerId.Value));
            await _sellerService.DeleteSellerByPayementGameIdsAsync(paymentGames.Select(x => x.Id));
        }

        private async Task ImportAsync(IEnumerable<SrvSpreadsheet> rows)
        {
            var games = _mapper.Map<IEnumerable<SrvGame>>(rows);
            await _gameService.InsertGamesAsync(games);

            games = await _gameService.GetAllGamesAsync();

            var payments = BuildPayments(games, rows);
            await _paymentGameService.InsertPaymentGamesAsync(payments);
        }

        private IEnumerable<SrvPaymentGame> BuildPayments(IEnumerable<SrvGame> games, IEnumerable<SrvSpreadsheet> rows)
        {
            var gamesLookup = games.ToLookup(g => $"{g.Name}-{g.Owner}");

            return rows
                .Select(row => new
                {
                    Key = $"{row.NomeGioco}-{row.Proprietario}",
                    Price = row.Prezzo.GetValueOrDefault(100) / 100m,
                    Row = row
                })
                .Where(x => gamesLookup.Contains(x.Key))
                .Select(x => new SrvPaymentGame
                {
                    GameId = gamesLookup[x.Key].First().Id,
                    SellingPrice = x.Price,
                    PaymentProcess = PaymentGameProcess.Insert
                })
                .ToList();
        }


        private IEnumerable<SrvSpreadsheet> GetSpreadsheetAsync(string csvData)
        {
            _logger.LogInformation(nameof(GetSpreadsheetAsync) + " start");

            csvData = DataCleaning(csvData);

            using var reader = new StringReader(csvData);
            using var csv = new CsvReader(reader, culture: CultureInfo.InvariantCulture);

            var list = csv.GetRecords<SrvSpreadsheet>().ToList();

            _logger.LogInformation(nameof(GetSpreadsheetAsync) + " start");

            return list;
        }

        private string DataCleaning(string csvData)
        {
            csvData = csvData.Replace("€", "");

            return csvData;
        }
    }
}
