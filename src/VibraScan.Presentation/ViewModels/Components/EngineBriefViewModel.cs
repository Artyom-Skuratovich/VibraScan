using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Application.Engines.Queries.GetEngines;

namespace VibraScan.Presentation.ViewModels.Components
{
    public partial class EngineBriefViewModel : ObservableObject
    {
        [ObservableProperty] private EngineBriefDto _engine = null!;
        [ObservableProperty] private bool _isChecked;
        [ObservableProperty] private bool _canCheck;
        [ObservableProperty] private bool _isVisible;

        partial void OnCanCheckChanged(bool value)
        {
            IsVisible = value;
            IsChecked = false;
        }

        [RelayCommand(CanExecute = nameof(CanCheck))]
        private void Select()
        {
            IsChecked = !IsChecked;
        }
    }
}