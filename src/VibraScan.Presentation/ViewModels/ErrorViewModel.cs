using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Presentation.Services.Interfaces;
using VibraScan.Presentation.ViewModels.Models;

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
        public ToastNotification _currentToast;

        public ErrorViewModel(ErrorParameters parameters, IClipboardService clipboard)
        {
            Title = parameters.Title;
            Message = parameters.Message;

            if (parameters.Exception != null)
            {
                StackTrace = parameters.Exception.ToString();
                HasDetails = true;
            }
            CurrentToast = new ToastNotification();
            _clipboard = clipboard;
        }

        [RelayCommand(CanExecute = nameof(HasDetails), AllowConcurrentExecutions = true)]
        private async Task CopyToClipboard()
        {
            CurrentToast.IsSuccess = _clipboard.SetText(StackTrace!);
            CurrentToast.Message = CurrentToast.IsSuccess ? "Скопировано в буфер" : "Ошибка доступа к буферу";

            await CurrentToast.ShowCommand.ExecuteAsync(null);
        }
    }
}