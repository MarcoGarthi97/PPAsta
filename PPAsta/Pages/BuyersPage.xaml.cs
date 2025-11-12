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
using PPAsta.Service.Storages.PP;
using PPAsta.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PPAsta.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuyersPage : Page, IForServiceCollectionExtension
    {
        private readonly ILogger<BuyersPage> _logger;
        private BuyerViewModel _buyerViewModel; 
        private ContentDialog _currentDialog;

        public BuyersPage(BuyerViewModel buyerViewModel, ILogger<BuyersPage> logger)
        {
            InitializeComponent();
            this.DataContext = buyerViewModel;
            _logger = logger;
            _buyerViewModel = (BuyerViewModel)DataContext;

            LoadComponents();

            this.Unloaded += Page_Unloaded;
        }

        private void LoadComponents()
        {
            if (SrvAppConfigurationStorage.DatabaseConfiguration.DatabaseExists)
            {
                LoadBuyersAsync();
                BuyersCount();
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

        private async void LoadBuyersAsync()
        {
            try
            {
                var buyerViewModel = (BuyerViewModel)DataContext;
                await buyerViewModel.LoadBuyersAsync();
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
                var buyerViewModel = (BuyerViewModel)DataContext;
                await buyerViewModel.PrevButton();
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
                var buyerViewModel = (BuyerViewModel)DataContext;
                await buyerViewModel.NextButton();
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
                var buyerViewModel = (BuyerViewModel)DataContext;
                await buyerViewModel.LoadBuyersAsync();

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

                    await buyerViewModel.DataSortAsync(propertyName, isAscending);
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
                LoadBuyersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void BuyersCount()
        {
            try
            {
                var buyerViewModel = (BuyerViewModel)DataContext;
                buyerViewModel.BuyersCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await TypeOfInsertAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async Task TypeOfInsertAsync()
        {
            var singleAddButton = new Button
            {
                Name = "SingleAdd",
                Content = "Inserimento Singolo",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            singleAddButton.Click += SingleAddButton_Click;

            var multipleAddButton = new Button
            {
                Name = "MultipleAdd",
                Content = "Inserimento Multiplo CSV",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            multipleAddButton.Click += MultipleAddButton_Click;

            var dialog = new ContentDialog
            {
                Title = "Tipologia inserimento",
                PrimaryButtonText = "Conferma",
                CloseButtonText = "Annulla",
                XamlRoot = XamlRoot,
                Content = new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        singleAddButton,
                        multipleAddButton
                    }
                }
            };

            await ShowDialogSafeAsync(dialog);
        }

        private async void SingleAddButton_Click(object sender, RoutedEventArgs e)
        {
            await OpenBuyerDialogAsync();
        }

        private async void MultipleAddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var picker = new FileOpenPicker();

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

                picker.ViewMode = PickerViewMode.List;
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".csv");

                StorageFile file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    string content = await FileIO.ReadTextAsync(file);
                    await _buyerViewModel.InsertBuyersAsync(content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                
                if (button?.Tag != null)
                {
                    var buyer = button.Tag as SrvBuyer;
                    await OpenBuyerDialogAsync(buyer!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await ExceptionDialogAsync(ex);
            }
        }

        private async Task OpenBuyerDialogAsync(SrvBuyer? buyer = null)
        {
            var template = (DataTemplate)this.Resources["BuyerDialog"];

            var content = template.LoadContent() as StackPanel;

            var nameBox = content.Children[0] as TextBox;
            var numberBox = content.Children[1] as TextBox;
            var yearBox = content.Children[2] as TextBox;

            string title = "Aggiungi compratore";

            if (buyer != null)
            {
                nameBox.Text = buyer.Name;
                numberBox.Text = buyer.Number.ToString();
                yearBox.Text = buyer.Year.ToString();

                title = "Modifica compratore";
            }
            else
            {
                int year = DateTime.Now.Year;
                int number = await _buyerViewModel.GetNextNumberByYearAsync(year);

                yearBox.Text = year.ToString();
                numberBox.Text = number.ToString();
            }

            var dialog = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = "Conferma",
                CloseButtonText = "Annulla",
                XamlRoot = XamlRoot,
                Content = content
            };

            void ValidateFields()
            {
                bool isValid = !string.IsNullOrWhiteSpace(nameBox.Text) &&
                               !string.IsNullOrWhiteSpace(numberBox.Text) &&
                               Int32.TryParse(numberBox.Text, out int n) &&
                               n > 0 &&
                               !string.IsNullOrWhiteSpace(yearBox.Text) &&
                               Int32.TryParse(yearBox.Text, out int y) &&
                               y > 0;

                dialog.IsPrimaryButtonEnabled = isValid;
            }

            dialog.Loaded += (s, e) => ValidateFields();

            nameBox.TextChanged += (s, e) => ValidateFields();
            numberBox.TextChanged += (s, e) => ValidateFields();
            yearBox.TextChanged += (s, e) => ValidateFields();

            var result = await ShowDialogSafeAsync(dialog);

            if (result == ContentDialogResult.Primary)
            {
                var buyerTemp = new SrvBuyer(nameBox.Text, Int32.Parse(numberBox.Text), Int32.Parse(yearBox.Text));

                if (buyer != null)
                {
                    buyerTemp.Id = buyer.Id;
                    await _buyerViewModel.UpdateBuyerAsync(buyerTemp);
                }
                else
                {
                    await _buyerViewModel.InsertBuyerAsync(buyerTemp);
                }
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;

                if (button?.Tag != null)
                {
                    var buyer = button.Tag as SrvBuyer;
                    await OpenDeleteDialogAsync(buyer!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await ExceptionDialogAsync(ex);
            }
        }

        private async Task OpenDeleteDialogAsync(SrvBuyer buyer)
        {
            var dialog = new ContentDialog
            {
                Title = "Elimina Compratore",
                PrimaryButtonText = "Conferma",
                CloseButtonText = "Annulla",
                XamlRoot = XamlRoot,
                Content = new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        new TextBlock { Text = "Sei sicuro di voler eliminare il record selezionato?" }
                    }
                }
            };

            var result = await ShowDialogSafeAsync(dialog);

            if (result == ContentDialogResult.Primary)
            {
                await _buyerViewModel.DeleteBuyerAsync(buyer);
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
