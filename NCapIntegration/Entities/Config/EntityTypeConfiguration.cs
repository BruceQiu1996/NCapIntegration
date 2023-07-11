using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NCapIntegration.Entities.Config
{
    public abstract class EntityTypeConfiguration<T, TKey> : IEntityTypeConfiguration<T> where T : class, IEFEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
            if (typeof(IEFSoftDelete).IsAssignableFrom(typeof(T)))
            {
                builder.HasQueryFilter(d => EF.Property<bool>(d, "IsDeleted") == false);
            }
        }
    }
}
