using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.Property(u => u.UserName).IsRequired();
            builder.HasMany(u => u.studentSubjects).WithOne(u => u.User).HasForeignKey(x => x.UserId);

        }
    }
}
