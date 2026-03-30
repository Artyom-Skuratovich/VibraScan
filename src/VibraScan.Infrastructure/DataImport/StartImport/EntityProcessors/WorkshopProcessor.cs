using VibraScan.Domain.Entities;
using VibraScan.Infrastructure.Data.Bulk;
using VibraScan.Infrastructure.DataImport.StartImport.Actions;
using VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors
{
    internal class WorkshopProcessor(IBulkOperations bulk, IEnumerable<IAfterSaveAction<Workshop>> actions) : EntityProcessor<Workshop>(bulk, actions)
    {
        public override string EntityName => "Folder";

        protected override bool TryMap(IReadOnlyDictionary<string, string> data, out string? rawId, out Workshop? entity, ImportContext context)
        {
            entity = null;

            rawId = data.GetValueOrDefault("C0");
            var name = data.GetValueOrDefault("C1");

            if (!string.IsNullOrWhiteSpace(rawId) && !string.IsNullOrWhiteSpace(name))
            {
                entity = new Workshop
                {
                    Name = name.Trim()
                };

                return true;
            }

            return false;
        }
    }
}