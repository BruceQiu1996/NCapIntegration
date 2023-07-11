using Microsoft.EntityFrameworkCore;
using NCapIntegration.Entities;

namespace NCapIntegration.Persistence.MSSql
{
    public class NCapIntegrationDbContext : DbContext
    {
        private readonly EFEntitiesInfo _efEntitysInfo;

        public NCapIntegrationDbContext(DbContextOptions options,EFEntitiesInfo entitiesInfo) : base(options)
        {
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
            _efEntitysInfo = entitiesInfo;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var (Assembly, Types) = _efEntitysInfo.GetEFEntitiesAndAssembly();
            foreach (var entityType in Types)
            {
                modelBuilder.Entity(entityType);
            }
            //只需要将配置类所在的程序集给到，它会自动加载
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var flag = false;

            if (Database.AutoTransactionBehavior == AutoTransactionBehavior.Never
                && ChangeTracker.Entries().Count() > 1
                && Database.CurrentTransaction == null)
            {
                Database.AutoTransactionBehavior = AutoTransactionBehavior.Always;
                flag = true;
            }

            var result = base.SaveChangesAsync(cancellationToken);
            if (flag)
                Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

            return result;
        }
    }
}
