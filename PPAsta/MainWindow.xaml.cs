using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Pages;
using PPAsta.Service.Services.Google;
using PPAsta.Service.Services.Windows;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PPAsta
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, IForServiceCollectionExtension
    {
        private readonly IServiceProvider _serviceProvider;

        private bool _isWindowLoaded = false;

        public MainWindow(ISrvMainWindowService service, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;

            this.Activated += MainWindow_Activated;
        }

        private async void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (!_isWindowLoaded)
            {
                _isWindowLoaded = true;
                this.Activated -= MainWindow_Activated; // Rimuovi l'event handler

                var service = _serviceProvider.GetRequiredService<ISrvSpreadsheetService>();
                await service.ImportFromGoogleSpreadsheetToDatabaseAsync();

                var gamesPage = _serviceProvider.GetRequiredService<GamesPage>();
                ContentFrame.Content = gamesPage;
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is NavigationViewItem item &&
                item.Tag is string tag)
            {
                switch (tag)
                {
                    case "gamesPage":
                        var tablesPage = _serviceProvider.GetRequiredService<GamesPage>();
                        ContentFrame.Content = tablesPage;
                        break;
                    //case "usersPage":
                    //    var usersPage = _serviceProvider.GetRequiredService<UsersPage>();
                    //    ContentFrame.Content = usersPage;
                    //    break;
                    //case "eventsPage":
                    //    var eventsPage = _serviceProvider.GetRequiredService<EventsPage>();
                    //    ContentFrame.Content = eventsPage;
                    //    break;
                    //case "ordersPage":
                    //    var ordersPage = _serviceProvider.GetRequiredService<OrdersPage>();
                    //    ContentFrame.Content = ordersPage;
                    //    break;
                    //case "settingsPage":
                    //    var settingsPage = _serviceProvider.GetRequiredService<SettingsPage>();
                    //    ContentFrame.Content = settingsPage;
                    //    break;
                }
            }
        }

        private async Task LoadConfigurationsAsync()
        {
            try
            {
                // TODO: Da fare
                //var helperService = _serviceProvider.GetRequiredService<ISrvHelperService>();
                //await helperService.LoadConfigurationAsync();
            }
            catch (Exception ex)
            {
                //await ShowErrorAlertAsync(ex);
            }
        }
    }
}
