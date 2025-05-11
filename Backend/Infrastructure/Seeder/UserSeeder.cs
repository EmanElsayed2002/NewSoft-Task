using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeder
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<User> user)
        {
            if (await user.Users.CountAsync() == 0)
            {
                var userDta = new User()
                {
                    UserName = "Eman",
                    FullName = "Eman Elsayed",
                    Email = "emy2192002@gmail.com",
                    Address = "Egype,Cairo",
                    EmailConfirmed = true,
                    Phone = "098765432",

                };
                await user.CreateAsync(userDta, "EmanElsayed21");
                await user.AddToRoleAsync(userDta, "Instructor");

            }
        }
    }
}
