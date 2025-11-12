using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using PPAsta.Abstraction.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPAsta.Services.Navigation
{
    public interface INavigationAware
    {
        void OnNavigatedTo(object parameter);
    }

    public interface INavigationService : IForServiceCollectionExtension
    {
        void NavigateTo<T>(object parameter = null) where T : Page;
        Frame Frame { get; set; }
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        public Frame Frame { get; set; }

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<T>(object parameter = null) where T : Page
        {
            if (Frame == null)
                throw new InvalidOperationException("Frame non inizializzato");

            var page = _serviceProvider.GetRequiredService<T>();

            // Se la pagina implementa un'interfaccia per ricevere parametri
            if (parameter != null && page is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(parameter);
            }

            Frame.Content = page;
        }
    }
}
