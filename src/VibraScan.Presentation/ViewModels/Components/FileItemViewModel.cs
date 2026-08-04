using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Application.DataImport;
using VibraScan.Presentation.Common.Interfaces;

namespace VibraScan.Presentation.ViewModels.Components
{
    public partial class FileItemViewModel : ObservableObject, ITrackableTask
    {
        private const string DefaultStage = "Готов к импорту";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCompleted))]
        private double _importPercentage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCompleted))]
        private bool _isFailure;

        [ObservableProperty] private string _name = null!;
        [ObservableProperty] private string _stage = DefaultStage;
        [ObservableProperty] private ImportError? _error;
        [ObservableProperty] private bool _isExpanded;

        partial void OnErrorChanged(ImportError? value)
        {
            IsFailure = value != null;
        }

        public string FullPath { get; set; } = null!;

        public bool IsCompleted => (ImportPercentage >= 100.0) || IsFailure;

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