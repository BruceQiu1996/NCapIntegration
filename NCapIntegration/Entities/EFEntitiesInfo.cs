using System.Reflection;

namespace NCapIntegration.Entities
{
    public class EFEntitiesInfo
    {
        public (Assembly assembly, Type[] entities) GetEFEntitiesAndAssembly()
        {
            var assembly = this.GetType().Assembly;
            var types = assembly.GetTypes().Where(m => m.FullName != null
                                                        && Array.Exists(m.GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEFEntity<>))
                                                        && !m.IsAbstract && !m.IsInterface).ToArray();

            return (assembly, types);
        }
    }
}
