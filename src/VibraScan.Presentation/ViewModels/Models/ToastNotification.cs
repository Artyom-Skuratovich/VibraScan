using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VibraScan.Presentation.ViewModels.Models
{
    public partial class ToastNotification : ObservableObject
    {
        [ObservableProperty]
        private string? _message;

        [ObservableProperty]
        private bool _isSuccess;

        [ObservableProperty]
        private bool _isVisible;

        [RelayCommand]
        private void Close()
        {
            IsVisible = false;
        }
    }
}