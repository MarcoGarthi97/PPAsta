using Microsoft.Extensions.Logging;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.Google;

namespace PPAsta.Service.Services.Google
{
    public interface ISrvSpreadsheetBuilderService : IForServiceCollectionExtension
    {
        SrvSpreadsheet SpreadsheetBuilder(string row);
    }

    public class SrvSpreadsheetBuilderService : ISrvSpreadsheetBuilderService
    {
        private readonly ILogger<SrvSpreadsheetBuilderService> _logger;

        public SrvSpreadsheetBuilderService(ILogger<SrvSpreadsheetBuilderService> logger)
        {
            _logger = logger;
        }

        public SrvSpreadsheet SpreadsheetBuilder(string row)
        {
            return null;
        }
    }
}
