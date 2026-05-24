using AppointmentManagementSystem.ViewModels;

namespace AppointmentManagementSystem.Views.Home
{
    public class DoctorDashboardViewModel
    {
        public int TotalAppointments { get; set; }

        public int TodaysAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int ApprovedAppointments { get; set; }

        public IEnumerable<AppointmentViewModel> RecentAppointments { get; set; }
            = new List<AppointmentViewModel>();
    }
}
