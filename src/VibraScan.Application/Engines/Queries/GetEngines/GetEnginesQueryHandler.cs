using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;

namespace VibraScan.Application.Engines.Queries.GetEngines
{
    public class GetEnginesQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetEnginesQuery, IEnumerable<EngineBriefDto>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<EngineBriefDto>> Handle(GetEnginesQuery request, CancellationToken ct)
        {
            var query = _context.Engines.AsNoTracking()
                                        .Where(e => e.WorkshopId == request.WorkshopId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(e => e.Name.Contains(request.SearchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            if (request.Condition is not null)
            {
                query = query.Where(e => e.Condition == request.Condition);
            }

            if (request.DateType == InspectionDateType.LastInspection)
            {
                if (request.InspectionFrom.HasValue)
                {
                    query = query.Where(e => e.LastInspectionDate != null && e.LastInspectionDate >= request.InspectionFrom.Value);
                }

                if (request.InspectionTo.HasValue)
                {
                    query = query.Where(e => e.LastInspectionDate != null && e.LastInspectionDate <= request.InspectionTo.Value);
                }
            }
            else
            {
                if (request.InspectionFrom.HasValue)
                {
                    query = query.Where(e => e.NextInspectionDate != null && e.NextInspectionDate >= request.InspectionFrom.Value);
                }

                if (request.InspectionTo.HasValue)
                {
                    query = query.Where(e => e.NextInspectionDate != null && e.NextInspectionDate <= request.InspectionTo.Value);
                }
            }

            if (request.InspectionStatus != InspectionStatusFilter.All)
            {
                var today = DateTime.Today;

                query = request.InspectionStatus switch
                {
                    InspectionStatusFilter.Valid => query.Where(e => e.NextInspectionDate.HasValue && e.NextInspectionDate >= today),
                    InspectionStatusFilter.Overdue => query.Where(e => e.NextInspectionDate == null || e.NextInspectionDate < today),
                    _ => query
                };
            }

            return await query.OrderBy(e => e.NextInspectionDate ?? DateTime.MaxValue)
                              .ProjectTo<EngineBriefDto>(_mapper.ConfigurationProvider)
                              .ToListAsync(ct);
        }
    }
}