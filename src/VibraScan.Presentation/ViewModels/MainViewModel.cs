using CommunityToolkit.Mvvm.ComponentModel;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MainViewModel(ChartsViewModel chartsVm, MonitoringViewModel monitoringVm, DataImportViewModel dataImportVm) : ObservableObject
    {
        [ObservableProperty] private ChartsViewModel _chartsVm = chartsVm;
        [ObservableProperty] private MonitoringViewModel _monitoringVm = monitoringVm;
        [ObservableProperty] private DataImportViewModel _dataImportVm = dataImportVm;
    }
}