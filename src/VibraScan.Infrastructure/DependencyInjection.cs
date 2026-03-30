using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.DataImport;
using VibraScan.Infrastructure.Data;
using VibraScan.Infrastructure.Data.Bulk;
using VibraScan.Infrastructure.DataImport.StartImport;
using VibraScan.Infrastructure.DataImport.StartImport.Actions;
using VibraScan.Infrastructure.Dispatching;

namespace VibraScan.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("VibraScanDb")
                                   ?? throw new InvalidOperationException("Строка подключения 'VibraScanDb' не найдена");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<ApplicationDbContextInitializer>();

            services.AddScoped<IBulkOperations, SqlServerBulkOperations>();

            services.AddScoped<IDataImportService, DataImportService>();

            services.Scan(scan => scan
                .FromAssemblyOf<ApplicationDbContext>()

                .AddClasses(classes => classes.AssignableTo<IEntityProcessor>().Where(t => !t.IsAbstract), publicOnly: false)
                    .As<IEntityProcessor>()
                    .WithScopedLifetime()

                .AddClasses(classes => classes.AssignableTo(typeof(IAfterSaveAction<>)), publicOnly: false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );

            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        }
    }
}