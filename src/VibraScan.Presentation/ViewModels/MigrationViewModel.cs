using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            Title = "Подготовка базы данных";
            Spinner = new();
        }

        [ObservableProperty] private string _statusText;
        [ObservableProperty] private string _title;
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
                _errorVisualizerService.ShowError("Критическая ошибка", "Ошибка при выполнении миграций", ex);
                _windowService.Shutdown();
            }
        }
    }
}