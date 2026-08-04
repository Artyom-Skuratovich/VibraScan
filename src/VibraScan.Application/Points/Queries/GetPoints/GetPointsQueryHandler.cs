using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.Entities;

namespace VibraScan.Application.Points.Queries.GetPoints
{
    public class GetPointsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetPointsQuery, IEnumerable<Point>>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<IEnumerable<Point>> Handle(GetPointsQuery request, CancellationToken ct)
        {
            return await _context.Points.AsNoTracking()
                                        .Where(p => p.EngineId == request.EngineId)
                                        .OrderBy(p => p.Name)
                                        .ToListAsync(ct);
        }
    }
}