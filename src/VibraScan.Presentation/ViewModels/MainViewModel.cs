using CommunityToolkit.Mvvm.ComponentModel;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MainViewModel(ChartsViewModel chartsVm, MonitoringViewModel monitoringVm) : ObservableObject
    {
        [ObservableProperty]
        private ChartsViewModel _chartsVm = chartsVm;


        [ObservableProperty]
        private MonitoringViewModel _monitoringVm = monitoringVm;
    }
}