using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace IntegrationDemo.Persistence.Uow
{
    public abstract class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        protected readonly TDbContext _dbContext;
        protected IDbContextTransaction? DbTransaction { get; private set; }

        public UnitOfWork(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected bool IsStartingUow => _dbContext.Database.CurrentTransaction != null;

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distribute = false)
        {
            if (IsStartingUow)
                throw new InvalidOperationException($"xxx");

            DbTransaction = GetDbContextTransaction();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await DbTransaction?.CommitAsync();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await DbTransaction?.RollbackAsync();
        }

        public void Dispose()
        {
            DbTransaction?.Dispose();
            DbTransaction = null;
        }

        public abstract IDbContextTransaction GetDbContextTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distribute = false);
    }
}
