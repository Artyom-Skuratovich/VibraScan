using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.Engines.Queries.GetEngines;
using VibraScan.Application.Workshops.Queries.GetWorkshops;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class MonitoringViewModel(IErrorVisualizerService errorVisualizer, ICommandDispatcher commandDispatcher) : ObservableObject, ICancellable
    {
        public class ConditionFilterItem(Condition? condition)
        {
            public Condition? Value { get; } = condition;

            public override string ToString()
            {
                return Value?.Description ?? "Все состояния";
            }
        }

        private readonly IErrorVisualizerService _errorVisualizer = errorVisualizer;
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;

        private bool _isInitialized = false;

        [ObservableProperty] private ObservableCollection<Workshop> _workshops = [];
        [ObservableProperty] private ObservableCollection<EngineBriefViewModel> _engines = [];
        [ObservableProperty] private string _lastUpdatedTime = "--:--";
        [ObservableProperty] private Workshop? _currentWorkshop;
        [ObservableProperty] private string? _searchText;
        [ObservableProperty] private DateTime? _dateFrom;
        [ObservableProperty] private DateTime? _dateTo;
        [ObservableProperty] private InspectionStatusFilter _selectedInspectionFilter = InspectionStatusFilter.All;
        [ObservableProperty] private InspectionDateType _selectedDateType = InspectionDateType.LastInspection;
        [ObservableProperty] private ConditionFilterItem _selectedCondition = null!;
        [ObservableProperty] private bool _isWorkshopSelected;

        public int UrgentCheckCount => Engines.Count(e => e.Engine.NextInspectionDate.HasValue && e.Engine.NextInspectionDate <= DateTime.Today);

        public IEnumerable<ConditionFilterItem> AvailableConditions { get; } = [
            new(null),
            new(Condition.Excellent),
            new(Condition.Satisfactory),
            new(Condition.Unsatisfactory),
            new(Condition.Bad),
            new(Condition.NotChecked)
        ];

        public void Cancel()
        {
            LoadedCommand.Cancel();
            LoadWorkshopsCommand.Cancel();
            LoadEnginesCommand.Cancel();
        }

        partial void OnCurrentWorkshopChanged(Workshop? value)
        {
            IsWorkshopSelected = value is not null;

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
                Engines.Clear();

                foreach (var engine in engines)
                {
                    var vm = new EngineBriefViewModel
                    {
                        Engine = engine
                    };

                    Engines.Add(vm);
                }

                OnPropertyChanged(nameof(UrgentCheckCount));
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
            SelectedCondition = AvailableConditions.First(c => c.Value is null);
            DateFrom = DateTo = null;
            SelectedInspectionFilter = InspectionStatusFilter.All;
        }
    }
}