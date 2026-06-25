using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Application.DataImport;

namespace VibraScan.Presentation.ViewModels.Components
{
    public partial class FileItemViewModel : ObservableObject
    {
        private const string DefaultStage = "Готов к импорту";

        [ObservableProperty] private string _name = null!;
        [ObservableProperty] private double _importPercentage;
        [ObservableProperty] private string _stage = DefaultStage;
        [ObservableProperty] private ImportError? _error;
        [ObservableProperty] private bool _isExpanded;
        [ObservableProperty] private bool _isFailure;

        partial void OnErrorChanged(ImportError? value)
        {
            IsFailure = value != null;
        }

        public string FullPath { get; set; } = null!;

        public void ResetToDefault(bool includeError = true)
        {
            ImportPercentage = 0;
            Stage = DefaultStage;
            IsExpanded = false;

            if (includeError)
            {
                Error = null;
            }
        }

        [RelayCommand]
        private void ToggleExpand()
        {
            if (IsFailure)
            {
                IsExpanded = !IsExpanded;
            }
        }
    }
}