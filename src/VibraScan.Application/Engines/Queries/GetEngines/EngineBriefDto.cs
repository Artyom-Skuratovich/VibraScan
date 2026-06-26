using AutoMapper;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.Engines.Queries.GetEngines
{
    public class EngineBriefDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public Condition Condition { get; set; } = null!;

        public DateTime? LastInspectionDate { get; set; }

        public DateTime? NextInspectionDate { get; set; }

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Engine, EngineBriefDto>();
            }
        }
    }
}