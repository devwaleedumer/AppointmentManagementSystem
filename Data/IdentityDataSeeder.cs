using AppointmentManagementSystem.Helpers;
using Microsoft.AspNetCore.Identity;

namespace AppointmentManagementSystem.Data
{
    /// <summary>
    /// Ensures Identity roles exist before users are assigned to them.
    /// </summary>
    public static class IdentityDataSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles =
            [
                ConstHelper.AdminRole,
                ConstHelper.DoctorRole,
                ConstHelper.PatientRole
            ];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
