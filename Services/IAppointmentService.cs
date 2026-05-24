using AppointmentManagementSystem.ViewModels;
using AppointmentManagementSystem.ViewModels.Appointments;

namespace AppointmentManagementSystem.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<DoctorViewModel>> GetAllDoctors();
        Task<IEnumerable<PatientViewModel>> GetAllPatients();
        Task<int> AddOrUpdatePatient(AppointmentViewModel model);
        Task<IEnumerable<AppointmentViewModel>> DoctorEventsById(string doctorId);
        Task<IEnumerable<AppointmentViewModel>> PatientsEventById(string patientId);
        Task<AppointmentViewModel> GetById(int id);
        Task<IEnumerable<AppointmentViewModel>> GetRecentAppointments();
        Task<int> GetTotalAppointmentsByPatientId(string patientId);
        Task<int> GetUpcomingAppointmentsByPatientId(string patientId);
        Task<int> GetApprovedAppointmentsByPatientId(string patientId);
        Task<bool> CancelAppointment(int appointmentId);
        Task<int> GetTotalNoOfDoctors();
        Task<int> GetTodaysAppointments();
        Task<int> DeleteAppointment(int appointmentId);
        Task<bool> AcceptAppointment(int appointmentId);
        Task<IEnumerable<AppointmentViewModel>> GetRecentAppointmentsByDoctorId(string doctorId);
        Task<int> GetTotalAppointmentsByDoctorId(string doctorId);
        Task<int> GetUpcomingAppointmentsByDoctorId(string doctorId);
        Task<int> GetApprovedAppointmentsByDoctorId(string doctorId);
        Task<int> GetCancelledAppointmentsByDoctorId(string doctorId);


    }
}