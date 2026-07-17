using MediatR;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.AxisTypes.Queries.GetAxisTypes
{
    public record GetAxisTypesQuery(long PointId) : IRequest<IEnumerable<AxisType>>;
}