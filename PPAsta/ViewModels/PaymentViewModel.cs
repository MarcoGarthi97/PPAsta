using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Payment;
using PPAsta.Service.Services.PP.Payment;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.ViewModels
{
    public class PaymentViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvPaymentService _paymentService;

        private string _textResearch = "";
        private string? _totalRecordsText;
        private string? _pageText;

        private IEnumerable<SrvPaymentDetail> _paymentsList = new List<SrvPaymentDetail>();

        public IAsyncRelayCommand LoadPaymentCommand { get; }

        public int _page = 0;
        public int _count = -1;

        public ObservableCollection<SrvPaymentDetail> Payments { get; } = new ObservableCollection<SrvPaymentDetail>();
        public ObservableCollection<ComboBoxPP> FilterYears { get; } = new ObservableCollection<ComboBoxPP>();

        public PaymentViewModel(ISrvPaymentService paymentService)
        {
            _paymentService = paymentService;
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

        public async Task LoadPaymentsAsync()
        {
            var paymentsTemp = await GetPaymentsFilteredAsync();
            RecordsPagination(paymentsTemp);
        }

        public async Task ReloadPaymentsAsync()
        {
            ClearData();
            LoadComboBoxYear();

            await LoadPaymentsAsync();
        }

        public void ClearData()
        {
            _paymentsList = new List<SrvPaymentDetail>();
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
                    SelectedFilter = FilterYears.FirstOrDefault();
                }

                year++;
            }
        }

        private async Task<IEnumerable<SrvPaymentDetail>> GetPaymentsFilteredAsync()
        {
            IEnumerable<SrvPaymentDetail> paymentsTemp = null;

            if (!_paymentsList.Any())
            {
                _paymentsList = await _paymentService.GetAllPaymentDetailsAsync();
            }

            var predicate = BuildPredicate();
            paymentsTemp = _paymentsList.Where(predicate).ToList();
            _count = paymentsTemp.Count();

            TotalRecordsText = $"Records: {_count}";
            PageText = $"Pagina: {_page + 1} di {(_count / 50) + 1}";

            return paymentsTemp;
        }

        private Func<SrvPaymentDetail, bool> BuildPredicate()
        {
            return e => (string.IsNullOrEmpty(_textResearch)
                        || e.BuyerName.ToLower().Contains(_textResearch.ToLower()))
                        && (SelectedFilter == null
                        || string.IsNullOrEmpty(SelectedFilter.DisplayName)
                        || e.Year.ToString() == SelectedFilter.DisplayName);
        }

        public int PaymentsCountAsync()
        {
            return _count;
        }

        private void RecordsPagination(IEnumerable<SrvPaymentDetail> paymentsTemp)
        {
            int skip = _page * 50;
            paymentsTemp = paymentsTemp.Skip(skip).Take(50);

            BindGrid(paymentsTemp);
        }

        private void BindGrid(IEnumerable<SrvPaymentDetail> paymentsTemp)
        {
            Payments.Clear();

            foreach (var x in paymentsTemp)
            {
                Payments.Add(x);
            }
        }

        public async Task PrevButton()
        {
            if (_page > 0)
            {
                _page--;

                var paymentsTemp = await GetPaymentsFilteredAsync();
                RecordsPagination(paymentsTemp);
            }
        }

        public async Task NextButton()
        {
            if (_count - (50 + (_page * 50)) > 0)
            {
                _page++;

                var paymentsTemp = await GetPaymentsFilteredAsync();
                RecordsPagination(paymentsTemp);
            }
        }

        public async Task DataSortAsync(string propertyName, bool isAscending)
        {
            var paymentsTemp = await GetPaymentsFilteredAsync();

            switch (propertyName)
            {
                case "BuyerName":
                    paymentsTemp = isAscending
                        ? paymentsTemp.OrderBy(x => x.BuyerName).ToList()
                        : paymentsTemp.OrderByDescending(x => x.BuyerName).ToList();
                    break;
                case "PaymentProcess":
                    paymentsTemp = isAscending
                        ? paymentsTemp.OrderBy(x => x.PaymentProcess).ToList()
                        : paymentsTemp.OrderByDescending(x => x.PaymentProcess).ToList();
                    break;
                case "TotalPurchasePrice":
                    paymentsTemp = isAscending
                        ? paymentsTemp.OrderBy(x => x.TotalPurchasePrice).ToList()
                        : paymentsTemp.OrderByDescending(x => x.TotalPurchasePrice).ToList();
                    break;
                case "TotalShareOwner":
                    paymentsTemp = isAscending
                        ? paymentsTemp.OrderBy(x => x.TotalShareOwner).ToList()
                        : paymentsTemp.OrderByDescending(x => x.TotalShareOwner).ToList();
                    break;
                case "TotalSharePP":
                    paymentsTemp = isAscending
                        ? paymentsTemp.OrderBy(x => x.TotalSharePP).ToList()
                        : paymentsTemp.OrderByDescending(x => x.TotalSharePP).ToList();
                    break;
                case "TotalGames":
                    paymentsTemp = isAscending
                        ? paymentsTemp.OrderBy(x => x.TotalGames).ToList()
                        : paymentsTemp.OrderByDescending(x => x.TotalGames).ToList();
                    break;
                default:
                    break;
            }

            RecordsPagination(paymentsTemp);
        }
    }
}
