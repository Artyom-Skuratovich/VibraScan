using AutoMapper;
using VibraScan.Domain.Entities;

namespace VibraScan.Application.Engines.Queries.GetEngines
{
    public class EngineBriefDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Engine, EngineBriefDto>();
            }
        }
    }
}