using MongoDB.Driver;
using System.Collections.Concurrent;

namespace NCapIntegration.Persistence.MongoDB
{
    //获取数据表
    public class CollectionProvider
    {
        private readonly ConcurrentDictionary<Type, object> _collections = new ConcurrentDictionary<Type, object>();
        private readonly IMongoDatabase _mongoDatabase;
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public CollectionProvider(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<IMongoCollection<T>> GetCollection<T>()
        {
            try
            {
                await semaphore.WaitAsync();
                if (_collections.Keys.Contains(typeof(T)))
                {
                    return _collections[typeof(T)] as IMongoCollection<T>;
                }
                
                var collection = _mongoDatabase.GetCollection<T>($"{typeof(T).Name}s");
                _collections.TryAdd(typeof(T), collection);

                return collection;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
