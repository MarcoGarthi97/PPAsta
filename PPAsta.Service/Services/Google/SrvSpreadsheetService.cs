using AutoMapper;
using CsvHelper;
using Microsoft.Extensions.Logging;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.Google;
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
        Task<IEnumerable<SrvSpreadsheet>> GetGoogleSpreadsheetAsync(CancellationToken cancellationToken);
    }

    public class SrvSpreadsheetService : ISrvSpreadsheetService
    {
        private readonly ILogger<SrvSpreadsheetService> _logger;
        private readonly IMapper _mapper;

        private readonly ISrvSpreadsheetBuilderService _srvSpreadsheetBuilderService;

        public SrvSpreadsheetService(ILogger<SrvSpreadsheetService> logger, IMapper mapper, ISrvSpreadsheetBuilderService srvSpreadsheetBuilderService)
        {
            _logger = logger;
            _mapper = mapper;
            _srvSpreadsheetBuilderService = srvSpreadsheetBuilderService;
        }

        public async Task<IEnumerable<SrvSpreadsheet>> GetGoogleSpreadsheetAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(nameof(GetGoogleSpreadsheetAsync) + " start");

            string sheetID = "1nSSKkhASvRuoBdfUj_ztC9AM572qVZZ9lVvm5KUELIo";
            string url = $"https://docs.google.com/spreadsheets/d/{sheetID}/export?format=csv";

            using var client = new HttpClient();
            var csvData = await client.GetStringAsync(url);

            _logger.LogInformation(nameof(GetGoogleSpreadsheetAsync) + " end");

            return GetSpreadsheetAsync(csvData);
        }

        private IEnumerable<SrvSpreadsheet> GetSpreadsheetAsync(string csvData)
        {
            _logger.LogInformation(nameof(GetSpreadsheetAsync) + " start");

            using var reader = new StringReader(csvData);
            using var csv = new CsvReader(reader, culture: CultureInfo.InvariantCulture);

            var list = csv.GetRecords<SrvSpreadsheet>().ToList();

            _logger.LogInformation(nameof(GetSpreadsheetAsync) + " start");

            return list;
        }
    }
}
