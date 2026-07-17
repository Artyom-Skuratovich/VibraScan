using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.Entities;

namespace VibraScan.Application.MeasurementProfiles.Queries.GetMeasurementProfiles
{
    public class GetMeasurementProfilesQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMeasurementProfilesQuery, IEnumerable<MeasurementProfile>>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<IEnumerable<MeasurementProfile>> Handle(GetMeasurementProfilesQuery request, CancellationToken ct)
        {
            var profileIdsQuery = _context.VibrationMeasurements.Where(v => (v.PointId == request.PointId) && (v.AxisType == request.AxisType))
                                                                .Select(v => v.MeasurementProfileId)
                                                                .Distinct();

            return await _context.MeasurementProfiles.AsNoTracking()
                                                     .Where(p => profileIdsQuery.Contains(p.Id))
                                                     .ToListAsync(ct);
        }
    }
}