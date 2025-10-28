using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Services.PP.Game;
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

        private string _textResearch = "";
        private string? _totalRecordsText;
        private string? _pageText;
        private IEnumerable<SrvGame> _gamesList = new List<SrvGame>();

        public IAsyncRelayCommand LoadGamesCommand { get; }

        public int _page = 0;
        public int _count = -1;

        public ObservableCollection<SrvGame> Games { get; } = new ObservableCollection<SrvGame>();

        public GameViewModel(ISrvGameService gameService)
        {
            _gameService = gameService;

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

        private async Task<IEnumerable<SrvGame>> GetGamesFilteredAsync()
        {
            IEnumerable<SrvGame> gamesTemp = null;

            if (!_gamesList.Any())
            {
                _gamesList = await _gameService.GetAllGamesAsync();
            }

            var predicate = BuildPredicate();
            gamesTemp = _gamesList.Where(predicate).ToList();
            _count = gamesTemp.Count();

            TotalRecordsText = $"Records: {_count}";
            PageText = $"Pagina: {_page + 1} di {(_count / 50) + 1}";

            return gamesTemp;
        }

        private Func<SrvGame, bool> BuildPredicate()
        {
            return e => e.Name.ToLower().Contains(_textResearch.ToLower())
                        || e.Owner.ToLower().Contains(_textResearch.ToLower());
        }

        public int GamesCountAsync()
        {
            return _count;
        }

        private void RecordsPagination(IEnumerable<SrvGame> gamesTemp)
        {
            int skip = _page * 50;
            gamesTemp = gamesTemp.Skip(skip).Take(50);

            BindGrid(gamesTemp);
        }

        private void BindGrid(IEnumerable<SrvGame> gamesTemp)
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

                var ordersTemp = await GetGamesFilteredAsync();
                RecordsPagination(ordersTemp);
            }
        }

        public async Task NextButton()
        {
            if (_count - (50 + (_page * 50)) > 0)
            {
                _page++;

                var ordersTemp = await GetGamesFilteredAsync();
                RecordsPagination(ordersTemp);
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
                //case "StartPrice":
                //    gamesTemp = isAscending
                //        ? gamesTemp.OrderBy(x => x.StartPrice).ToList()
                //        : gamesTemp.OrderByDescending(x => x.StartPrice).ToList();
                //    break;
                //case "EndPrice":
                //    gamesTemp = isAscending
                //        ? gamesTemp.OrderBy(x => x.EndPrice).ToList()
                //        : gamesTemp.OrderByDescending(x => x.EndPrice).ToList();
                //    break;
                //case "Year":
                //    gamesTemp = isAscending
                //        ? gamesTemp.OrderBy(x => x.Year).ToList()
                //        : gamesTemp.OrderByDescending(x => x.Year).ToList();
                //    break;
                //case "IsSell":
                //    gamesTemp = isAscending
                //        ? gamesTemp.OrderBy(x => x.IsSell).ToList()
                //        : gamesTemp.OrderByDescending(x => x.IsSell).ToList();
                //    break;
                default:
                    break;
            }

            RecordsPagination(gamesTemp); 
        }
    }
}
