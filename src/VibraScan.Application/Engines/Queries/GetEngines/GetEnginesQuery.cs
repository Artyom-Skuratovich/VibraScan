using MediatR;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.Engines.Queries.GetEngines
{
    public record GetEnginesQuery(
        long WorkshopId,
        string? SearchTerm = null,
        Condition? Condition = null,
        DateTime? LastInspectionFrom = null,
        DateTime? LastInspectionTo = null) : IRequest<IEnumerable<EngineBriefDto>>;
}