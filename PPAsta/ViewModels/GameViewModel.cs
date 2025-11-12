using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Models.PP.Payment;
using PPAsta.Service.Models.PP.PaymentGame;
using PPAsta.Service.Services.PP.Game;
using PPAsta.Service.Services.PP.PaymentGame;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.ViewModels
{
    public class GameViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvGameService _gameService;
        private readonly ISrvPaymentGameService _paymentGameService;

        private string _textResearch = "";
        private string? _totalRecordsText;
        private string? _pageText;
        private IEnumerable<SrvGameDetail> _gamesList = new List<SrvGameDetail>();

        public IAsyncRelayCommand LoadGamesCommand { get; }

        public int _page = 0;
        public int _count = -1;

        public ObservableCollection<SrvGameDetail> Games { get; } = new ObservableCollection<SrvGameDetail>();

        public GameViewModel(ISrvGameService gameService, ISrvPaymentGameService paymentGameService)
        {
            _gameService = gameService;
            _paymentGameService = paymentGameService;

            LoadGamesCommand = new AsyncRelayCommand(LoadGamesAsync);
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

        public async Task LoadGamesAsync()
        {
            var gamesTemp = await GetGamesFilteredAsync();
            RecordsPagination(gamesTemp);
        }

        private async Task<IEnumerable<SrvGameDetail>> GetGamesFilteredAsync()
        {
            IEnumerable<SrvGameDetail> gamesTemp = null;

            if (!_gamesList.Any())
            {
                _gamesList = await _gameService.GetAllGameDetailsAsync();
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

        public async Task InsertCardPaymentGameAsync(SrvPaymentGame payment)
        {
            await _paymentGameService.InsertPaymentGameAsync(payment);
        }
    }
}
