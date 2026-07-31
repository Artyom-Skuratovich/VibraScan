using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VibraScan.Presentation.ViewModels.Components
{
    public partial class ToastNotificationViewModel(int delayMs = 2000) : ObservableObject
    {
        private readonly int _delayMs = delayMs;

        [ObservableProperty] private string? _message;
        [ObservableProperty] private bool _isSuccess;
        [ObservableProperty] private bool _isVisible;

        [RelayCommand]
        private async Task Show(CancellationToken ct)
        {
            IsVisible = true;

            try
            {
                await Task.Delay(_delayMs, ct);
                IsVisible = false;
            }
            catch (OperationCanceledException)
            {
            }
        }

        [RelayCommand]
        private void Close()
        {
            if (ShowCommand.CanBeCanceled)
            {
                ShowCommand.Cancel();
            }
            IsVisible = false;
        }
    }
}