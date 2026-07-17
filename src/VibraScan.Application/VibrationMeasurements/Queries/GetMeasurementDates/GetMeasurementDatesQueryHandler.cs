using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;

namespace VibraScan.Application.VibrationMeasurements.Queries.GetMeasurementDates
{
    public class GetMeasurementDatesQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMeasurementDatesQuery, IEnumerable<DateTime>>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<IEnumerable<DateTime>> Handle(GetMeasurementDatesQuery request, CancellationToken ct)
        {
            return await _context.VibrationMeasurements.Where(v => (v.PointId == request.PointId) && (v.MeasurementProfileId == request.MeasurementProfileId) && (v.AxisType == request.AxisType))
                                                       .Select(v => v.CapturedAt)
                                                       .Distinct()
                                                       .ToListAsync(ct);
        }
    }
}