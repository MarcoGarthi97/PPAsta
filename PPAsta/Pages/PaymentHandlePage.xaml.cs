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
using PPAsta.Service.Models.PP.Payment;
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
    public sealed partial class PaymentHandlePage : Page, IForServiceCollectionExtension, INavigationAware
    {
        private readonly ILogger<PaymentHandlePage> _logger;
        private readonly PaymentHandleViewModel _paymentHandleViewModel;
        private readonly INavigationService _navigationService;

        private ContentDialog _currentDialog;

        public PaymentHandlePage(ILogger<PaymentHandlePage> logger, PaymentHandleViewModel paymentHandleViewModel, INavigationService navigationService)
        {
            _logger = logger;
            this.DataContext = paymentHandleViewModel;
            _paymentHandleViewModel = (PaymentHandleViewModel)this.DataContext;
            _navigationService = navigationService;

            InitializeComponent();
        }

        private void DataGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;

            while (element != null)
            {
                if (element.DataContext is SrvGameDetail gameDetail)
                {
                    gameDetail.IsSelected = gameDetail.IsSelected ? false : true;
                }

                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
        }

        public async void OnNavigatedTo(object parameter)
        {
            try
            {
                if (parameter is SrvPaymentDetail dto)
                {
                    await _paymentHandleViewModel.LoadDataAsync(dto);
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
                await _paymentHandleViewModel.LoadGamesAsync();
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
                    var paymentDetail = button.Tag as SrvGameDetail;
                    await _paymentHandleViewModel.DeletePaymentAsync(paymentDetail!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;

                if (button?.Tag != null)
                {
                    var paymentDetail = button.Tag as SrvGameDetail;
                    await _paymentHandleViewModel.RemoveGameFromBuyerAsync(paymentDetail!);
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
                _paymentHandleViewModel.CheckAll();
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
                await _paymentHandleViewModel.PrevButton();
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
                await _paymentHandleViewModel.NextButton();
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
                await _paymentHandleViewModel.LoadGamesAsync();

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

                    await _paymentHandleViewModel.DataSortAsync(propertyName, isAscending);
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
                int? total = _paymentHandleViewModel.GetTotalGamesChecked();
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

                ComboBox paymentMethodComboBox = new ComboBox
                {
                    Header = "Metodo di pagamento",
                    PlaceholderText = "Seleziona metodo",
                    Width = 300
                };

                paymentMethodComboBox.Items.Add(new ComboBoxPP { DisplayName = "Contanti", Value = 0 });
                paymentMethodComboBox.Items.Add(new ComboBoxPP { DisplayName = "PayPal", Value = 1 });
                paymentMethodComboBox.Items.Add(new ComboBoxPP { DisplayName = "Bonifico", Value = 2 });
                paymentMethodComboBox.Items.Add(new ComboBoxPP { DisplayName = "Altro", Value = 3 });

                dialogContent.Children.Add(paymentMethodComboBox);

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
                    var selectedMethod = paymentMethodComboBox.SelectedItem as ComboBoxPP;

                    if (selectedMethod == null)
                    {
                        return;
                    }

                    await _paymentHandleViewModel.PaymentGamesAsync(selectedMethod.Value);
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
                _paymentHandleViewModel.GamesCountAsync();
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
                _paymentHandleViewModel.ClearData();
                _navigationService.NavigateTo<PaymentsPage>("");
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
