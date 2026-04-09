using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VibraScan.Presentation.Common;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.Services
{
    public class WindowService(IServiceScopeFactory scopeFactory) : IWindowService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly Dictionary<Type, Type> _mappings = [];
        private readonly Dictionary<object, (Window Window, IServiceScope Scope)> _openWindows = [];

        public void Register<TViewModel, TWindow>() where TViewModel : class where TWindow : Window
        {
            _mappings[typeof(TViewModel)] = typeof(TWindow);
        }

        public void Close(object model)
        {
            if (_openWindows.TryGetValue(model, out var entry))
            {
                entry.Window.Close();
            }
        }

        public void Show<TVm>() where TVm : class
        {
            PrepareAndDisplay(sp => sp.GetRequiredService<TVm>(), false);
        }

        public void Show<TVm, TParam>(TParam param) where TVm : class
        {
            PrepareAndDisplay(sp => CreateViewModelInstance<TVm, TParam>(sp, param), false);
        }

        public bool? ShowDialog<TVm>() where TVm : class
        {
            return PrepareAndDisplay(sp => sp.GetRequiredService<TVm>(), true);
        }

        public bool? ShowDialog<TVm, TParam>(TParam param) where TVm : class
        {
            return PrepareAndDisplay(sp => CreateViewModelInstance<TVm, TParam>(sp, param), true);
        }

        private bool? PrepareAndDisplay<TVm>(Func<IServiceProvider, TVm> factory, bool isDialog) where TVm : class
        {
            var scope = _scopeFactory.CreateScope();

            try
            {
                var viewModel = factory(scope.ServiceProvider);
                var window = CreateWindow(viewModel, scope.ServiceProvider);

                _openWindows[viewModel] = (window, scope);

                if (isDialog)
                {
                    return window.ShowDialog();
                }

                window.Show();

                return true;
            }
            catch
            {
                scope.Dispose();
                throw;
            }
        }

        private Window CreateWindow(object model, IServiceProvider serviceProvider)
        {
            var modelType = model.GetType();

            if (!_mappings.TryGetValue(modelType, out var windowType))
            {
                throw new InvalidOperationException($"Отсутствует определение окна для {modelType.Name}");
            }

            var window = (Window)ActivatorUtilities.CreateInstance(serviceProvider, windowType);
            window.DataContext = model;

            ConfigureWindowLifeCycle(window, model);
            SetWindowOwner(window);

            return window;
        }

        private void SetWindowOwner(Window window)
        {
            if (window is IMainWindow)
            {
                System.Windows.Application.Current.MainWindow = window;

                foreach (var entry in _openWindows)
                {
                    var openWindow = entry.Value.Window;

                    if ((openWindow != window) && (openWindow.Owner == null))
                    {
                        openWindow.Owner = window;
                    }
                }
            }
            else
            {
                var active = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive && w.IsVisible);
                window.Owner = active ?? System.Windows.Application.Current.MainWindow;
            }
        }

        private void ConfigureWindowLifeCycle(Window window, object model)
        {
            if (model is ISupportCancellation cancelable)
            {
                window.Closing += (s, e) => cancelable.Cancel();
            }

            window.Closed += (s, e) =>
            {
                if (_openWindows.Remove(model, out var entry))
                {
                    entry.Scope.Dispose();
                }

                window.DataContext = null;

                if (window is IMainWindow)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            };
        }

        private static TVm CreateViewModelInstance<TVm, TParam>(IServiceProvider serviceProvider, TParam param) where TVm : class
        {
            var factory = serviceProvider.GetRequiredService<ViewModelFactory<TParam, TVm>>();
            return factory(param);
        }
    }
}