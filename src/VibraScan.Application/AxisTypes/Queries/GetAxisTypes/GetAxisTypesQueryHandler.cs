using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.AxisTypes.Queries.GetAxisTypes
{
    public class GetAxisTypesQueryHandler(IApplicationDbContext context) : IRequestHandler<GetAxisTypesQuery, IEnumerable<AxisType>>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<IEnumerable<AxisType>> Handle(GetAxisTypesQuery request, CancellationToken ct)
        {
            return await _context.VibrationMeasurements.AsNoTracking()
                                                       .Where(v => v.PointId == request.PointId)
                                                       .GroupBy(v => v.AxisType)
                                                       .Select(g => g.Key)
                                                       .ToListAsync(ct);
        }
    }
}