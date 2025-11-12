using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PPAsta.Abstraction.Models.Enums;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Buyer;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Storages.PP;
using PPAsta.Services.Navigation;
using PPAsta.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PPAsta.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamesPage : Page, IForServiceCollectionExtension, INavigationAware
    {
        private readonly ILogger<GamesPage> _logger;
        private ContentDialog _currentDialog;
        private readonly INavigationService _navigationService;

        public GamesPage(GameViewModel gameViewModel, ILogger<GamesPage> logger, INavigationService navigationService)
        {
            InitializeComponent();
            this.DataContext = gameViewModel;
            _logger = logger;

            LoadComponents();
            _navigationService = navigationService;
        }

        public async void OnNavigatedTo(object parameter)
        {
            try
            {
                GameViewModel gameViewModel = (GameViewModel)DataContext;
                await gameViewModel.ReloadGamesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private void LoadComponents()
        {
            if (SrvAppConfigurationStorage.DatabaseConfiguration.DatabaseExists)
            {
                LoadGamesAsync();
                GamesCount();
            }
            else
            {
                this.Loaded += TablesPage_Loaded;
            }
        }

        private void TablesPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= TablesPage_Loaded;
            ShowDatabaseNotFoundAlertAsync();
        }

        private async void ShowDatabaseNotFoundAlertAsync()
        {
            var dialog = new ContentDialog
            {
                Title = "Database non trovato",
                Content = "Il database non esiste.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            try
            {
                await dialog.ShowAsync();
                _logger.LogWarning("Database non trovato - alert mostrato all'utente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la visualizzazione dell'alert");
                await ExceptionDialogAsync(ex);
            }
        }

        private async void LoadGamesAsync()
        {
            try
            {
                GameViewModel gameViewModel = (GameViewModel)DataContext;
                await gameViewModel.LoadGamesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var gameViewModel = (GameViewModel)DataContext;
                await gameViewModel.PrevButton();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var gameViewModel = (GameViewModel)DataContext;
                await gameViewModel.NextButton();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            try
            {
                var gameViewModel = (GameViewModel)DataContext;
                await gameViewModel.LoadGamesAsync();

                string propertyName = e.Column.Tag?.ToString();

                if (!string.IsNullOrEmpty(propertyName))
                {
                    bool isAscending = e.Column.SortDirection != DataGridSortDirection.Ascending;
                    e.Column.SortDirection = isAscending ? DataGridSortDirection.Ascending : DataGridSortDirection.Descending;

                    foreach (var column in ((DataGrid)sender).Columns)
                    {
                        if (column != e.Column)
                        {
                            column.SortDirection = null;
                        }
                    }

                    await gameViewModel.DataSortAsync(propertyName, isAscending);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                LoadGamesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void GamesCount()
        {
            try
            {
                var gameViewModel = (GameViewModel)DataContext;
                gameViewModel.GamesCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                
                if (button?.Tag != null)
                {
                    var gameViewModel = (GameViewModel)DataContext;
                    gameViewModel.ClearData();

                    var game = button.Tag as SrvGameDetail;
                    _navigationService.NavigateTo<PaymentGamesPage>(game);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async Task ExceptionDialogAsync(Exception ex)
        {
            var errorText = ex.Message;

            var dialog = new ContentDialog
            {
                Title = "Errore",
                CloseButtonText = "Ok",
                XamlRoot = XamlRoot,
                Content = new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        new TextBlock { Text = errorText }
                    }
                }
            };

            await ShowDialogSafeAsync(dialog);
        }

        private async Task<ContentDialogResult> ShowDialogSafeAsync(ContentDialog dialog)
        {
            try
            {
                _currentDialog?.Hide();

                // Aspetta un momento per permettere al dialog precedente di chiudersi
                await Task.Delay(100);

                _currentDialog = dialog;
                return await _currentDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore dialog");
                return ContentDialogResult.None;
            }
        }
    }
}
