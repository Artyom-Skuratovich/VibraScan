using VibraScan.Domain.Common;
using VibraScan.Infrastructure.Data.Bulk;
using VibraScan.Infrastructure.DataImport.StartImport.Actions;

namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base
{
    internal abstract class EntityProcessor<T>(IBulkOperations bulk, IEnumerable<IAfterSaveAction<T>> actions) : AbstractEntityProcessor<T> where T : BaseEntity
    {
        private readonly IBulkOperations _bulk = bulk;
        private readonly IEnumerable<IAfterSaveAction<T>> _actions = actions;

        public override async Task CommitAsync(ImportContext context, CancellationToken ct = default)
        {
            var entities = context.GetPending<T>();
            if (entities.Count == 0) return;

            await _bulk.EnsureRangeAsync(entities, ct);

            foreach (var action in _actions)
            {
                await action.ExecuteAsync(entities, ct);
            }

            context.ClearPending<T>();
        }

        protected override void OnProcessed(string rawId, T entity, ImportContext context)
        {
            context.Store(rawId, entity);
            context.AddToPending(entity);
        }
    }
}