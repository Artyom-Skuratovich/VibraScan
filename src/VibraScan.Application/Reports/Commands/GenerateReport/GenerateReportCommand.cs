using MediatR;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.Reports.Commands.GenerateReport
{
    public record GenerateReportCommand(
        DateTime From,
        DateTime To,
        long EngineId,
        long MeasurementProfileId,
        AxisType AxisType,
        string DestinationPath) : IRequest;
}