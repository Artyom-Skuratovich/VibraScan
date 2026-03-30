using VibraScan.Domain.Common;

namespace VibraScan.Infrastructure.DataImport.StartImport.Actions
{
    internal interface IAfterSaveAction<T> where T : BaseEntity
    {
        Task ExecuteAsync(IEnumerable<T> entities, CancellationToken ct = default);
    }
}