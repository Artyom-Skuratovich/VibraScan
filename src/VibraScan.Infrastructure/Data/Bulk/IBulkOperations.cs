using VibraScan.Domain.Common;

namespace VibraScan.Infrastructure.Data.Bulk
{
    internal interface IBulkOperations
    {
        Task EnsureRangeAsync<T>(IEnumerable<T> entities, CancellationToken ct = default) where T : BaseEntity;
    }
}