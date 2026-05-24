using AppointmentManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Data
{
    public class AppointmentManagementSystemDbContext: IdentityDbContext<ApplicationUser>
    {
        public DbSet<Appointment> Appointments { get; set; }
        public AppointmentManagementSystemDbContext(DbContextOptions<AppointmentManagementSystemDbContext> options) : base(options)
        {
        }
    }
}
