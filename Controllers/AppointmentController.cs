using AppointmentManagementSystem.Helpers;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.ViewModels.Appointments;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppointmentManagementSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointment;
        private readonly UserManager<ApplicationUser> _user;

        public AppointmentController(IAppointmentService appointment, UserManager<ApplicationUser> user)
        {
            _appointment = appointment;
            _user = user;
        }

        public async Task<IActionResult> AvailableDoctors()
        {
            var doctor = await _appointment.GetAllDoctors();
            return View(doctor);
        }

        public async Task<IActionResult> PatientAppointments()
        {
            var patient = await _appointment.GetAllPatients();
            return View(
                    await _appointment.PatientsEventById(
                        User!.FindFirstValue(ClaimTypes.NameIdentifier)
                    )
                );
        }

        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {

            var result = await _appointment.CancelAppointment(appointmentId);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction("PatientAppointments");
        }

        public async Task<IActionResult> RecentAppointments()
        {
            var results =  await _appointment.GetRecentAppointments();
            return View(results);
        }

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





        public async Task<IActionResult> Index()
        {
            ViewBag.DoctorList = await _appointment.GetAllDoctors();
            ViewBag.PatientList = await _appointment.GetAllPatients();
            ViewBag.Durations = ConstHelper.GetTimeDropDown();
            return View();
        }
    }
}
