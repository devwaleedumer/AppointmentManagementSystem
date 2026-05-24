using AppointmentManagementSystem.Helpers;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.ViewModels.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppointmentManagementSystem.Controllers
{
    /// <summary>
    /// MVC pages for browsing doctors, booking visits, and viewing the FullCalendar schedule.
    /// </summary>
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointment;
        private readonly UserManager<ApplicationUser> _user;

        public AppointmentController(IAppointmentService appointment, UserManager<ApplicationUser> user)
        {
            _appointment = appointment;
            _user = user;
        }

        [Authorize(Roles = ConstHelper.PatientRole)]
        public async Task<IActionResult> AvailableDoctors()
        {
            var doctor = await _appointment.GetAllDoctors();
            return View(doctor);
        }

        [Authorize(Roles = ConstHelper.PatientRole)]
        public async Task<IActionResult> PatientAppointments()
        {
            return View(
                await _appointment.PatientsEventById(
                    User!.FindFirstValue(ClaimTypes.NameIdentifier)!));
        }

        [Authorize(Roles = ConstHelper.PatientRole)]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var result = await _appointment.CancelAppointment(appointmentId);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(PatientAppointments));
        }

        [Authorize(Roles = ConstHelper.AdminRole)]
        public async Task<IActionResult> RecentAppointments()
        {
            var results = await _appointment.GetRecentAppointments();
            return View(results);
        }

        [Authorize(Roles = ConstHelper.PatientRole)]
        public async Task<IActionResult> BookAppointment(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                return NotFound();
            }

            var doctor = await _user.FindByIdAsync(doctorId);
            if (doctor is null)
            {
                return NotFound();
            }

            var model = new DoctorViewModel
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
            };

            ViewBag.Durations = ConstHelper.GetTimeDropDown();

            return View(model);
        }

        [Authorize(Roles = ConstHelper.AdminAndDoctor)]
        public async Task<IActionResult> Index()
        {
            ViewBag.DoctorList = await _appointment.GetAllDoctors();
            ViewBag.PatientList = await _appointment.GetAllPatients();
            ViewBag.Durations = ConstHelper.GetTimeDropDown();
            return View();
        }
    }
}
