using VibraScan.Domain.Entities;
using VibraScan.Infrastructure.Data.Bulk;
using VibraScan.Infrastructure.DataImport.StartImport.Actions;
using VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors
{
    internal class PointProcessor(IBulkOperations bulk, IEnumerable<IAfterSaveAction<Point>> actions) : EntityProcessor<Point>(bulk, actions)
    {
        public override string EntityName => "Point";

        protected override bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out Point? entity, ImportContext context)
        {
            entity = null;

            rawId = data.GetValueOrDefault("C0");
            var engineId = data.GetValueOrDefault("C1");
            var name = data.GetValueOrDefault("C2");

            if (!string.IsNullOrWhiteSpace(rawId) && !string.IsNullOrWhiteSpace(engineId) && !string.IsNullOrWhiteSpace(name))
            {
                var engine = context.Get<Engine>(engineId)
                             ?? throw new ImportInconsistencyException($"Сущность Machine с Id='{engineId}' не найдена для сущности Point с Id='{rawId}'");

                entity = new Point
                {
                    Name = name,
                    EngineId = engine.Id
                };

                return true;
            }

            return false;
        }
    }
}