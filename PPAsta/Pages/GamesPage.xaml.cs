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
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Storages.PP;
using PPAsta.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PPAsta.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamesPage : Page, IForServiceCollectionExtension
    {
        private readonly ILogger<GamesPage> _logger;
        public GamesPage(GameViewModel gameViewModel, ILogger<GamesPage> logger)
        {
            InitializeComponent();
            this.DataContext = gameViewModel;
            _logger = logger;

            LoadComponents();
        }

        private void LoadComponents()
        {
            if (SrvAppConfigurationStorage.DatabaseConfiguration.DatabaseExists)
            {
                LoadOrdersAsync();
                OrdersCount();
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
            }
        }

        private async void LoadOrdersAsync()
        {
            try
            {
                var gameViewModel = (GameViewModel)DataContext;
                await gameViewModel.LoadGamesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
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
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                LoadOrdersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void OrdersCount()
        {
            try
            {
                var gameViewModel = (GameViewModel)DataContext;
                gameViewModel.GamesCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async void RowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Da fare
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                if (e.Row.DataContext is SrvGameDetail gameDetail)
                {
                    _logger.LogInformation($"LoadingRow per table {gameDetail.Id}");

                    if (gameDetail.PaymentProcess == PaymentProcess.Paid)
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Green);
                    }
                    else if (gameDetail.PaymentProcess == PaymentProcess.ToBePaid)
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Transparent);
                    }
                }
                else
                {
                    _logger.LogWarning("DataContext non è di tipo SrvGameDetail");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore in LoadingRow");
            }
        }
    }
}
