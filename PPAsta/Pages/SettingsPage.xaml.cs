using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PPAsta.Abstraction.Models.Interfaces;
using Microsoft.Extensions.Logging;
using PPAsta.ViewModels;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PPAsta.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page, IForServiceCollectionExtension
    {
        private readonly ILogger<SettingsPage> _logger;
        private SettingViewModel _settingViewModel;
        private ContentDialog _currentDialog;

        public SettingsPage(ILogger<SettingsPage> logger, SettingViewModel settingViewModel)
        {
            this.InitializeComponent();
            this.DataContext = settingViewModel;
            _logger = logger;
            _settingViewModel = (SettingViewModel)DataContext;
        }

        private async void LoadDataDatabaseButton_Click(object sender, RoutedEventArgs e)
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
                Name = "OnlineAdd",
                Content = "Inserimento CSV Online",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            singleAddButton.Click += OnlineAddButton_Click;

            var multipleAddButton = new Button
            {
                Name = "FileAdd",
                Content = "Inserimento CSV da file",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            multipleAddButton.Click += FileAddButton_Click;

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

        private async void OnlineAddButton_Click(object sender, RoutedEventArgs e)
        {
            var textBox = new TextBox
            {
                PlaceholderText = "Inserisci url",
                Margin = new Thickness(0, 0, 0, 10)
            };

            var checkBox = new CheckBox
            {
                Content = "Cancella i dati precedenti",
                Margin = new Thickness(0, 0, 0, 10)
            };

            var panel = new StackPanel();
            panel.Children.Add(textBox);
            panel.Children.Add(checkBox);

            var dialog = new ContentDialog
            {
                Title = "Inserimento dati",
                Content = panel,
                CloseButtonText = "Annulla",
                PrimaryButtonText = "Conferma",
                XamlRoot = this.Content.XamlRoot 
            };

            void ValidateFields()
            {
                bool isValid = !string.IsNullOrEmpty(textBox.Text) &&
                    textBox.Text.Contains("http");

                dialog.IsPrimaryButtonEnabled = isValid;
            }

            dialog.Loaded += (s, e) => ValidateFields();

            textBox.TextChanged += (s, e) => ValidateFields();

            var result = await ShowDialogSafeAsync(dialog);

            if (result == ContentDialogResult.Primary)
            {
                bool isChecked = checkBox.IsChecked == true;

                await _settingViewModel.OnlineInsertDataAsync(textBox.Text, isChecked);
            }
        }

        private async void FileAddButton_Click(object sender, RoutedEventArgs e)
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
