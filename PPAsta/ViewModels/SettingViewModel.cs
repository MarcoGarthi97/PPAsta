using CommunityToolkit.Mvvm.ComponentModel;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Services.Google;
using PPAsta.Service.Services.PP.Spreadsheet;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.ViewModels
{
    public class SettingViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvGoogleSpreadsheetService _googleSpreadsheetService;
        private readonly ISrvSpreadsheetService _spreadsheetService;

        public SettingViewModel(ISrvGoogleSpreadsheetService googleSpreadsheetService, ISrvSpreadsheetService spreadsheetService)
        {
            LoadConfigurations();

            _googleSpreadsheetService = googleSpreadsheetService;
            _spreadsheetService = spreadsheetService;
        }

        private string? _databasePath;
        public string? DatabasePath
        {
            get => _databasePath;
            set => SetProperty(ref _databasePath, value);
        }

        private void LoadConfigurations()
        {
            DatabasePath = SrvAppConfigurationStorage.DatabaseConfiguration.Path;
        }

        public async Task OnlineInsertDataAsync(string url, bool isDelete)
        {
            string csvData = await _googleSpreadsheetService.GetGoogleSpreadsheetDataAsync(url);

            await InsertDataAsync(csvData, isDelete);
        }

        private async Task InsertDataAsync(string data, bool isDelete)
        {
            await _spreadsheetService.ImportToDatabaseAsync(data, isDelete);
        }
    }
}
