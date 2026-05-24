using AppointmentManagementSystem.Helpers;
using Hangfire.Dashboard;

namespace AppointmentManagementSystem
{
    /// <summary>
    /// Allows only authenticated users in the Admin role to open the Hangfire dashboard.
    /// </summary>
    public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity?.IsAuthenticated == true
                && httpContext.User.IsInRole(ConstHelper.AdminRole);
        }
    }
}
