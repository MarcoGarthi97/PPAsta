using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Buyer;
using PPAsta.Service.Services.PP.Buyer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.ViewModels
{
    public class BuyerViewModel : ObservableObject, IForServiceCollectionExtension
    {
        private readonly ISrvBuyerService _buyerService;

        private string _textResearch = "";
        private string? _totalRecordsText;
        private string? _pageText;
        private List<SrvBuyer> _buyersList = new List<SrvBuyer>();

        public IAsyncRelayCommand LoadBuyersCommand { get; }

        public int _page = 0;
        public int _count = -1;

        public ObservableCollection<SrvBuyer> Buyers { get; } = new ObservableCollection<SrvBuyer>();
        public BuyerViewModel(ISrvBuyerService buyerService)
        {
            _buyerService = buyerService;

            LoadBuyersCommand = new AsyncRelayCommand(LoadBuyersAsync);
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

        public async Task LoadBuyersAsync()
        {
            var buyersTemp = await GetBuyersFilteredAsync();
            RecordsPagination(buyersTemp);
        }

        private async Task<IEnumerable<SrvBuyer>> GetBuyersFilteredAsync()
        {
            IEnumerable<SrvBuyer> buyersTemp = null;

            if (!_buyersList.Any())
            {
                _buyersList = await _buyerService.GetAllBuyersAsync();

                _buyersList = _buyersList.OrderByDescending(x => x.Year)
                         .ThenBy(x => x.Number)
                         .ToList();
            }

            var predicate = BuildPredicate();
            buyersTemp = _buyersList.Where(predicate).ToList();
            _count = buyersTemp.Count();

            TotalRecordsText = $"Records: {_count}";
            PageText = $"Pagina: {_page + 1} di {(_count / 50) + 1}";

            return buyersTemp;
        }

        private Func<SrvBuyer, bool> BuildPredicate()
        {
            return e => e.Name.ToLower().Contains(_textResearch.ToLower())
                        || e.Number.ToString().Contains(_textResearch)
                        || e.Year.ToString().Contains(_textResearch);
        }

        public int BuyersCountAsync()
        {
            return _count;
        }

        private void RecordsPagination(IEnumerable<SrvBuyer> buyersTemp)
        {
            int skip = _page * 50;
            buyersTemp = buyersTemp.Skip(skip).Take(50);

            BindGrid(buyersTemp);
        }

        private void BindGrid(IEnumerable<SrvBuyer> buyersTemp)
        {
            Buyers.Clear();

            foreach (var x in buyersTemp)
            {
                Buyers.Add(x);
            }
        }

        public async Task PrevButton()
        {
            if (_page > 0)
            {
                _page--;

                var buyersTemp = await GetBuyersFilteredAsync();
                RecordsPagination(buyersTemp);
            }
        }

        public async Task NextButton()
        {
            if (_count - (50 + (_page * 50)) > 0)
            {
                _page++;

                var buyersTemp = await GetBuyersFilteredAsync();
                RecordsPagination(buyersTemp);
            }
        }

        public async Task DataSortAsync(string propertyName, bool isAscending)
        {
            var buyersTemp = await GetBuyersFilteredAsync();

            switch (propertyName)
            {
                case "Name":
                    buyersTemp = isAscending
                        ? buyersTemp.OrderBy(x => x.Name).ToList()
                        : buyersTemp.OrderByDescending(x => x.Name).ToList();
                    break;
                case "Number":
                    buyersTemp = isAscending
                        ? buyersTemp.OrderBy(x => x.Number).ToList()
                        : buyersTemp.OrderByDescending(x => x.Number).ToList();
                    break;
                case "Year":
                    buyersTemp = isAscending
                        ? buyersTemp.OrderBy(x => x.Year).ToList()
                        : buyersTemp.OrderByDescending(x => x.Year).ToList();
                    break;
                default:
                    break;
            }

            RecordsPagination(buyersTemp);
        }

        public async Task InsertBuyerAsync(SrvBuyer buyer)
        {
            if (_buyersList.Where(x => x.Year == buyer.Year && x.Number == buyer.Number).Count() < 1)
            {
                await _buyerService.InsertBuyerAsync(buyer);
                _buyersList = new List<SrvBuyer>();

                await LoadBuyersAsync();
            }
            else
            {
                throw new Exception("Esiste già un compratore che ha il numero: " + buyer.Number + " nell'anno " + buyer.Year);
            }

            await LoadBuyersAsync();
        }

        public async Task InsertBuyersAsync(string content)
        {
            var contentBuyers = _buyerService.GetContentByCSV(content);

            var dictContentBuyers = contentBuyers.ToDictionary(x => x.Year + "_" + x.Number, x => x);
            var hashBuyers = _buyersList.Select(x => x.Year + "_" + x.Number).ToHashSet();
            
            var errors = new List<SrvBuyer>();
            var insert = new List<SrvBuyer>();

            foreach (var item in dictContentBuyers)
            {
                if (!hashBuyers.Contains(item.Key))
                {
                    insert.Add(item.Value);
                }
                else
                {
                    errors.Add(item.Value);
                }
            }

            if (insert.Any())
            {
                await _buyerService.InsertBuyersAsync(insert);
                _buyersList = new List<SrvBuyer>();

                await LoadBuyersAsync();
            }

            if (errors.Any())
            {
                string errorMessage = "";

                foreach (var error in errors)
                {
                    errorMessage += "\n" + "Esiste già un compratore che ha il numero: " + error.Number + " nell'anno " + error.Year;
                }

                throw new Exception(errorMessage);
            }
        }

        public async Task UpdateBuyerAsync(SrvBuyer buyer)
        {
            var indexToEdit = _buyersList.FindIndex(x => x.Id == buyer.Id);
            if(indexToEdit > -1)
            {
                if (_buyersList.Where(x => x.Year == buyer.Year && x.Number == buyer.Number).Count() < 1)
                {
                    await _buyerService.UpdateBuyerAsync(buyer);
                    _buyersList[indexToEdit] = buyer;
                }
                else
                {
                    throw new Exception("Esiste già un compratore che ha il numero: " + buyer.Number + " nell'anno " + buyer.Year);
                }
            }

            await LoadBuyersAsync();
        }

        public async Task DeleteBuyerAsync(SrvBuyer buyer)
        {
            await _buyerService.DeleteBuyerAsync(buyer);

            _buyersList.Remove(buyer);
            await LoadBuyersAsync();
        }
    }
}
