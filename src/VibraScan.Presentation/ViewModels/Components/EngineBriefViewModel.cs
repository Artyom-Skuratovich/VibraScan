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

        public bool IsInspectionOverdue =>
            Engine.NextInspectionDate.HasValue &&
            Engine.NextInspectionDate.Value.Date <= DateTime.Today;

        partial void OnCanCheckChanged(bool value)
        {
            IsVisible = value;
            IsChecked = false;
        }

        partial void OnEngineChanged(EngineBriefDto value)
        {
            OnPropertyChanged(nameof(IsInspectionOverdue));
        }

        [RelayCommand(CanExecute = nameof(CanCheck))]
        private void Select()
        {
            IsChecked = !IsChecked;
        }
    }
}