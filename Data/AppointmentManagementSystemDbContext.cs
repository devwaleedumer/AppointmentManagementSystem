using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Data
{
    /// <summary>
    /// EF Core context for Identity tables and application appointments.
    /// </summary>
    public class AppointmentManagementSystemDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Appointment> Appointments { get; set; }

        public AppointmentManagementSystemDbContext(DbContextOptions<AppointmentManagementSystemDbContext> options)
            : base(options)
        {
        }
    }
}
