using MediatR;
using VibraScan.Domain.Entities;

namespace VibraScan.Application.Workshops.Queries.GetWorkshops
{
    public record GetWorkshopsQuery : IRequest<IEnumerable<Workshop>>;
}