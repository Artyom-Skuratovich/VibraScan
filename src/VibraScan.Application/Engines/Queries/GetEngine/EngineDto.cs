using AutoMapper;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.Engines.Queries.GetEngine
{
    public class EngineDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public Condition Condition { get; set; } = null!;

        public DateTime? LastInspectionDate { get; set; }

        public DateTime? NextInspectionDate { get; set; }

        public string WorkshopName { get; set; } = null!;

        public InspectionRule Rule { get; set; } = null!;

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Engine, EngineDto>()
                    .ForMember(e => e.Rule, o => o.Ignore())
                    .ForMember(e => e.WorkshopName, o => o.Ignore());
            }
        }
    }
}