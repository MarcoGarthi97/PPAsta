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
using PPAsta.Service.Services.PP.Buyer;
using PPAsta.Service.Services.PP.Game;
using PPAsta.Service.Services.PP.Helper;

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
        public static Window? MainWindow { get; private set; }

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

            try
            {
                await InizializeServiceAsync(_host.Services);

                var mainWindowService = _host.Services.GetRequiredService<ISrvMainWindowService>();
                MainWindow = new MainWindow(mainWindowService, _host.Services);
                //MainWindow.Closed += MainWindow_Closed;

                MainWindow.Closed += (s, e) =>
                {
                    MainWindow = null;
                    //(_host as IDisposable)?.Dispose();
                };

                MainWindow.Activate();
            }
            catch (Exception ex)
            {

            }
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddAutoMapper(x => { }, typeof(App), typeof(SrvVersionService));

            services.AddSharedLibrary();
            services.AddSharedLibraryServices();
            services.AddSharedLibraryRepositories();
            services.AddSharedLibraryMigrations();

            //string dbPath = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "app.db");
            string dbPath = GetPathDB();
            var connectionString = $"Data Source={dbPath}";
            services.AddSingleton<IDatabaseConnectionFactory>(provider =>
                new MdlSqliteConnectionFactory(connectionString));
        }

        private string GetPathDB()
        {
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // 2. Definisci una sottocartella specifica per la tua applicazione (usa un nome unico, ad es. "PPAsta")
            string appFolderName = "PPAsta";
            string appDataPath = System.IO.Path.Combine(localAppDataPath, appFolderName);

            // 3. Verifica e crea la cartella se non esiste
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            // 4. Combina il percorso della cartella con il nome del file DB
            string dbPath = System.IO.Path.Combine(appDataPath, "app.db");

            return dbPath;
        }

        private async Task InizializeServiceAsync(IServiceProvider services)
        {
            await InizializeDatabase();

            var migrationService = services.GetRequiredService<IMigrationOrchestrator>();
            await migrationService.ExecuteMigrationAsync();

            await LoadConfigurationsAsync(services);
        }

        private async Task LoadConfigurationsAsync(IServiceProvider services)
        {
            SrvAppConfigurationStorage.SetDatabaseExist();

            var gameService = services.GetRequiredService<ISrvGameService>();
            SrvAppConfigurationStorage.SetOldestYear(await gameService.GetOldestYearAsync());

            var helperService = services.GetRequiredService<ISrvHelperService>();
            var helperInitializeYear = await helperService.GetHelperByKeyAsync("InitializeYear");
            SrvAppConfigurationStorage.SetInitializeYearNow(helperInitializeYear?.Json);
        }

        private async Task InizializeDatabase()
        {
            string dbFilePath = GetPathDB();

            if (!File.Exists(dbFilePath))
            {
                using (var stream = File.Create(dbFilePath))
                {
                }
            }

            SrvAppConfigurationStorage.SetDatabasePath(System.IO.Path.GetDirectoryName(dbFilePath));

            ConfigurationDatabase();
        }

        private void ConfigurationDatabase()
        {
            RepositoryCollectionExtension.ConfigurationDatabase();
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            MainWindow = null;
        }
    }
}
