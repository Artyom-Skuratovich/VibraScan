using MediatR;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.MeasurementProfiles.Queries.GetMeasurementProfiles
{
    public record GetMeasurementProfilesQuery(long PointId, AxisType AxisType) : IRequest<IEnumerable<MeasurementProfile>>;
}