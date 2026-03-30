using VibraScan.Domain.Entities;
using VibraScan.Infrastructure.Data.Bulk;
using VibraScan.Infrastructure.DataImport.StartImport.Actions;
using VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors
{
    internal class EngineProcessor(IBulkOperations bulk, IEnumerable<IAfterSaveAction<Engine>> actions) : EntityProcessor<Engine>(bulk, actions)
    {
        public override string EntityName => "Machine";

        protected override bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out Engine? entity, ImportContext context)
        {
            entity = null;

            rawId = data.GetValueOrDefault("C0");
            var workshopId = data.GetValueOrDefault("C1");
            var name = data.GetValueOrDefault("C2");

            if (!string.IsNullOrWhiteSpace(rawId) && !string.IsNullOrWhiteSpace(workshopId) && !string.IsNullOrWhiteSpace(name))
            {
                var workshop = context.Get<Workshop>(workshopId)
                               ?? throw new ImportInconsistencyException($"Сущность Folder с Id='{workshopId}' не найдена для сущности Machine с Id='{rawId}'");

                entity = new Engine
                {
                    Name = name,
                    WorkshopId = workshop.Id
                };

                return true;
            }

            return false;
        }
    }
}