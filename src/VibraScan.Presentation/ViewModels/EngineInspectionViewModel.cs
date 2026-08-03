using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VibraScan.Application.Common.Exceptions;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.Engines.Commands.UpdateEngines;
using VibraScan.Domain.ValueObjects;
using VibraScan.Presentation.Common;
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
            LastInspectionDate = DateTime.Today;
            Title = "Редактирование двигател" + ((engineIds.Count() > 1) ? "ей" : "я");
            IsApplying = false;
        }

        [ObservableProperty] private ConditionFilterItem _selectedCondition;
        [ObservableProperty] private DateTime? _lastInspectionDate;
        [ObservableProperty] private DateTime? _nextInspectionDate;
        [ObservableProperty] private bool _allowedEditNextDate;
        [ObservableProperty] private string _title;
        [ObservableProperty] private string? _validationError;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CancelOperationCommand))]
        private bool _isApplying;

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
                ValidationError = null;

                IsApplying = true;

                var command = new UpdateEnginesCommand(
                    _engineIds,
                    SelectedCondition.Value!,
                    LastInspectionDate,
                    NextInspectionDate,
                    AllowedEditNextDate);

                await _dispatcher.ExecuteInScopeAsync(m => m.Send(command, ct), ct);
                _windowService.Close(this, true);
            }
            catch (ValidationException ex)
            {
                if (ex.Errors.TryGetValue(nameof(UpdateEnginesCommand.NextInspectionDate), out var messages))
                {
                    ValidationError = messages.FirstOrDefault();
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _errorVisualizer.ShowError(Constants.Titles.CriticalError, Constants.Errors.EngineUpdateFailed, ex);
            }
            finally
            {
                IsApplying = false;
            }
        }

        [RelayCommand(CanExecute = nameof(IsApplying))]
        private void CancelOperation()
        {
            Cancel();
        }
    }
}