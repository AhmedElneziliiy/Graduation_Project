using Graduation.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Graduation.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager
        , RoleManager<AppRole> roleManager)
        {

            
            if (await roleManager.Roles.AnyAsync()) return;
            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            if (await userManager.Users.AnyAsync()) return;

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}
