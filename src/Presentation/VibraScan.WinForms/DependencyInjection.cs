using Microsoft.Extensions.DependencyInjection;
using VibraScan.Application.Common.Interfaces;
using VibraScan.WinForms.Presenters;
using VibraScan.WinForms.Views.Abstractions;
using VibraScan.WinForms.Views.Forms;

namespace VibraScan.WinForms
{
    public static class DependencyInjection
    {
        public static void AddWinFormsServices(this IServiceCollection services)
        {
            services.AddTransient<IDataImportView, DataImportForm>();
            services.AddTransient<DataImportForm>();

            services.AddSingleton<Func<string[], DataImportPresenter>>(sp =>
            {
                return files =>
                {
                    var view = sp.GetRequiredService<IDataImportView>();
                    var dispatcher = sp.GetRequiredService<ICommandDispatcher>();

                    return new DataImportPresenter(view, dispatcher, files);
                };
            });
        }
    }
}