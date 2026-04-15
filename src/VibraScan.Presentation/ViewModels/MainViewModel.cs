using CommunityToolkit.Mvvm.ComponentModel;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MainViewModel(ChartsViewModel chartsVm) : ObservableObject
    {
        [ObservableProperty]
        private ChartsViewModel _chartsVm = chartsVm;
    }
}