using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MigrationViewModel(IWindowService windowService, Func<CancellationToken, Task> migrationRunner) : ObservableObject, ISupportCancellation
    {
        private readonly IWindowService _windowService = windowService;
        private readonly Func<CancellationToken, Task> _migrationRunner = migrationRunner;

        [ObservableProperty]
        private string _statusText = "Выполняются миграции...";

        [ObservableProperty]
        private string _title = "Подготовка базы данных";

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
                await _migrationRunner(ct);

                _windowService.Close(this);
                _windowService.Show<MainViewModel>();
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                // TODO: вызов сервиса, который выводит ошибки пользователю.
            }
        }
    }
}