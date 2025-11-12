using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Models.PP.Buyer;
using PPAsta.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PPAsta.Control
{
    public sealed partial class BuyersControl : UserControl, IForServiceCollectionExtension
    {
        public event EventHandler<SrvBuyer> BuyerSelected;

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                nameof(SelectionMode),
                typeof(DataGridSelectionMode),
                typeof(BuyersControl),
                new PropertyMetadata(DataGridSelectionMode.Single));

        public static readonly DependencyProperty SelectedBuyerProperty =
            DependencyProperty.Register(
                nameof(SelectedBuyer),
                typeof(SrvBuyer),
                typeof(BuyersControl),
                new PropertyMetadata(null));

        public DataGridSelectionMode SelectionMode
        {
            get => (DataGridSelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        public SrvBuyer SelectedBuyer
        {
            get => (SrvBuyer)GetValue(SelectedBuyerProperty);
            set => SetValue(SelectedBuyerProperty, value);
        }

        public BuyersControl()
        {
            InitializeComponent();
        }

        private async void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is BuyerViewModel viewModel)
            {
                await viewModel.LoadBuyersAsync();
            }
        }

        private void DataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (SelectedBuyer != null)
            {
                BuyerSelected?.Invoke(this, SelectedBuyer);
            }
        }
    }
}
