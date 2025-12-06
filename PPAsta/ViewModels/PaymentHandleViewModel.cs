using CommunityToolkit.Mvvm.ComponentModel;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Models.PP.Payment;
using PPAsta.Service.Services.PP.Game;
using PPAsta.Service.Services.PP.Payment;
using PPAsta.Service.Services.PP.PaymentGame;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Payments;

namespace PPAsta.ViewModels
{
    public class PaymentHandleViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvGameService _gameService;
        private readonly ISrvPaymentGameService _paymentGameService;
        private readonly ISrvPaymentService _paymentService;

        public PaymentHandleViewModel(ISrvGameService gameService, ISrvPaymentGameService paymentGameService, ISrvPaymentService paymentService)
        {
            _gameService = gameService;
            _paymentGameService = paymentGameService;
            _paymentService = paymentService;
        }
        
        private IEnumerable<SrvGameDetail> _gamesList = new List<SrvGameDetail>();
        public ObservableCollection<SrvGameDetail> Games { get; } = new ObservableCollection<SrvGameDetail>();
        public List<SrvGameDetail> SelectedGames { get; } = new();

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected!, value);
        }

        private string _buyer = "";
        public string? Buyer
        {
            get => _buyer;
            set => SetProperty(ref _buyer!, value);
        }

        private string? _year;
        public string? Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        private string? _totalPrice;
        public string? TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }

        public int _page = 0;
        public int _count = -1;

        private string _textResearch = "";
        public string? TextResearch
        {
            get => _textResearch;
            set => SetProperty(ref _textResearch!, value);
        }

        private string? _totalRecordsText;
        public string? TotalRecordsText
        {
            get => _totalRecordsText;
            set => SetProperty(ref _totalRecordsText, value);
        }

        private string? _pageText;
        public string? PageText
        {
            get => _pageText;
            set => SetProperty(ref _pageText, value);
        }

        int _buyerId;

        public async Task LoadDataAsync(SrvPaymentDetail paymentDetail)
        {
            ClearData();

            Buyer = paymentDetail.BuyerName;
            Year = paymentDetail.Year.ToString();
            TotalPrice = paymentDetail.TotalPurchasePrice.ToString();

            _buyerId = paymentDetail.BuyerId;

            await LoadGamesAsync();
        }

        public void ClearData()
        {
            TextResearch = string.Empty;
            TotalRecordsText = string.Empty;
            PageText = string.Empty;
            Buyer = string.Empty;
            Year = string.Empty;
            TotalPrice = string.Empty;
            _buyerId = 0;


            _gamesList = new List<SrvGameDetail>();
        }

        public void ClearPartialData()
        {
            _gamesList = new List<SrvGameDetail>();
        }

        public async Task LoadGamesAsync()
        {
            var gamesTemp = await GetGamesFilteredAsync();
            RecordsPagination(gamesTemp);
        }

        public async Task ReloadGamesAsync()
        {
            ClearData();

            await LoadGamesAsync();
        }

        public async Task PartialReloadGamesAsync()
        {
            ClearPartialData();

            await LoadGamesAsync();
        }

        public void CheckAll()
        {
            var uncheckedGames = Games.Where(x => !x.IsSelected);
            if (uncheckedGames.Any())
            {
                foreach (var uncheckedGame in uncheckedGames)
                {
                    uncheckedGame.IsSelected = true;
                }
            }
            else
            {
                foreach (var checkedGame in Games)
                {
                    checkedGame.IsSelected = false;
                }
            }
        }

        public int? GetTotalGamesChecked()
        {
            var gamesChecked = Games.Where(x => x.IsSelected && x.PaymentProcess == PaymentGameProcess.ToBePaid);
            if (gamesChecked.Any())
            {
                return (int)gamesChecked.Sum(x => x.PurchasePrice)!;
            }

            return null;
        }

        public async Task PaymentGamesAsync(int paymentType)
        {
            var checkedGames = Games.Where(x => x.IsSelected && x.PaymentProcess == PaymentGameProcess.ToBePaid);
            if (checkedGames.Any())
            {
                var paymentGames = await _paymentGameService.GetPaymentGameAsyncByGameIdsAsync(checkedGames.Select(x => x.Id));

                foreach (var paymentGame in paymentGames)
                {
                    paymentGame.PaymentType = (PaymentType)paymentType;
                    paymentGame.PaymentProcess = PaymentGameProcess.Paid;
                }

                await _paymentGameService.BulkUpdatePaymentGameAsync(paymentGames);

                await PartialReloadGamesAsync();

                var payment = await _paymentService.GetPaymentByBuyerIdAsync(_buyerId);
                var notPaidedGames = Games.Where(x => x.PaymentProcess == PaymentGameProcess.ToBePaid);

                if (notPaidedGames.Any())
                {
                    payment.PaymentProcess = PaymentProcess.NotFullyPaid;
                }
                else
                {
                    payment.PaymentProcess = PaymentProcess.Paid;
                }

                await _paymentService.UpdatePaymentAsync(payment);
            }            
        }

        private async Task<IEnumerable<SrvGameDetail>> GetGamesFilteredAsync()
        {
            IEnumerable<SrvGameDetail> gamesTemp = null;

            if (!_gamesList.Any())
            {
                _gamesList = await _gameService.GetAllGameDetailsByBuyerIdAsync(_buyerId);
            }

            var predicate = BuildPredicate();
            gamesTemp = _gamesList.Where(predicate).ToList();
            _count = gamesTemp.Count();

            TotalRecordsText = $"Records: {_count}";
            PageText = $"Pagina: {_page + 1} di {(_count / 50) + 1}";

            return gamesTemp;
        }

        private Func<SrvGameDetail, bool> BuildPredicate()
        {
            return e => e.Name.ToLower().Contains(_textResearch.ToLower())
                        || e.Owner.ToLower().Contains(_textResearch.ToLower());
        }

        public int GamesCountAsync()
        {
            return _count;
        }

        private void RecordsPagination(IEnumerable<SrvGameDetail> gamesTemp)
        {
            int skip = _page * 50;
            gamesTemp = gamesTemp.Skip(skip).Take(50);

            BindGrid(gamesTemp);
        }

        private void BindGrid(IEnumerable<SrvGameDetail> gamesTemp)
        {
            Games.Clear();

            foreach (var x in gamesTemp)
            {
                Games.Add(x);
            }
        }

        public async Task PrevButton()
        {
            if (_page > 0)
            {
                _page--;

                var gamesTemp = await GetGamesFilteredAsync();
                RecordsPagination(gamesTemp);
            }
        }

        public async Task NextButton()
        {
            if (_count - (50 + (_page * 50)) > 0)
            {
                _page++;

                var gamesTemp = await GetGamesFilteredAsync();
                RecordsPagination(gamesTemp);
            }
        }

        public async Task DataSortAsync(string propertyName, bool isAscending)
        {
            var gamesTemp = await GetGamesFilteredAsync();

            switch (propertyName)
            {
                case "Name":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.Name).ToList()
                        : gamesTemp.OrderByDescending(x => x.Name).ToList();
                    break;
                case "Owner":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.Owner).ToList()
                        : gamesTemp.OrderByDescending(x => x.Owner).ToList();
                    break;
                case "Year":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.Year).ToList()
                        : gamesTemp.OrderByDescending(x => x.Year).ToList();
                    break;
                case "Buyer":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.Buyer).ToList()
                        : gamesTemp.OrderByDescending(x => x.Buyer).ToList();
                    break;
                case "PaymentProcess":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.PaymentProcess).ToList()
                        : gamesTemp.OrderByDescending(x => x.PaymentProcess).ToList();
                    break;
                case "SellingPrice":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.SellingPrice).ToList()
                        : gamesTemp.OrderByDescending(x => x.SellingPrice).ToList();
                    break;
                case "PurchasePrice":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.PurchasePrice).ToList()
                        : gamesTemp.OrderByDescending(x => x.PurchasePrice).ToList();
                    break;
                case "ShareOwner":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.ShareOwner).ToList()
                        : gamesTemp.OrderByDescending(x => x.ShareOwner).ToList();
                    break;
                case "SharePP":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.SharePP).ToList()
                        : gamesTemp.OrderByDescending(x => x.SharePP).ToList();
                    break;
                default:
                    break;
            }

            RecordsPagination(gamesTemp);
        }

        public async Task DeletePaymentAsync(SrvGameDetail gameDetail)
        {
            var paymentGame = await _paymentGameService.GetPaymentGameAsyncByGameIdAsync(gameDetail.Id);
            paymentGame.PaymentProcess = PaymentGameProcess.ToBePaid;
            paymentGame.PaymentType = null;

            await _paymentGameService.UpdatePaymentGameAsync(paymentGame);

            await PartialReloadGamesAsync();

            var payment = await _paymentService.GetPaymentByBuyerIdAsync(_buyerId);
            var gamePayed = Games.Where(x => x.PaymentProcess == PaymentGameProcess.Paid);

            if (gamePayed.Any())
            {
                payment.PaymentProcess = PaymentProcess.NotFullyPaid;
            }
            else
            {
                payment.PaymentProcess = PaymentProcess.ToBePaid;
            }

            await _paymentService.UpdatePaymentAsync(payment);
        }

        public async Task RemoveGameFromBuyerAsync(SrvGameDetail gameDetail)
        {
            var paymentGame = await _paymentGameService.GetPaymentGameAsyncByGameIdAsync(gameDetail.Id);
            paymentGame.PaymentProcess = PaymentGameProcess.Insert;
            paymentGame.PaymentType = null;
            paymentGame.BuyerId = null;
            paymentGame.PurchasePrice = null;
            paymentGame.ShareOwner = null;
            paymentGame.SharePP = null;

            await _paymentGameService.UpdatePaymentGameAsync(paymentGame);

            await PartialReloadGamesAsync();

            var payment = await _paymentService.GetPaymentByBuyerIdAsync(_buyerId);

            if (Games.Any())
            {
                var gamePayed = Games.Where(x => x.PaymentProcess == PaymentGameProcess.ToBePaid);

                if (gamePayed.Any())
                {
                    payment.PaymentProcess = PaymentProcess.NotFullyPaid;
                }
                else
                {
                    payment.PaymentProcess = PaymentProcess.ToBePaid;
                }

                await _paymentService.UpdatePaymentAsync(payment);
                await _paymentService.HandlePaymentAsync(_buyerId);
            }
            else
            {
                await _paymentService.DeletePaymentAsync(payment);
            }
        }
    }
}
