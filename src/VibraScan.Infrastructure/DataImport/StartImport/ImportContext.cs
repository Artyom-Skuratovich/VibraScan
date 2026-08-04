using VibraScan.Domain.Common;

namespace VibraScan.Infrastructure.DataImport.StartImport
{
    internal class ImportContext
    {
        private readonly Dictionary<Type, Dictionary<string, object>> _identityMap = [];
        private readonly Dictionary<Type, object> _pendingCollections = [];

        public void Store<T>(string key, T entity) where T : class
        {
            if (string.IsNullOrEmpty(key)) return;

            var type = typeof(T);

            if (!_identityMap.TryGetValue(type, out var lookup))
            {
                lookup = [];
                _identityMap[type] = lookup;
            }

            lookup[key] = entity;
        }

        public T? Get<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key)) return null;

            return _identityMap.TryGetValue(typeof(T), out var lookup) &&
                lookup.TryGetValue(key, out var entity)
                ? (T)entity : null;
        }

        public void AddToPending<T>(T entity) where T : BaseEntity
        {
            var type = typeof(T);

            if (!_pendingCollections.TryGetValue(type, out var list))
            {
                list = new List<T>();
                _pendingCollections[type] = list;
            }

            ((List<T>)list).Add(entity);
        }

        public List<T> GetPending<T>() where T : BaseEntity
        {
            return _pendingCollections.TryGetValue(typeof(T), out var list)
                ? (List<T>)list : [];
        }

        public void ClearPending<T>() where T : BaseEntity
        {
            if (_pendingCollections.TryGetValue(typeof(T), out var list))
            {
                ((List<T>)list).Clear();
            }
        }
    }
}