using Autofac;
using MongoDB.Driver;

namespace NCapIntegration.Persistence.MongoDB
{
    internal static class ServiceExtension
    {
        public static void AddMongoDb(this ContainerBuilder containerBuilder)
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("NCapIntegration");
            containerBuilder.RegisterType<CollectionProvider>()
                .SingleInstance().AsSelf()
                .UsingConstructor(typeof(IMongoDatabase)).WithParameter("mongoDatabase", database);
        }
    }
}
