using Microsoft.EntityFrameworkCore;
using NCapIntegration.Entities;

namespace NCapIntegration.Persistence.MSSql
{
    public class NCapIntegrationDbContext : DbContext
    {
        public NCapIntegrationDbContext(DbContextOptions options) : base(options)
        {
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

        public DbSet<Student> Students { get; set; }
    }
}
