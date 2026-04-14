using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MigrationViewModel(IWindowService windowService, IErrorVisualizerService errorVisualizerService, Func<CancellationToken, Task> migrationRunner)
        : ObservableObject, ISupportCancellation
    {
        private readonly IWindowService _windowService = windowService;
        private readonly IErrorVisualizerService _errorVisualizerService = errorVisualizerService;
        private readonly Func<CancellationToken, Task> _migrationRunner = migrationRunner;

        [ObservableProperty]
        private string _statusText = "Выполняются миграции...";

        [ObservableProperty]
        private string _title = "Подготовка базы данных";

        [ObservableProperty]
        private bool _isLoading = true;

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
                IsLoading = false;
                _errorVisualizerService.ShowError("Критическая ошибка", "Ошибка при выполнении миграций", ex);
                _windowService.Shutdown();
            }
        }
    }
}