using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Presentation.Common;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;
using VibraScan.Presentation.ViewModels.Components;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MigrationViewModel : ObservableObject, ICancellable
    {
        private readonly IWindowService _windowService;
        private readonly IErrorVisualizerService _errorVisualizerService;
        private readonly Func<CancellationToken, Task> _migrationRunner;

        public MigrationViewModel(IWindowService windowService, IErrorVisualizerService errorVisualizerService, Func<CancellationToken, Task> migrationRunner)
        {
            _windowService = windowService;
            _errorVisualizerService = errorVisualizerService;
            _migrationRunner = migrationRunner;

            StatusText = "Выполняются миграции...";
            Spinner = new();
        }

        [ObservableProperty] private string _statusText;
        [ObservableProperty] private LoadingSpinnerViewModel _spinner;

        public void Cancel()
        {
            if (LoadedCommand.CanBeCanceled)
            {
                LoadedCommand.Cancel();
            }
        }

        [RelayCommand]
        private async Task OnLoaded(CancellationToken ct)
        {
            try
            {
                await Task.Run(async () => await _migrationRunner(ct), ct);

                _windowService.Close(this);
                _windowService.Show<MainViewModel>();
            }
            catch (OperationCanceledException)
            {
                _windowService.Shutdown();
            }
            catch (Exception ex)
            {
                Spinner.IsPaused = true;
                _errorVisualizerService.ShowError(Constants.Titles.CriticalError, Constants.Errors.MigrationExecutionFailed, ex);
                _windowService.Shutdown();
            }
        }
    }
}