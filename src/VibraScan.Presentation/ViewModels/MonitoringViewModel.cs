using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.Engines.Queries.GetEngines;
using VibraScan.Application.Workshops.Queries.GetWorkshops;
using VibraScan.Domain.Entities;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Models;
using VibraScan.Presentation.Services.Interfaces;
using VibraScan.Presentation.ViewModels.Components;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MonitoringViewModel : ObservableObject, ICancellable
    {
        private readonly IWindowService _windowService;
        private readonly IErrorVisualizerService _errorVisualizer;
        private readonly ICommandDispatcher _commandDispatcher;

        private bool _isInitialized;

        public MonitoringViewModel(IWindowService windowService, IErrorVisualizerService errorVisualizer, ICommandDispatcher commandDispatcher)
        {
            _windowService = windowService;
            _errorVisualizer = errorVisualizer;
            _commandDispatcher = commandDispatcher;

            _isInitialized = false;

            Workshops = [];
            Engines = [];
            LastUpdatedTime = "--:--";
            SelectedInspectionFilter = InspectionStatusFilter.All;
            SelectedDateType = InspectionDateType.LastInspection;
        }

        [ObservableProperty] private ObservableCollection<Workshop> _workshops;
        [ObservableProperty] private ObservableCollection<EngineBriefViewModel> _engines;
        [ObservableProperty] private string _lastUpdatedTime;
        [ObservableProperty] private Workshop? _currentWorkshop;
        [ObservableProperty] private string? _searchText;
        [ObservableProperty] private DateTime? _dateFrom;
        [ObservableProperty] private DateTime? _dateTo;
        [ObservableProperty] private InspectionStatusFilter _selectedInspectionFilter;
        [ObservableProperty] private InspectionDateType _selectedDateType;
        [ObservableProperty] private ConditionFilterItem _selectedCondition = null!;
        [ObservableProperty] private bool _isWorkshopSelected;
        [ObservableProperty] private bool _isMultiSelectMode;

        public bool CanEditSelected => IsMultiSelectMode && Engines.Any(e => e.IsChecked);

        public int UrgentCheckCount => Engines.Count(e => e.Engine.NextInspectionDate.HasValue && e.Engine.NextInspectionDate <= DateTime.Today);

        public void Cancel()
        {
            LoadedCommand.Cancel();
            LoadWorkshopsCommand.Cancel();
            LoadEnginesCommand.Cancel();
        }

        partial void OnIsMultiSelectModeChanged(bool value)
        {
            foreach (var engine in Engines)
            {
                engine.CanCheck = value;
            }

            EditSelectedEnginesCommand.NotifyCanExecuteChanged();
        }

        partial void OnCurrentWorkshopChanged(Workshop? value)
        {
            IsWorkshopSelected = value is not null;
            IsMultiSelectMode = false;

            if (LoadEnginesCommand.IsRunning)
            {
                LoadEnginesCommand.Cancel();
            }

            LoadEnginesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadWorkshopsAsync(CancellationToken ct)
        {
            try
            {
                var actualWorkshops = await _commandDispatcher.SendAsync(new GetWorkshopsQuery(), ct);
                Workshops.Clear();

                foreach (var workshop in actualWorkshops)
                {
                    Workshops.Add(workshop);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError("Критическая ошибка", "Ошибка при загрузке цехов", ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsWorkshopSelected))]
        private async Task LoadEnginesAsync(CancellationToken ct)
        {
            try
            {
                if (CurrentWorkshop is null)
                {
                    return;
                }

                var query = new GetEnginesQuery(CurrentWorkshop.Id, SearchText, SelectedCondition.Value, DateFrom, DateTo, SelectedDateType, SelectedInspectionFilter);
                var engines = await _commandDispatcher.SendAsync(query, ct);

                foreach (var oldEngine in Engines)
                {
                    oldEngine.PropertyChanged -= OnEngineViewModelPropertyChanged;
                }
                Engines.Clear();

                foreach (var engine in engines)
                {
                    var vm = new EngineBriefViewModel
                    {
                        Engine = engine,
                        CanCheck = IsMultiSelectMode
                    };
                    vm.PropertyChanged += OnEngineViewModelPropertyChanged;

                    Engines.Add(vm);
                }

                OnPropertyChanged(nameof(UrgentCheckCount));
                EditSelectedEnginesCommand.NotifyCanExecuteChanged();
                LastUpdatedTime = DateTime.Now.ToString(@"dd.MM.yyyy \в HH:mm");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError("Критическая ошибка", "Ошибка при загрузке двигателей", ex);
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditSelected))]
        private void EditSelectedEngines()
        {
            var engineIds = Engines.Where(e => e.IsChecked)
                                   .Select(e => e.Engine.Id)
                                   .ToList();

            if (_windowService.ShowDialog<EngineInspectionViewModel, IEnumerable<long>>(engineIds) == true)
            {
                IsMultiSelectMode = false;
                LoadEnginesCommand.Execute(null);
            }
        }

        [RelayCommand]
        private async Task OnLoaded(CancellationToken ct)
        {
            if (_isInitialized) return;

            await LoadWorkshopsCommand.ExecuteAsync(ct);
            ResetFilters();

            _isInitialized = true;
        }

        private void ResetFilters()
        {
            SearchText = string.Empty;
            SelectedCondition = ConditionFilterItem.AvailableConditions.First(c => c.Value is null);
            DateFrom = DateTo = null;
            SelectedInspectionFilter = InspectionStatusFilter.All;
        }

        private void OnEngineViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EngineBriefViewModel.IsChecked))
            {
                OnPropertyChanged(nameof(CanEditSelected));
                EditSelectedEnginesCommand.NotifyCanExecuteChanged();
            }
        }
    }
}