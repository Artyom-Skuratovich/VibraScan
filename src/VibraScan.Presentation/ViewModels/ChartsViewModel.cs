using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VibraScan.Application.AxisTypes.Queries.GetAxisTypes;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.Engines.Queries.GetEngines;
using VibraScan.Application.MeasurementProfiles.Queries.GetMeasurementProfiles;
using VibraScan.Application.Points.Queries.GetPoints;
using VibraScan.Application.VibrationMeasurements.Queries.GetCharts;
using VibraScan.Application.VibrationMeasurements.Queries.GetMeasurementDates;
using VibraScan.Application.Workshops.Queries.GetWorkshops;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;
using VibraScan.Presentation.Common;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class ChartsViewModel(ICommandDispatcher commandDispatcher, IErrorVisualizerService errorVisualizer)
        : ObservableObject, ICancellable
    {
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
        private readonly IErrorVisualizerService _errorVisualizer = errorVisualizer;

        private bool _isInitialized = false;

        public ObservableCollection<Workshop> Workshops { get; } = [];
        public ObservableCollection<EngineBriefDto> Engines { get; } = [];
        public ObservableCollection<Point> Points { get; } = [];
        public ObservableCollection<AxisType> AxisTypes { get; } = [];
        public ObservableCollection<DateTime> MeasurementDates { get; } = [];
        public ObservableCollection<MeasurementProfile> MeasurementProfiles { get; } = [];

        [ObservableProperty] private ChartBundle<TimeDomainChart>? _timeDomain;
        [ObservableProperty] private ChartBundle<FrequencyDomainChart>? _frequencyDomain;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsWorkshopSelected))]
        private Workshop? _currentWorkshop;

        partial void OnCurrentWorkshopChanged(Workshop? value)
        {
            SelectedEngine = null;

            if (LoadEnginesCommand.IsRunning)
            {
                LoadEnginesCommand.Cancel();
            }

            LoadEnginesCommand.Execute(null);
        }

        public bool IsWorkshopSelected => CurrentWorkshop != null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEngineSelected))]
        private EngineBriefDto? _selectedEngine;

        partial void OnSelectedEngineChanged(EngineBriefDto? value)
        {
            SelectedPoint = null;

            if (LoadPointsCommand.IsRunning)
            {
                LoadPointsCommand.Cancel();
            }

            LoadPointsCommand.Execute(null);
        }

        public bool IsEngineSelected => SelectedEngine != null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPointSelected))]
        private Point? _selectedPoint;

        partial void OnSelectedPointChanged(Point? value)
        {
            SelectedAxisType = null;

            if (LoadAxisTypesCommand.IsRunning)
            {
                LoadAxisTypesCommand.Cancel();
            }

            LoadAxisTypesCommand.Execute(null);
        }

        public bool IsPointSelected => SelectedPoint != null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsAxisTypeSelected))]
        private AxisType? _selectedAxisType;

        partial void OnSelectedAxisTypeChanged(AxisType? value)
        {
            SelectedMeasurementProfile = null;

            if (LoadMeasurementProfilesCommand.IsRunning)
            {
                LoadMeasurementProfilesCommand.Cancel();
            }

            LoadMeasurementProfilesCommand.Execute(null);
        }

        public bool IsAxisTypeSelected => SelectedAxisType is not null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsMeasurementProfileSelected))]
        private MeasurementProfile? _selectedMeasurementProfile;

        partial void OnSelectedMeasurementProfileChanged(MeasurementProfile? value)
        {
            SelectedMeasurementDate = null;

            if (LoadMeasurementDatesCommand.IsRunning)
            {
                LoadMeasurementDatesCommand.Cancel();
            }

            LoadMeasurementDatesCommand.Execute(null);
        }

        public bool IsMeasurementProfileSelected => SelectedMeasurementProfile != null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsMeasurementDateSelected))]
        private DateTime? _selectedMeasurementDate;

        partial void OnSelectedMeasurementDateChanged(DateTime? value)
        {
            TimeDomain = null;
            FrequencyDomain = null;

            if (LoadChartsCommand.IsRunning)
            {
                LoadChartsCommand.Cancel();
            }

            LoadChartsCommand.Execute(null);
        }

        public bool IsMeasurementDateSelected => SelectedMeasurementDate.HasValue;

        public void Cancel()
        {
            LoadedCommand.Cancel();
            LoadWorkshopsCommand.Cancel();
            LoadEnginesCommand.Cancel();
            LoadPointsCommand.Cancel();
            LoadAxisTypesCommand.Cancel();
            LoadMeasurementProfilesCommand.Cancel();
            LoadMeasurementDatesCommand.Cancel();
            LoadChartsCommand.Cancel();
        }

        [RelayCommand]
        private async Task OnLoaded(CancellationToken ct)
        {
            if (_isInitialized) return;

            await LoadWorkshopsCommand.ExecuteAsync(ct);

            _isInitialized = true;
        }

        [RelayCommand]
        private async Task LoadWorkshopsAsync(CancellationToken ct)
        {
            try
            {
                var workshops = await _commandDispatcher.SendAsync(new GetWorkshopsQuery(), ct);
                Workshops.Clear();

                foreach (var workshop in workshops)
                {
                    Workshops.Add(workshop);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError(Constants.Titles.CriticalError, Constants.Errors.LoadWorkshopsFailed, ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsWorkshopSelected))]
        private async Task LoadEnginesAsync(CancellationToken ct)
        {
            try
            {
                Engines.Clear();

                if (CurrentWorkshop == null) return;

                var query = new GetEnginesQuery(CurrentWorkshop.Id);
                var engines = await _commandDispatcher.SendAsync(query, ct);

                foreach (var engine in engines)
                {
                    Engines.Add(engine);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError(Constants.Titles.CriticalError, Constants.Errors.LoadEnginesFailed, ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsEngineSelected))]
        private async Task LoadPointsAsync(CancellationToken ct)
        {
            try
            {
                Points.Clear();

                if (SelectedEngine == null) return;

                var query = new GetPointsQuery(SelectedEngine.Id);
                var points = await _commandDispatcher.SendAsync(query, ct);

                foreach (var point in points)
                {
                    Points.Add(point);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError(Constants.Titles.CriticalError, Constants.Errors.LoadPointsFailed, ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsPointSelected))]
        private async Task LoadAxisTypesAsync(CancellationToken ct)
        {
            try
            {
                AxisTypes.Clear();

                if (SelectedPoint == null) return;

                var query = new GetAxisTypesQuery(SelectedPoint.Id);
                var axisTypes = await _commandDispatcher.SendAsync(query, ct);

                foreach (var axisType in axisTypes)
                {
                    AxisTypes.Add(axisType);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError(Constants.Titles.CriticalError, Constants.Errors.LoadAxisTypesFailed, ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsAxisTypeSelected))]
        private async Task LoadMeasurementProfilesAsync(CancellationToken ct)
        {
            try
            {
                MeasurementProfiles.Clear();

                if ((SelectedPoint == null) || (SelectedAxisType is null)) return;

                var query = new GetMeasurementProfilesQuery(SelectedPoint.Id, SelectedAxisType);
                var profiles = await _commandDispatcher.SendAsync(query, ct);

                foreach (var profile in profiles)
                {
                    MeasurementProfiles.Add(profile);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError(Constants.Titles.CriticalError, Constants.Errors.LoadMeasurementProfilesFailed, ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsMeasurementDateSelected))]
        private async Task LoadMeasurementDatesAsync(CancellationToken ct)
        {
            try
            {
                MeasurementDates.Clear();

                if ((SelectedPoint == null) || (SelectedAxisType is null) || (SelectedMeasurementProfile == null)) return;

                var query = new GetMeasurementDatesQuery(SelectedPoint.Id, SelectedMeasurementProfile.Id, SelectedAxisType);
                var measurementDates = await _commandDispatcher.SendAsync(query, ct);

                foreach (var date in measurementDates)
                {
                    MeasurementDates.Add(date);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError(Constants.Titles.CriticalError, Constants.Errors.LoadMeasurementDatesFailed, ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsMeasurementDateSelected))]
        private async Task LoadChartsAsync(CancellationToken ct)
        {
            try
            {
                if ((SelectedPoint == null) || (SelectedAxisType is null) || (SelectedMeasurementProfile == null) || !SelectedMeasurementDate.HasValue) return;

                var query = new GetChartsQuery(SelectedMeasurementDate.Value, SelectedPoint.Id, SelectedMeasurementProfile.Id, SelectedAxisType);
                var charts = await _commandDispatcher.SendAsync(query, ct);

                TimeDomain = charts?.TimeDomain;
                FrequencyDomain = charts?.FrequencyDomain;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError(Constants.Titles.CriticalError, Constants.Errors.LoadChartsFailed, ex);
            }
        }
    }
}