using Microsoft.Extensions.DependencyInjection;
using VibraScan.Infrastructure.Data;
using VibraScan.Presentation.Common;
using VibraScan.Presentation.Services;
using VibraScan.Presentation.Services.Interfaces;
using VibraScan.Presentation.ViewModels;
using VibraScan.Presentation.ViewModels.Models;
using VibraScan.Presentation.Views.Windows;

namespace VibraScan.Presentation
{
    public static class DependencyInjection
    {
        public static void AddPresentationServices(this IServiceCollection services)
        {
            services.AddSingleton<IClipboardService, ClipboardService>();

            services.AddSingleton<IErrorVisualizerService, ErrorVisualizerService>();

            services.AddSingleton<Func<CancellationToken, Task>>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return ct => scopeFactory.InitializeDatabaseAsync(ct);
            });

            services.AddTransient<MigrationViewModel>();
            services.AddTransient<MigrationWindow>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();

            services.AddTransient<ErrorViewModel>();
            services.AddTransient<ErrorWindow>();

            services.AddTransient<ChartsViewModel>();
            services.AddTransient<MonitoringViewModel>();

            services.AddSingleton<IWindowService>(sp =>
            {
                var ws = new WindowService(sp.GetRequiredService<IServiceScopeFactory>());

                ws.Register<MigrationViewModel, MigrationWindow>();
                ws.Register<MainViewModel, MainWindow>();
                ws.Register<ErrorViewModel, ErrorWindow>();

                return ws;
            });

            services.AddTransient<ViewModelFactory<ErrorParameters, ErrorViewModel>>(sp =>
            {
                return param => ActivatorUtilities.CreateInstance<ErrorViewModel>(sp, param);
            });
        }
    }
}