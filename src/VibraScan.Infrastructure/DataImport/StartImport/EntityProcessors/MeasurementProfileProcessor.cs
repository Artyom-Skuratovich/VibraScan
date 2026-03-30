using VibraScan.Domain.Entities;
using VibraScan.Infrastructure.Data.Bulk;
using VibraScan.Infrastructure.DataImport.StartImport.Actions;
using VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors
{
    internal class MeasurementProfileProcessor(IBulkOperations bulk, IEnumerable<IAfterSaveAction<MeasurementProfile>> actions)
        : EntityProcessor<MeasurementProfile>(bulk, actions)
    {
        public override string EntityName => "ParamSet";

        protected override bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out MeasurementProfile? entity, ImportContext context)
        {
            entity = null;

            rawId = data.GetValueOrDefault("C0");
            var description = data.GetValueOrDefault("C4");

            if (!string.IsNullOrWhiteSpace(rawId))
            {
                entity = new MeasurementProfile
                {
                    Description = description ?? string.Empty
                };

                return true;
            }

            return false;
        }
    }
}