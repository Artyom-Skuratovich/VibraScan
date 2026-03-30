namespace VibraScan.Infrastructure.DataImport.StartImport.EntityProcessors.Base
{
    internal abstract class ProxyProcessor<T> : AbstractEntityProcessor<T> where T : class
    {
        public override Task CommitAsync(ImportContext context, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        protected override void OnProcessed(string rawId, T entity, ImportContext context)
        {
            context.Store(rawId, entity);
        }
    }
}