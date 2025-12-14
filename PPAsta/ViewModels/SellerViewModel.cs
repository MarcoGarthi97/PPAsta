using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Seller;
using PPAsta.Service.Services.PP.Seller;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Payments;

namespace PPAsta.ViewModels
{
    public class SellerViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvSellerService _sellerService;

        private string _textResearch = "";
        private string? _totalRecordsText;
        private string? _pageText;

        private IEnumerable<SrvSellerDetail> _sellersList = new List<SrvSellerDetail>();

        public IAsyncRelayCommand LoadSellerCommand { get; }

        public int _page = 0;
        public int _count = -1;

        public ObservableCollection<SrvSellerDetail> Sellers { get; } = new ObservableCollection<SrvSellerDetail>();
        public ObservableCollection<ComboBoxPP> FilterYears { get; } = new ObservableCollection<ComboBoxPP>();

        public SellerViewModel(ISrvSellerService sellerService)
        {
            _sellerService = sellerService;
        }

        public string? TextResearch
        {
            get => _textResearch;
            set => SetProperty(ref _textResearch!, value);
        }

        public string? TotalRecordsText
        {
            get => _totalRecordsText;
            set => SetProperty(ref _totalRecordsText, value);
        }

        public string? PageText
        {
            get => _pageText;
            set => SetProperty(ref _pageText, value);
        }

        private ComboBoxPP _selectedFilter;
        public ComboBoxPP SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadSellersAsync()
        {
            var sellersTemp = await GetSellersFilteredAsync();
            RecordsPagination(sellersTemp);
        }

        public async Task ReloadSellersAsync()
        {
            ClearData();
            LoadComboBoxYear();

            await LoadSellersAsync();
        }

        public void ClearData()
        {
            _sellersList = new List<SrvSellerDetail>();
            TextResearch = string.Empty;
        }

        public void LoadComboBoxYear()
        {
            FilterYears.Clear();

            int year = SrvAppConfigurationStorage.OldestYear;

            FilterYears.Add(new ComboBoxPP { DisplayName = "", Value = -1 });

            while (year <= DateTime.Now.Year)
            {
                FilterYears.Add(new ComboBoxPP { DisplayName = year.ToString(), Value = year });

                if (SrvAppConfigurationStorage.InitializeYearNow.IsYearInitialized && SrvAppConfigurationStorage.InitializeYearNow.Year == year)
                {
                    SelectedFilter = FilterYears.FirstOrDefault(x => x.Value == year);
                }

                year++;
            }
        }

        private async Task<IEnumerable<SrvSellerDetail>> GetSellersFilteredAsync()
        {
            IEnumerable<SrvSellerDetail> sellersTemp = null;

            if (!_sellersList.Any())
            {
                _sellersList = await _sellerService.GetAllSellersAsync();
            }

            var predicate = BuildPredicate();
            sellersTemp = _sellersList.Where(predicate).ToList();
            _count = sellersTemp.Count();

            TotalRecordsText = $"Records: {_count}";
            PageText = $"Pagina: {_page + 1} di {(_count / 50) + 1}";

            return sellersTemp;
        }

        private Func<SrvSellerDetail, bool> BuildPredicate()
        {
            return e => (string.IsNullOrEmpty(_textResearch)
                        || e.Owner.ToLower().Contains(_textResearch.ToLower()))
                        && (SelectedFilter == null
                        || string.IsNullOrEmpty(SelectedFilter.DisplayName)
                        || e.Year.ToString() == SelectedFilter.DisplayName);
        }

        public int SellersCountAsync()
        {
            return _count;
        }

        private void RecordsPagination(IEnumerable<SrvSellerDetail> sellersTemp)
        {
            int skip = _page * 50;
            sellersTemp = sellersTemp.Skip(skip).Take(50);

            BindGrid(sellersTemp);
        }

        private void BindGrid(IEnumerable<SrvSellerDetail> sellersTemp)
        {
            Sellers.Clear();

            foreach (var x in sellersTemp)
            {
                Sellers.Add(x);
            }
        }

        public async Task PrevButton()
        {
            if (_page > 0)
            {
                _page--;

                var sellersTemp = await GetSellersFilteredAsync();
                RecordsPagination(sellersTemp);
            }
        }

        public async Task NextButton()
        {
            if (_count - (50 + (_page * 50)) > 0)
            {
                _page++;

                var sellersTemp = await GetSellersFilteredAsync();
                RecordsPagination(sellersTemp);
            }
        }

        public async Task DataSortAsync(string propertyName, bool isAscending)
        {
            var sellersTemp = await GetSellersFilteredAsync();

            switch (propertyName)
            {
                case "Owner":
                    sellersTemp = isAscending
                        ? sellersTemp.OrderBy(x => x.Owner).ToList()
                        : sellersTemp.OrderByDescending(x => x.Owner).ToList();
                    break;
                case "PaymentSellerProcess":
                    sellersTemp = isAscending
                        ? sellersTemp.OrderBy(x => x.PaymentSellerProcess).ToList()
                        : sellersTemp.OrderByDescending(x => x.PaymentSellerProcess).ToList();
                    break;
                case "TotalShareOwner":
                    sellersTemp = isAscending
                        ? sellersTemp.OrderBy(x => x.TotalShareOwner).ToList()
                        : sellersTemp.OrderByDescending(x => x.TotalShareOwner).ToList();
                    break;
                case "TotalSharePP":
                    sellersTemp = isAscending
                        ? sellersTemp.OrderBy(x => x.TotalSharePP).ToList()
                        : sellersTemp.OrderByDescending(x => x.TotalSharePP).ToList();
                    break;
                case "TotalGames":
                    sellersTemp = isAscending
                        ? sellersTemp.OrderBy(x => x.TotalGames).ToList()
                        : sellersTemp.OrderByDescending(x => x.TotalGames).ToList();
                    break;
                case "TotalGamesSold":
                    sellersTemp = isAscending
                        ? sellersTemp.OrderBy(x => x.TotalGamesSold).ToList()
                        : sellersTemp.OrderByDescending(x => x.TotalGamesSold).ToList();
                    break;
                default:
                    break;
            }

            RecordsPagination(sellersTemp);
        }
    }
}
