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
    public partial class ChartsViewModel(ICommandDispatcher commandDispatcher, IErrorVisualizerService errorVisualizer) : ObservableObject, ICancellable
    {
        private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
        private readonly IErrorVisualizerService _errorVisualizer = errorVisualizer;

        public ObservableCollection<Workshop> Workshops { get; } = [];

        public ObservableCollection<EngineBriefDto> Engines { get; } = [];

        public ObservableCollection<AxisType> AxisTypes { get; } = [AxisType.Vertical, AxisType.Horizontal, AxisType.Axial];

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

            // LoadPointsCommand...
        }

        public bool IsEngineSelected => SelectedEngine != null;

        [ObservableProperty]
        private Point? _selectedPoint;

        public void Cancel()
        {
            LoadWorkshopsCommand.Cancel();
            LoadEnginesCommand.Cancel();
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
                _errorVisualizer.ShowError("Критическая ошибка", "Ошибка при загрузке цехов", ex);
            }
        }

        [RelayCommand(CanExecute = nameof(IsWorkshopSelected))]
        private async Task LoadEnginesAsync(CancellationToken ct)
        {
            try
            {
                if (CurrentWorkshop == null) return;

                var query = new GetEnginesQuery(CurrentWorkshop.Id);
                var engines = await _commandDispatcher.SendAsync(query, ct);
                Engines.Clear();

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
                _errorVisualizer.ShowError("Критическая ошибка", "Ошибка при загрузке двигателей", ex);
            }
        }
    }
}