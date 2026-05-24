namespace AppointmentManagementSystem.ViewModels.Appointments
{
    public class AdminDashboardViewModael
    {
        public int TodaysAppointments { get; set; }
        public int NoOfDoctors { get; set; }
        public IEnumerable<AppointmentViewModel> RecentAppointments { get; set; } = new List<AppointmentViewModel>();
    }
}
