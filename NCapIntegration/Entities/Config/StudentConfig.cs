using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NCapIntegration.Entities.Config
{
    public class StudentConfig : EntityTypeConfiguration<Student, int>
    {
        public override void Configure(EntityTypeBuilder<Student> builder)
        {
            base.Configure(builder);

            builder.Property(x=>x.Name).HasMaxLength(50);
            builder.Property(x => x.Address).HasMaxLength(50);
        }
    }
}
