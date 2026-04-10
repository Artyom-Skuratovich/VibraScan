using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class ErrorViewModel : ObservableObject
    {
        private readonly IClipboardService _clipboard;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _message;

        [ObservableProperty]
        private string? _stackTrace;

        [ObservableProperty]
        private bool _hasDetails;

        [ObservableProperty]
        private bool _isToastVisible;

        [ObservableProperty]
        private string _toastMessage = string.Empty;

        [ObservableProperty]
        private bool _isSuccess;

        public ErrorViewModel(string title, string message, Exception? ex, IClipboardService clipboard)
        {
            Title = title;
            Message = message;

            if (ex != null)
            {
                StackTrace = ex.ToString();
                HasDetails = true;
            }

            _clipboard = clipboard;
        }

        [RelayCommand(CanExecute = nameof(HasDetails))]
        private async Task CopyStackTraceToClipboardAsync()
        {
            IsSuccess = _clipboard.SetText(StackTrace!);

            ToastMessage = IsSuccess ? "Скопировано в буфер!" : "Ошибка доступа к буферу";
            IsToastVisible = true;

            await Task.Delay(2500);
            IsToastVisible = false;
        }
    }
}