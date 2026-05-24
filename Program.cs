using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.Services.Email;
using AppointmentManagementSystem.ViewModels.Settings;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem
{
    /// <summary>
    /// Application entry point. Configures Identity cookie auth, EF Core, Hangfire, and MVC routing.
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // SQL Server + EF Core for Identity users and appointments.
            builder.Services.AddDbContext<AppointmentManagementSystemDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Hangfire uses the same database for background jobs (emails, reminders).
            builder.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddHangfireServer();

            // Cookie-based authentication. Lockout is strict: one failed attempt triggers a 5-minute lockout.
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 1;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<AppointmentManagementSystemDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Accounts/Login";
                options.AccessDeniedPath = "/Accounts/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

            builder.Services.AddAuthorization();

            builder.Services.AddTransient<IAppointmentService, AppointmentService>();
            builder.Services.Configure<MailjetSettings>(
                builder.Configuration.GetSection("MailJetSettings"));
            builder.Services.AddScoped<IEmailService, MailjetEmailService>();

            var app = builder.Build();

            await IdentityDataSeeder.SeedRolesAsync(app.Services);

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            // Restrict Hangfire dashboard to admins in non-development environments.
            app.UseHangfireDashboard("/hangfire", new Hangfire.DashboardOptions
            {
                Authorization = [new HangfireAdminAuthorizationFilter()]
            });

            app.MapStaticAssets();
            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            await app.RunAsync();
        }
    }
}
