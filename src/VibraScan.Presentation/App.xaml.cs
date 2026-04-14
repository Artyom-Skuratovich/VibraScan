using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using VibraScan.Application;
using VibraScan.Infrastructure;
using VibraScan.Presentation.Services.Interfaces;
using VibraScan.Presentation.ViewModels;

namespace VibraScan.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = CreateConfiguration();

            services.AddSingleton(configuration);

            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices();
            services.AddPresentationServices();
        }

        private static IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var windowService = _serviceProvider.GetRequiredService<IWindowService>();
            windowService.Show<MigrationViewModel>();
        }
    }
}