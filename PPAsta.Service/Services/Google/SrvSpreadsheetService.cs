using AutoMapper;
using CsvHelper;
using Microsoft.Extensions.Logging;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.Google;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Models.PP.PaymentGame;
using PPAsta.Service.Services.PP.Game;
using PPAsta.Service.Services.PP.PaymentGame;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.Google
{
    public interface ISrvSpreadsheetService : IForServiceCollectionExtension
    {
        Task ImportFromGoogleSpreadsheetToDatabaseAsync();
    }

    public class SrvSpreadsheetService : ISrvSpreadsheetService
    {
        private readonly ILogger<SrvSpreadsheetService> _logger;
        private readonly IMapper _mapper;

        private readonly ISrvGameService _gameService;
        private readonly ISrvPaymentGameService _paymentService;

        public SrvSpreadsheetService(ILogger<SrvSpreadsheetService> logger, IMapper mapper, ISrvGameService gameService, ISrvPaymentGameService paymentService)
        {
            _logger = logger;
            _mapper = mapper;
            _gameService = gameService;
            _paymentService = paymentService;
        }

        public async Task ImportFromGoogleSpreadsheetToDatabaseAsync()
        {
            var rows = await GetGoogleSpreadsheetAsync();

            await ImportToDatabaseAsync(rows);
        }

        private async Task ImportToDatabaseAsync(IEnumerable<SrvSpreadsheet> rows)
        {
            var games = _mapper.Map<IEnumerable<SrvGame>>(rows);
            await _gameService.InsertGamesAsync(games);

            games = await _gameService.GetAllGamesAsync();

            var payments = BuildPayments(games, rows);
            await _paymentService.InsertPaymentGamesAsync(payments);
        }

        private IEnumerable<SrvPaymentGame> BuildPayments(IEnumerable<SrvGame> games, IEnumerable<SrvSpreadsheet> rows)
        {
            Dictionary<string, List<decimal>> dictRows = new Dictionary<string, List<decimal>>();

            foreach (var x in rows)
            {
                decimal price = 1;

                if (x.Prezzo.HasValue)
                {
                    price = x.Prezzo.Value / 100;    
                }

                if (dictRows.ContainsKey(x.NomeGioco + "-" + x.Proprietario))
                {
                    dictRows[x.NomeGioco + "-" + x.Proprietario].Add(price);
                }
                else
                {
                    dictRows.Add(x.NomeGioco + "-" + x.Proprietario, new List<decimal> { price });
                }
            }

            Dictionary<string, List<int>> dictGames = new Dictionary<string, List<int>>();

            foreach (var x in games)
            {
                if (dictGames.ContainsKey(x.Name + "-" + x.Owner))
                {
                    dictGames[x.Name + "-" + x.Owner].Add(x.Id);
                }
                else
                {
                    dictGames.Add(x.Name + "-" + x.Owner, new List<int> { x.Id });
                }
            }

            var payments = new List<SrvPaymentGame>();

            foreach (var x in games)
            {
                if (dictRows.ContainsKey(x.Name + "-" + x.Owner) && dictGames.ContainsKey(x.Name + "-" + x.Owner))
                {
                    payments.Add(new SrvPaymentGame
                    {
                        GameId = dictGames[x.Name + "-" + x.Owner].First(),
                        SellingPrice = dictRows[x.Name + "-" + x.Owner].First(),
                        PaymentProcess = PaymentGameProcess.Insert
                    });

                    dictGames[x.Name + "-" + x.Owner].RemoveAt(0);
                    dictRows[x.Name + "-" + x.Owner].RemoveAt(0);
                }
            }

            return payments;
        }

        private async Task<IEnumerable<SrvSpreadsheet>> GetGoogleSpreadsheetAsync()
        {
            _logger.LogInformation(nameof(GetGoogleSpreadsheetAsync) + " start");

            using var client = new HttpClient();
            var csvData = await client.GetStringAsync(SrvAppConfigurationStorage.GoogleSpreadsheetConfiguration.Url);

            _logger.LogInformation(nameof(GetGoogleSpreadsheetAsync) + " end");

            return GetSpreadsheetAsync(csvData);
        }

        private IEnumerable<SrvSpreadsheet> GetSpreadsheetAsync(string csvData)
        {
            _logger.LogInformation(nameof(GetSpreadsheetAsync) + " start");

            csvData = DataCleansing(csvData);

            using var reader = new StringReader(csvData);
            using var csv = new CsvReader(reader, culture: CultureInfo.InvariantCulture);

            var list = csv.GetRecords<SrvSpreadsheet>().ToList();

            _logger.LogInformation(nameof(GetSpreadsheetAsync) + " start");

            return list;
        }

        private string DataCleansing(string csvData)
        {
            csvData = csvData.Replace("€", "");

            return csvData;
        }
    }
}
