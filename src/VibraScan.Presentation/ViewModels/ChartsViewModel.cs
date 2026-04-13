using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.Engines.Queries.GetEngines;
using VibraScan.Application.Workshops.Queries.GetWorkshops;
using VibraScan.Domain.Entities;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public sealed partial class ChartsViewModel(ICommandDispatcher commandDispatcher, IErrorVisualizerService errorVisualizerService)
        : ObservableObject, ISupportCancellation, IDisposable
    {
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
        private readonly IErrorVisualizerService _errorVisualizerService = errorVisualizerService;
        private CancellationTokenSource _cts = new();

        [ObservableProperty] private ObservableCollection<Workshop> _workshops = [];
        [ObservableProperty] private ObservableCollection<EngineBriefDto> _engines = [];
        [ObservableProperty] private ObservableCollection<Point> _points = [];
        [ObservableProperty] private ObservableCollection<MeasurementProfile> _profiles = [];
        [ObservableProperty] private ObservableCollection<DateTime> _measurementDates = [];

        [ObservableProperty] private Workshop? _selectedWorkshop;

        partial void OnSelectedWorkshopChanged(Workshop? value)
        {
            _ = LoadEnginesAsync(value);
        }

        private async Task LoadEnginesAsync(Workshop? workshop)
        {
            try
            {
                SelectedEngine = null;
                Engines.Clear();

                if (workshop != null)
                {
                    var engines = await _commandDispatcher.SendAsync(new GetEnginesQuery(workshop.Id), _cts.Token);
                    foreach (var engine in engines)
                    {
                        Engines.Add(engine);
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                _errorVisualizerService.ShowError("Критическая ошибка", $"Ошибка при загрузке двигателей цеха {workshop!.Name}", ex);
            }
        }

        [ObservableProperty]
        private Engine? _selectedEngine;

        [ObservableProperty]
        private Point? _selectedPoint;

        [ObservableProperty]
        private MeasurementProfile? _selectedProfile;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsChartsVisible))]
        private DateTime? _selectedMeasurementDate;

        [ObservableProperty]
        private bool _isChartsVisible;

        [RelayCommand]
        private async Task OnLoaded(CancellationToken ct)
        {
            try
            {
                var workshops = await _commandDispatcher.SendAsync(new GetWorkshopsQuery(), ct);

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
                _errorVisualizerService.ShowError("Критическая ошибка", "Ошибка при загрузке цехов", ex);
            }
        }

        [RelayCommand]
        private async Task CloseCharts(CancellationToken ct)
        {

        }

        public void Cancel()
        {
            _cts.Cancel();
            _cts.Dispose();

            _cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}