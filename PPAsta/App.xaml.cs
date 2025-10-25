using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using PPAsta.Service.Services.Collection;
using PPAsta.Repository.Services.Collection;
using PPAsta.Services.Collection;
using PPAsta.Repository.Services.FactorySQL;
using PPAsta.Service.Storages.PP;
using PPAsta.Service.Services.PP.Version;
using PPAsta.Service.Services.Windows;
using PPAsta.Migration.Services.Collection;
using PPAsta.Migration.Services.Orchestrator;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PPAsta
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IConfiguration>(context.Configuration);
                    ConfigureServices(services, context.Configuration);
                })
                .Build();


            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder().Build();

            await InizializeServiceAsync(_host.Services);

            var mainWindowService = _host.Services.GetRequiredService<ISrvMainWindowService>();
            m_window = new MainWindow(mainWindowService, _host.Services);
            m_window.Activate();
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddAutoMapper(x => { }, typeof(App), typeof(SrvVersionService));

            services.AddSharedLibrary();
            services.AddSharedLibraryServices();
            services.AddSharedLibraryRepositories();
            services.AddSharedLibraryMigrations();

            string dbPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "app.db");
            var connectionString = $"Data Source={dbPath}";
            services.AddSingleton<IDatabaseConnectionFactory>(provider =>
                new MdlSqliteConnectionFactory(connectionString));

            LoadConfigurations(config);
        }

        private async Task InizializeServiceAsync(IServiceProvider services)
        {
            await InizializeDatabase();

            var migrationService = services.GetRequiredService<IMigrationOrchestrator>();
            await migrationService.ExecuteMigrationAsync();

            SrvAppConfigurationStorage.SetDatabaseExist();
        }

        private async Task InizializeDatabase()
        {
            var path = ApplicationData.Current.LocalFolder;
            await ApplicationData.Current.LocalFolder.CreateFileAsync("app.db", CreationCollisionOption.OpenIfExists);

            SrvAppConfigurationStorage.SetDatabasePath(path.Path);

            ConfigurationDatabase();
        }

        private void ConfigurationDatabase()
        {
            RepositoryCollectionExtension.ConfigurationDatabase();
        }

        private void LoadConfigurations(IConfiguration config)
        {
        }

        private Window? m_window;
    }
}
