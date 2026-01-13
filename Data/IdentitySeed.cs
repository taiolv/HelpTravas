using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SuporteIA.Models;

namespace SuporteIA.Data
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            var roles = new[] { "Usuario", "Tecnico", "Administrador" };
            var rm = sp.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var r in roles)
                if (!await rm.RoleExistsAsync(r))
                    await rm.CreateAsync(new IdentityRole(r));


            var um = sp.GetRequiredService<UserManager<AppUser>>();
            var adminEmail = "admin@suporte.local";
            var admin = await um.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, NomeCompleto = "Administrador" };
                await um.CreateAsync(admin, "Admin@123!");
                await um.AddToRolesAsync(admin, roles); 
            }
        }
    }
}
