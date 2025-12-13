using CommunityToolkit.Mvvm.ComponentModel;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Models.PP.Seller;
using PPAsta.Service.Services.PP.Game;
using PPAsta.Service.Services.PP.Seller;
using PPAsta.Service.Services.PP.SellerGame;
using PPAsta.Service.Services.PP.Seller;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.ViewModels
{
    public class SellerHandleViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvGameService _gameService;
        private readonly ISrvSellerGameService _sellerGameService;
        private readonly ISrvSellerService _sellerService;
        private readonly ISrvSellerService _sellerService;

        public SellerHandleViewModel(ISrvGameService gameService, ISrvSellerGameService sellerGameService, ISrvSellerService sellerService, ISrvSellerService sellerService)
        {
            _gameService = gameService;
            _sellerGameService = sellerGameService;
            _sellerService = sellerService;
            _sellerService = sellerService;
        }

        private IEnumerable<SrvSellerGameDetail> _gamesList = new List<SrvSellerGameDetail>();
        public ObservableCollection<SrvSellerGameDetail> Games { get; } = new ObservableCollection<SrvSellerGameDetail>();
        public List<SrvSellerGameDetail> SelectedGames { get; } = new();

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected!, value);
        }

        private string _owner = "";
        public string? Owner
        {
            get => _owner;
            set => SetProperty(ref _owner!, value);
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

        public async Task LoadDataAsync(SrvSellerDetail sellerDetail)
        {
            ClearData();

            Owner = sellerDetail.BuyerName;
            Year = sellerDetail.Year.ToString();
            TotalPrice = sellerDetail.TotalPurchasePrice.ToString();

            _buyerId = sellerDetail.BuyerId;

            await LoadGamesAsync();
        }

        public void ClearData()
        {
            TextResearch = string.Empty;
            TotalRecordsText = string.Empty;
            PageText = string.Empty;
            Owner = string.Empty;
            Year = string.Empty;
            TotalPrice = string.Empty;
            _buyerId = 0;


            _gamesList = new List<SrvSellerGameDetail>();
        }

        public void ClearPartialData()
        {
            _gamesList = new List<SrvSellerGameDetail>();
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
            var gamesChecked = Games.Where(x => x.IsSelected && x.SellerProcess == SellerGameProcess.ToBePaid);
            if (gamesChecked.Any())
            {
                return (int)gamesChecked.Sum(x => x.PurchasePrice)!;
            }

            return null;
        }

        public async Task SellerGamesAsync(int sellerType)
        {
            var checkedGames = Games.Where(x => x.IsSelected && x.SellerProcess == SellerGameProcess.ToBePaid);
            if (checkedGames.Any())
            {
                var sellerGames = await _sellerGameService.GetSellerGameAsyncByGameIdsAsync(checkedGames.Select(x => x.Id));

                foreach (var sellerGame in sellerGames)
                {
                    sellerGame.SellerType = (SellerType)sellerType;
                    sellerGame.SellerProcess = SellerGameProcess.Paid;
                }

                await _sellerGameService.BulkUpdateSellerGameAsync(sellerGames);

                var sellers = (await _sellerService.GetSellerByPayementGameIdsAsync(sellerGames.Select(x => x.Id)))
                .Where(x => x.SellerSellerProcess == SellerSellerProcess.NotPaid)
                .ToList();

                if (sellers.Count > 0)
                {
                    sellers.ForEach(s => s.SellerSellerProcess = SellerSellerProcess.PaidByBuyer);
                    await _sellerService.UpdateSellersAsync(sellers);
                }

                await PartialReloadGamesAsync();

                var seller = await _sellerService.GetSellerByBuyerIdAsync(_buyerId);
                var notPaidedGames = Games.Where(x => x.SellerProcess == SellerGameProcess.ToBePaid);

                if (notPaidedGames.Any())
                {
                    seller.SellerProcess = SellerProcess.NotFullyPaid;
                }
                else
                {
                    seller.SellerProcess = SellerProcess.Paid;
                }

                await _sellerService.UpdateSellerAsync(seller);
            }
        }

        private async Task<IEnumerable<SrvSellerGameDetail>> GetGamesFilteredAsync()
        {
            IEnumerable<SrvSellerGameDetail> gamesTemp = null;

            if (!_gamesList.Any())
            {
                _gamesList = await _gameService.GetAllSellerGameDetailsByOwnerAsync(_buyerId);
            }

            var predicate = BuildPredicate();
            gamesTemp = _gamesList.Where(predicate).ToList();
            _count = gamesTemp.Count();

            TotalRecordsText = $"Records: {_count}";
            PageText = $"Pagina: {_page + 1} di {(_count / 50) + 1}";

            return gamesTemp;
        }

        private Func<SrvSellerGameDetail, bool> BuildPredicate()
        {
            return e => e.Name.ToLower().Contains(_textResearch.ToLower())
                        || e.Owner.ToLower().Contains(_textResearch.ToLower());
        }

        public int GamesCountAsync()
        {
            return _count;
        }

        private void RecordsPagination(IEnumerable<SrvSellerGameDetail> gamesTemp)
        {
            int skip = _page * 50;
            gamesTemp = gamesTemp.Skip(skip).Take(50);

            BindGrid(gamesTemp);
        }

        private void BindGrid(IEnumerable<SrvSellerGameDetail> gamesTemp)
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
                case "SellerProcess":
                    gamesTemp = isAscending
                        ? gamesTemp.OrderBy(x => x.SellerProcess).ToList()
                        : gamesTemp.OrderByDescending(x => x.SellerProcess).ToList();
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

        public async Task DeleteSellerAsync(SrvSellerGameDetail gameDetail)
        {
            var sellerGame = await _sellerGameService.GetSellerGameAsyncByGameIdAsync(gameDetail.Id);
            sellerGame.SellerProcess = SellerGameProcess.ToBePaid;
            sellerGame.SellerType = null;

            await _sellerGameService.UpdateSellerGameAsync(sellerGame);

            var sellers = (await _sellerService.GetSellerByPayementGameIdsAsync([sellerGame.Id]))
                .Where(x => x.SellerSellerProcess == SellerSellerProcess.PaidByBuyer)
                .ToList();

            if (sellers.Count > 0)
            {
                sellers.ForEach(s => s.SellerSellerProcess = SellerSellerProcess.NotPaid);
                await _sellerService.UpdateSellersAsync(sellers);
            }

            await PartialReloadGamesAsync();

            var seller = await _sellerService.GetSellerByBuyerIdAsync(_buyerId);
            var gamePayed = Games.Where(x => x.SellerProcess == SellerGameProcess.Paid);

            if (gamePayed.Any())
            {
                seller.SellerProcess = SellerProcess.NotFullyPaid;
            }
            else
            {
                seller.SellerProcess = SellerProcess.ToBePaid;
            }

            await _sellerService.UpdateSellerAsync(seller);
        }

        public async Task RemoveGameFromBuyerAsync(SrvSellerGameDetail gameDetail)
        {
            var sellerGame = await _sellerGameService.GetSellerGameAsyncByGameIdAsync(gameDetail.Id);
            sellerGame.SellerProcess = SellerGameProcess.Insert;
            sellerGame.SellerType = null;
            sellerGame.BuyerId = null;
            sellerGame.PurchasePrice = null;
            sellerGame.ShareOwner = null;
            sellerGame.SharePP = null;

            await _sellerGameService.UpdateSellerGameAsync(sellerGame);
            await _sellerService.DeleteSellerByPayementGameIdAsync(sellerGame.Id);

            await PartialReloadGamesAsync();

            var seller = await _sellerService.GetSellerByBuyerIdAsync(_buyerId);

            if (Games.Any())
            {
                var gamePayed = Games.Where(x => x.SellerProcess == SellerGameProcess.ToBePaid);

                if (gamePayed.Any())
                {
                    seller.SellerProcess = SellerProcess.NotFullyPaid;
                }
                else
                {
                    seller.SellerProcess = SellerProcess.ToBePaid;
                }

                await _sellerService.UpdateSellerAsync(seller);
                await _sellerService.HandleSellerAsync(_buyerId);
            }
            else
            {
                await _sellerService.DeleteSellerAsync(seller);
            }
        }
    }
}
