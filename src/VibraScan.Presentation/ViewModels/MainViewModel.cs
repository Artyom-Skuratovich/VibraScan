using CommunityToolkit.Mvvm.ComponentModel;
using VibraScan.Presentation.Common.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MainViewModel(ChartsViewModel chartsVm, MonitoringViewModel monitoringVm, DataImportViewModel dataImportVm) : ObservableObject, ICancellable
    {
        [ObservableProperty] private ChartsViewModel _chartsVm = chartsVm;
        [ObservableProperty] private MonitoringViewModel _monitoringVm = monitoringVm;
        [ObservableProperty] private DataImportViewModel _dataImportVm = dataImportVm;

        public void Cancel()
        {
            ChartsVm.Cancel();
            MonitoringVm.Cancel();
            DataImportVm.Cancel();
        }
    }
}