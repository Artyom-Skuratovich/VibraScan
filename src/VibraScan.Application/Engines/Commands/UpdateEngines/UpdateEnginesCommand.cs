using MediatR;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.Engines.Commands.UpdateEngines
{
    public record UpdateEnginesCommand(
        IEnumerable<long> EngineIds,
        Condition Condition,
        DateTime? LastInspectionDate,
        DateTime? NextInspectionDate,
        bool IsManualNextDateCalculation) : IRequest;
}