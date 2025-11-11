using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Game;
using PPAsta.Service.Services.PP.Buyer;
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
    public sealed partial class PaymentGamesPage : Page, IForServiceCollectionExtension, INavigationAware
    {
        private readonly ILogger<PaymentGamesPage> _logger;
        private readonly PaymentGameViewModel _paymentGameViewModel;
        private readonly INavigationService _navigationService;
        private readonly IBuyerSelectionService _buyerSelectionService;

        private ContentDialog _currentDialog;


        public PaymentGamesPage(ILogger<PaymentGamesPage> logger, PaymentGameViewModel paymentGameViewModel, INavigationService navigationService, IBuyerSelectionService buyerSelectionService)
        {
            InitializeComponent();
            _logger = logger;
            this.DataContext = paymentGameViewModel;
            _paymentGameViewModel = (PaymentGameViewModel)DataContext;
            _navigationService = navigationService;
            _buyerSelectionService = buyerSelectionService;
        }

        public async void OnNavigatedTo(object parameter)
        {
            try
            {
                if (parameter is SrvGameDetail dto)
                {
                    _paymentGameViewModel.LoadData(dto);
                }
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
                _navigationService.NavigateTo<GamesPage>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void PurchasePrice_TextChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            try
            {
                if (sender is TextBox textBox)
                {
                    _paymentGameViewModel.CheckTextBoxForDigits(textBox.Text);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void SearchBuyer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var buyer = await _buyerSelectionService.SelectBuyerAsync(this.XamlRoot);

                if (buyer != null)
                {
                    _paymentGameViewModel.LoadBuyer(buyer);
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
