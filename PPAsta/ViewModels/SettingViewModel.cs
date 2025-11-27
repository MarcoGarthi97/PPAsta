using CommunityToolkit.Mvvm.ComponentModel;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.Helpers;
using PPAsta.Service.Models.PP.Helper;
using PPAsta.Service.Services.Google;
using PPAsta.Service.Services.PP.Helper;
using PPAsta.Service.Services.PP.Spreadsheet;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PPAsta.ViewModels
{
    public class SettingViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvGoogleSpreadsheetService _googleSpreadsheetService;
        private readonly ISrvSpreadsheetService _spreadsheetService;
        private readonly ISrvHelperService _helperService;

        public SettingViewModel(ISrvGoogleSpreadsheetService googleSpreadsheetService, ISrvSpreadsheetService spreadsheetService, ISrvHelperService helperService)
        {
            LoadConfigurations();

            _googleSpreadsheetService = googleSpreadsheetService;
            _spreadsheetService = spreadsheetService;
            _helperService = helperService;
        }

        private string? _databasePath;
        public string? DatabasePath
        {
            get => _databasePath;
            set => SetProperty(ref _databasePath, value);
        }

        private bool _isYearNow;
        public bool IsYearNow
        {
            get => _isYearNow;
            set => SetProperty(ref _isYearNow, value);
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

        public async Task InsertDataAsync(string data, bool isDelete)
        {
            await _spreadsheetService.ImportToDatabaseAsync(data, isDelete);
        }

        public async Task InitializeYearNowAsync()
        {
            var helper = await _helperService.GetHelperByKeyAsync("InitializeYear");

            if (helper != null)
            {
                var initializeYearNow = JsonSerializer.Deserialize<SrvInitializeYearNow>(helper.Json);
                initializeYearNow.Year = DateTime.Now.Year;
                initializeYearNow.IsYearInitialized = IsYearNow;

                helper.Json = JsonSerializer.Serialize(initializeYearNow);

                await _helperService.UpdateHelperAsync(helper);
            }
            else
            {
                var initializeYearNow = new SrvInitializeYearNow
                {
                    Year = DateTime.Now.Year,
                    IsYearInitialized = IsYearNow
                };

                helper = new SrvHelper
                {
                    Key = "InitializeYear",
                    Json = JsonSerializer.Serialize(initializeYearNow)
                };

                await _helperService.InsertHelpersAsync([helper]);
            }

            SrvAppConfigurationStorage.InitializeYearNow.IsYearInitialized = IsYearNow;
        }

        public async Task GetInitializeYearNowAsync()
        {
            IsYearNow = false;

            var helper = await _helperService.GetHelperByKeyAsync("InitializeYear");

            if (helper != null)
            {
                var initializeYearNow = JsonSerializer.Deserialize<SrvInitializeYearNow>(helper.Json);

                if (initializeYearNow != null)
                {
                    IsYearNow = initializeYearNow.IsYearInitialized;
                }
            }
        }
    }
}
