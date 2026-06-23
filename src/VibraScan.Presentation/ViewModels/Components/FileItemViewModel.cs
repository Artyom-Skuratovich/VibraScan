using CommunityToolkit.Mvvm.ComponentModel;

namespace VibraScan.Presentation.ViewModels.Components
{
    public partial class FileItemViewModel : ObservableObject
    {
        [ObservableProperty] private string _name = null!;
        [ObservableProperty] private double _importPercentage = 0;
        [ObservableProperty] private string _stage = null!;
    }
}