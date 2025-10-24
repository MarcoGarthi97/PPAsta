using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PPAsta.Abstraction.Models.Interfaces;
using PPAsta.Service.Services.Google;
using PPAsta.Service.Services.Windows;
using PPAsta.Service.Storages.PP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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

                await service.GetGoogleSpreadsheetAsync(CancellationToken.None);
            }
        }
    }
}
