using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Seller;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PPAsta.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SellersPage : Page, IForServiceCollectionExtension, INavigationAware
    {
        private readonly ILogger<SellersPage> _logger;
        private ContentDialog _currentDialog;
        private SellerViewModel _sellerViewModel;
        private readonly INavigationService _navigationService;

        public SellersPage(SellerViewModel sellerViewModel, ILogger<SellersPage> logger, INavigationService navigationService)
        {
            InitializeComponent();
            this.DataContext = sellerViewModel;
            _logger = logger;
            _sellerViewModel = (SellerViewModel)DataContext;

            LoadComponents();
            _navigationService = navigationService;
        }

        public async void OnNavigatedTo(object parameter)
        {
            try
            {
                await _sellerViewModel.ReloadSellersAsync();
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
                LoadSellersAsync();
                SellersCount();
            }
            else
            {
                this.Loaded += TablesPage_Loaded;
            }
        }
        private async void ComboBoxYears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LoadSellersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private void TablesPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= TablesPage_Loaded;
            ShowDatabaseNotFoundAlertAsync();
        }

        private async void ShowDatabaseNotFoundAlertAsync()
        {
            _currentDialog = new ContentDialog
            {
                Title = "Database non trovato",
                Content = "Il database non esiste.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            try
            {
                await _currentDialog.ShowAsync();
                _logger.LogWarning("Database non trovato - alert mostrato all'utente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la visualizzazione dell'alert");
            }
        }

        private async void LoadSellersAsync()
        {
            try
            {
                await _sellerViewModel.LoadSellersAsync();
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
                await _sellerViewModel.PrevButton();
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
                await _sellerViewModel.NextButton();
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
                await _sellerViewModel.LoadSellersAsync();

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

                    await _sellerViewModel.DataSortAsync(propertyName, isAscending);
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
                LoadSellersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void SellersCount()
        {
            try
            {
                _sellerViewModel.SellersCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void SellerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;

                if (button?.Tag != null)
                {
                    var sellerDetail = button.Tag as SrvSellerDetail;
                    _navigationService.NavigateTo<SellerHandlePage>(sellerDetail!);
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

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentDialog != null && this.XamlRoot != null)
                {
                    _currentDialog = null;
                }
                _currentDialog = null;
                this.Unloaded -= Page_Unloaded;
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<ContentDialogResult> ShowDialogSafeAsync(ContentDialog dialog)
        {
            try
            {
                _currentDialog?.Hide();

                // Aspetta un momento per permettere al dialog precedente di chiudersi
                await Task.Delay(300);

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
