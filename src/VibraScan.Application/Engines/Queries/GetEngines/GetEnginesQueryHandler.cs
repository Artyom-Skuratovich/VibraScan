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

            if (request.LastInspectionFrom.HasValue)
            {
                query = query.Where(e => e.LastInspectionDate >= request.LastInspectionFrom.Value);
            }

            if (request.LastInspectionTo.HasValue)
            {
                query = query.Where(e => e.LastInspectionDate <= request.LastInspectionTo.Value);
            }

            return await query.OrderBy(e => e.Name)
                              .ProjectTo<EngineBriefDto>(_mapper.ConfigurationProvider)
                              .ToListAsync(ct);
        }
    }
}