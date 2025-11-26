using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Control;
using PPAsta.Service.Models.PP.Buyer;
using PPAsta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Service.Services.PP.Buyer
{
    public interface IBuyerSelectionService : IForServiceCollectionExtension
    {
        Task<SrvBuyer> SelectBuyerAsync(XamlRoot xamlRoot, int year);
    }

    public class BuyerSelectionService : IBuyerSelectionService
    {
        private readonly IServiceProvider _serviceProvider;

        public BuyerSelectionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<SrvBuyer> SelectBuyerAsync(XamlRoot xamlRoot, int year)
        {
            var dialog = new ContentDialog
            {
                Title = "Seleziona Acquirente",
                PrimaryButtonText = "Seleziona",
                CloseButtonText = "Annulla",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            // Crea il controllo e il ViewModel
            var buyersControl = new BuyersControl
            {
                SelectionMode = DataGridSelectionMode.Single
            };

            var viewModel = _serviceProvider.GetRequiredService<BuyerViewModel>();
            buyersControl.DataContext = viewModel;

            // Carica i buyers
            viewModel.LoadComboBoxYears(year);
            await viewModel.LoadBuyersAsync();

            dialog.Content = buyersControl;

            // Gestisci doppio click per chiudere il dialog
            buyersControl.BuyerSelected += (s, buyer) =>
            {
                dialog.Hide();
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && buyersControl.SelectedBuyer != null)
            {
                return buyersControl.SelectedBuyer;
            }

            return null;
        }
    }
}

