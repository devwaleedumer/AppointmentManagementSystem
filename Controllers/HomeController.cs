using AppointmentManagementSystem.Helpers;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.ViewModels.Appointments;
using AppointmentManagementSystem.Views.Home;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AppointmentManagementSystem.Controllers
{
    /// <summary>
    /// Landing page for guests; role-specific dashboards for authenticated users.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public HomeController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View(); // _LandingPage partial
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Each role gets a different dashboard view bound to its own view model.
            if (User.IsInRole(ConstHelper.PatientRole))
            {
                var patientDashboardViewModel = new PatientDashboardViewModel();
                patientDashboardViewModel.TotalAppointments = await _appointmentService.GetTotalAppointmentsByPatientId(userId);
                patientDashboardViewModel.UpcomingAppointments = await _appointmentService.GetUpcomingAppointmentsByPatientId(userId);
                patientDashboardViewModel.ApprovedAppointments = await _appointmentService.GetApprovedAppointmentsByPatientId(userId);
                return View(
                    patientDashboardViewModel
                
                );
            }

            if (User.IsInRole(ConstHelper.AdminRole))
            {
                var adminDashboardViewModel = new AdminDashboardViewModael();
                adminDashboardViewModel.TodaysAppointments = await _appointmentService.GetTodaysAppointments();
                adminDashboardViewModel.NoOfDoctors = await _appointmentService.GetTotalNoOfDoctors();
                adminDashboardViewModel.RecentAppointments = await _appointmentService.GetRecentAppointments();
                // Populate recent appointments if needed
                return View(adminDashboardViewModel);
            }

            if (User.IsInRole(ConstHelper.DoctorRole))
            {
                var doctorDashboardViewModel = new DoctorDashboardViewModel();
                doctorDashboardViewModel.TotalAppointments = await _appointmentService.GetTotalAppointmentsByDoctorId(userId);
                doctorDashboardViewModel.RecentAppointments = await _appointmentService.GetRecentAppointmentsByDoctorId(userId);
                doctorDashboardViewModel.ApprovedAppointments = await _appointmentService.GetApprovedAppointmentsByDoctorId(userId);
                doctorDashboardViewModel.TodaysAppointments = await _appointmentService.GetTodaysAppointments();
                return View(doctorDashboardViewModel);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
