using MediatR;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.Engines.Queries.GetEngines
{
    public record GetEnginesQuery(
        long WorkshopId,
        string? SearchTerm = null,
        Condition? Condition = null,
        DateTime? InspectionFrom = null,
        DateTime? InspectionTo = null,
        InspectionDateType DateType = InspectionDateType.LastInspection,
        InspectionStatusFilter InspectionStatus = InspectionStatusFilter.All) : IRequest<IEnumerable<EngineBriefDto>>;
}