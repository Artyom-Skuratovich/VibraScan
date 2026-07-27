using CommunityToolkit.Mvvm.ComponentModel;

namespace VibraScan.Presentation.ViewModels.Components
{
    public partial class LoadingSpinnerViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isPaused;

        [ObservableProperty]
        private bool _isActive = true;
    }
}