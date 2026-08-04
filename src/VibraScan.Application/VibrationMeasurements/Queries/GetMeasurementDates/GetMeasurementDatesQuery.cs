using MediatR;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.VibrationMeasurements.Queries.GetMeasurementDates
{
    public record GetMeasurementDatesQuery(long PointId, long MeasurementProfileId, AxisType AxisType) : IRequest<IEnumerable<DateTime>>;
}