using System.Data;

namespace IntegrationDemo.Persistence.Uow
{
    public interface IUnitOfWork : IDisposable
    {

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distributed = false);

        Task RollbackAsync(CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
