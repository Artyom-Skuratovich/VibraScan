using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.Engines.Commands.UpdateEngines;
using VibraScan.Domain.ValueObjects;
using VibraScan.Presentation.Common.Interfaces;
using VibraScan.Presentation.Models;
using VibraScan.Presentation.Services.Interfaces;

namespace VibraScan.Presentation.ViewModels
{
    public partial class EngineInspectionViewModel : ObservableObject, ICancellable
    {
        private readonly ICommandDispatcher _dispatcher;
        private readonly IWindowService _windowService;
        private readonly IErrorVisualizerService _errorVisualizer;

        private readonly IEnumerable<long> _engineIds;

        public EngineInspectionViewModel(ICommandDispatcher dispatcher, IWindowService windowService, IErrorVisualizerService errorVisualizer, IEnumerable<long> engineIds)
        {
            _dispatcher = dispatcher;
            _windowService = windowService;
            _errorVisualizer = errorVisualizer;

            _engineIds = engineIds;

            AvailableConditions = ConditionFilterItem.AvailableConditions.Where(c => c.Value != null);
            SelectedCondition = AvailableConditions.First(c => c.Value == Condition.Excellent);
            Title = "Редактирование двигател" + ((engineIds.Count() > 1) ? "ей" : "я");
        }

        [ObservableProperty] private ConditionFilterItem _selectedCondition;
        [ObservableProperty] private DateTime? _lastInspectionDate;
        [ObservableProperty] private DateTime? _nextInspectionDate;
        [ObservableProperty] private bool _allowedEditNextDate;
        [ObservableProperty] private string _title;

        public IEnumerable<ConditionFilterItem> AvailableConditions { get; }

        public void Cancel()
        {
            ApplyCommand.Cancel();
        }

        [RelayCommand]
        private async Task ApplyAsync(CancellationToken ct)
        {
            try
            {
                var command = new UpdateEnginesCommand(_engineIds, SelectedCondition.Value!, LastInspectionDate, NextInspectionDate);
                await _dispatcher.ExecuteInScopeAsync(m => m.Send(command, ct), ct);
                _windowService.Close(this, true);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError("Критическая ошибка", "Ошибка при обновлении двигателей", ex);
            }
        }

        [RelayCommand]
        private void CancelOperation()
        {
            Cancel();
        }
    }
}