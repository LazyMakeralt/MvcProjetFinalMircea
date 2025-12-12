using Microsoft.AspNetCore.Identity;

namespace MvcProjetFinalMircea.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRoles(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "Auteur" };

            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
