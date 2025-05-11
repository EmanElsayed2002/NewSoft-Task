using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<Role> role)
        {
            var roleManger = await role.Roles.CountAsync();
            if (roleManger == 0)
            {
                await role.CreateAsync(new Role()
                {
                    Name = "Instructor"
                });

                await role.CreateAsync(new Role()
                {
                    Name = "Student"
                });
            }

        }
    }
}
