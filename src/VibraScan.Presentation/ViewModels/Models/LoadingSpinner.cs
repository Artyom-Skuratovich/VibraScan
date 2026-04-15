using CommunityToolkit.Mvvm.ComponentModel;

namespace VibraScan.Presentation.ViewModels.Models
{
    public partial class LoadingSpinner : ObservableObject
    {
        [ObservableProperty]
        private bool _isPaused;

        [ObservableProperty]
        private bool _isActive = true;
    }
}