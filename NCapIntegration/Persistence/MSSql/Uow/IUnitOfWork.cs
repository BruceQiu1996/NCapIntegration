using System.Data;

namespace NCapIntegration.Persistence.MSSql.Uow
{
    public interface IUnitOfWork : IDisposable
    {

        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distributed = false);

        Task RollbackAsync(CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
