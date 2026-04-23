using Microsoft.AspNetCore.Identity;
using PortalAcademicoWeb.Models;

namespace PortalAcademicoWeb.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync("Coordinador"))
            await roleManager.CreateAsync(new IdentityRole("Coordinador"));

        var email = "coordinador@universidad.edu";
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                NombreCompleto = "Coordinador Académico",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, "Admin123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "Coordinador");
        }
    }
}