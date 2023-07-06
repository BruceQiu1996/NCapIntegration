using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace NCapIntegration.Persistence.Uow
{
    /// <summary>
    /// TODO:可以为不同的数据库声明一个不同的dbcontext基类用以区分和约束。
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public class MssqlUnitOfWork<TDbContext> : UnitOfWork<TDbContext> where TDbContext : DbContext
    {
        private readonly ICapPublisher _publisher;

        public MssqlUnitOfWork(TDbContext dbContext, ICapPublisher publisher) : base(dbContext)
        {
            _publisher = publisher;
        }

        public override IDbContextTransaction GetDbContextTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, bool distribute = false)
        {
            if (distribute && _publisher is null) //需要分布式事务，但是_publisher为null
            {
                throw new InvalidOperationException("xxx");
            }

            return distribute ?
                _dbContext.Database.BeginTransaction(isolationLevel, _publisher, false)
                :
                _dbContext.Database.BeginTransaction(isolationLevel);
        }
    }
}
