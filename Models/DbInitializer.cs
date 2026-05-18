using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Models
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            // 1. Създаваме ролите, ако не съществуват
            //string[] roles = { "Admin", "User" };
            //foreach (var role in roles)
            //{
            //    if (!await roleManager.RoleExistsAsync(role))
            //    {
            //        await roleManager.CreateAsync(new IdentityRole(role));
            //    }
            //}

            // 2. Създаваме администратор, ако не съществува
            //string adminEmail = "admin@example.com";
            //string adminPassword = "Admin123!"; // сменя се на по-сигурна парола
            //if (await userManager.FindByEmailAsync(adminEmail) == null)
            //{
            //    var admin = new IdentityUser
            //    {
            //        UserName = adminEmail,
            //        Email = adminEmail,
            //        EmailConfirmed = true
            //    };
            //    var result = await userManager.CreateAsync(admin, adminPassword);
            //    if (result.Succeeded)
            //    {
            //        await userManager.AddToRoleAsync(admin, "Admin");
            //    }
            //}

        }
    }
}
