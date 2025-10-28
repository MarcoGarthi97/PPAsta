using AutoMapper;
using CsvHelper;
using Microsoft.Extensions.Logging;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.Google;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Services.PP.Game;
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

        public SrvSpreadsheetService(ILogger<SrvSpreadsheetService> logger, IMapper mapper, ISrvGameService gameService)
        {
            _logger = logger;
            _mapper = mapper;
            _gameService = gameService;
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
        }

        private async Task CreatePaymentForGamesAsync(IEnumerable<SrvGame> games, IEnumerable<SrvSpreadsheet> rows)
        {

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
