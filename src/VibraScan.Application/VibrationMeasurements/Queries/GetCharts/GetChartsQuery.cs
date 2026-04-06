using MediatR;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.VibrationMeasurements.Queries.GetCharts
{
    public record GetChartsQuery(
        DateTime CapturedAt,
        long PointId,
        long MeasurementProfileId,
        AxisType AxisType) : IRequest<ChartsResponse?>;
}