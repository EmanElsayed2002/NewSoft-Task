using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class SubjectConfig : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.Property(s => s.Name).IsRequired();
            builder.HasKey(s => s.Id);

            builder.HasMany(s => s.studentSubjects).WithOne(s => s.Subject).HasForeignKey(x => x.SubjectId);
        }
    }
}
