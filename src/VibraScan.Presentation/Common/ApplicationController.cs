using Microsoft.Extensions.DependencyInjection;
using VibraScan.Presentation.Presenters.Base;

namespace VibraScan.Presentation.Common
{
    public class ApplicationController(IServiceScopeFactory scopeFactory) : IApplicationController
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public void Run<TPresenter>() where TPresenter : class, IPresenter
        {
            Run<TPresenter>(p => p.Run());
        }

        public void Run<TPresenter, TArg>(TArg argument) where TPresenter : class, IPresenter<TArg>
        {
            Run<TPresenter>(p => p.Run(argument));
        }

        private void Run<TPresenter>(Action<TPresenter> run) where TPresenter : IBasePresenter
        {
            var scope = _scopeFactory.CreateScope();

            try
            {
                var presenter = scope.ServiceProvider.GetRequiredService<TPresenter>();
                var view = presenter.View;

                void OnClosed()
                {
                    view.Closed -= OnClosed;
                    scope.Dispose();
                }

                view.Closed += OnClosed;
                run(presenter);
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }
    }
}