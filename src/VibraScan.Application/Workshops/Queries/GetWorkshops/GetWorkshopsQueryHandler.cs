using MediatR;
using Microsoft.EntityFrameworkCore;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Domain.Entities;

namespace VibraScan.Application.Workshops.Queries.GetWorkshops
{
    public class GetWorkshopsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetWorkshopsQuery, IEnumerable<Workshop>>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<IEnumerable<Workshop>> Handle(GetWorkshopsQuery request, CancellationToken ct)
        {
            return await _context.Workshops.AsNoTracking()
                                           .OrderBy(w => w.Name)
                                           .ToListAsync(ct);
        }
    }
}