using MediatR;
using VibraScan.Domain.Entities;

namespace VibraScan.Application.Points.Queries.GetPoints
{
    public record GetPointsQuery(long EngineId) : IRequest<IEnumerable<Point>>;
}