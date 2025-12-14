using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PPAsta.Abstraction.Models.Entities;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Models.PP.Seller;
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
    public sealed partial class SellerHandlePage : Page, IForServiceCollectionExtension, INavigationAware
    {
        private readonly ILogger<SellerHandlePage> _logger;
        private readonly SellerHandleViewModel _sellerHandleViewModel;
        private readonly INavigationService _navigationService;

        private ContentDialog _currentDialog;

        public SellerHandlePage(ILogger<SellerHandlePage> logger, SellerHandleViewModel sellerHandleViewModel, INavigationService navigationService)
        {
            _logger = logger;
            this.DataContext = sellerHandleViewModel;
            _sellerHandleViewModel = (SellerHandleViewModel)this.DataContext;
            _navigationService = navigationService;

            InitializeComponent();
        }

        private void DataGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;

            while (element != null)
            {
                if (element.DataContext is SrvSellerGameDetail sellerGameDetail)
                {
                    sellerGameDetail.IsSelected = sellerGameDetail.IsSelected ? false : true;
                }

                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
        }

        public async void OnNavigatedTo(object parameter)
        {
            try
            {
                if (parameter is SrvSellerDetail dto)
                {
                    await _sellerHandleViewModel.LoadDataAsync(dto);
                    GamesCount();
                }
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

        private async void LoadGamesAsync()
        {
            try
            {
                await _sellerHandleViewModel.LoadGamesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void DeletePaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;

                if (button?.Tag != null)
                {
                    var sellerDetail = button.Tag as SrvSellerGameDetail;
                    await _sellerHandleViewModel.DeletePaymentSellerAsync(sellerDetail!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _sellerHandleViewModel.CheckAll();
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
                await _sellerHandleViewModel.PrevButton();
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
                await _sellerHandleViewModel.NextButton();
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
                await _sellerHandleViewModel.LoadGamesAsync();

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

                    await _sellerHandleViewModel.DataSortAsync(propertyName, isAscending);
                }
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
                decimal? total = _sellerHandleViewModel.GetTotalGamesChecked();
                if (!total.HasValue)
                {
                    return;
                }

                StackPanel dialogContent = new StackPanel
                {
                    Spacing = 16
                };

                TextBox totalTextBox = new TextBox
                {
                    Header = "Totale",
                    Text = total.Value.ToString(),
                    IsReadOnly = true,
                    Width = 300,
                };
                dialogContent.Children.Add(totalTextBox);

                ComboBox sellerMethodComboBox = new ComboBox
                {
                    Header = "Metodo di pagamento",
                    PlaceholderText = "Seleziona metodo",
                    Width = 300
                };

                sellerMethodComboBox.Items.Add(new ComboBoxPP { DisplayName = "Contanti", Value = 0 });
                sellerMethodComboBox.Items.Add(new ComboBoxPP { DisplayName = "PayPal", Value = 1 });
                sellerMethodComboBox.Items.Add(new ComboBoxPP { DisplayName = "Bonifico", Value = 2 });
                sellerMethodComboBox.Items.Add(new ComboBoxPP { DisplayName = "Altro", Value = 3 });

                dialogContent.Children.Add(sellerMethodComboBox);

                ContentDialog dialog = new ContentDialog
                {
                    Title = "Pagamento",
                    Content = dialogContent,
                    PrimaryButtonText = "Conferma",
                    CloseButtonText = "Annulla",
                    XamlRoot = this.XamlRoot,
                    DefaultButton = ContentDialogButton.Primary
                };

                var result = await ShowDialogSafeAsync(dialog);

                if (result == ContentDialogResult.Primary)
                {
                    var selectedMethod = sellerMethodComboBox.SelectedItem as ComboBoxPP;

                    if (selectedMethod == null)
                    {
                        return;
                    }

                    await _sellerHandleViewModel.PaymentSellerAsync(selectedMethod.Value);
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
                _sellerHandleViewModel.GamesCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _sellerHandleViewModel.ClearData();
                _navigationService.NavigateTo<SellersPage>("");
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

