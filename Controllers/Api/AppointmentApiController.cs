using AppointmentManagementSystem.Helpers;
using AppointmentManagementSystem.Services;
using AppointmentManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppointmentManagementSystem.Controllers.Api
{
    /// <summary>
    /// JSON endpoints for FullCalendar: load, create/update, approve, cancel, and delete appointments.
    /// Route prefix resolves to /api/AppointmentApi because of the controller class name.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentApiController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentApiController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Roles = ConstHelper.AdminAndDoctor)]
        [HttpDelete]
        [Route("DeleteCalendarData/{id:int}")]
        public async Task<IActionResult> DeleteCalendarData([FromRoute] int id)
        {
            CommonResponse<int> response = new();
            try
            {
                response.Status = await _appointmentService.DeleteAppointment(id);
                if (response.Status == 1)
                {
                    response.Message = ConstHelper.AppointmentDeleted;
                }
                else
                {
                    response.Message = ConstHelper.AppointmentNotFound;
                }
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Status = ConstHelper.FailedCode;
            }

            return Ok(response);
        }

        [Authorize(Roles = ConstHelper.AdminAndDoctor)]
        [Route("AcceptAppointment/{id:int}")]
        public async Task<IActionResult> AcceptAppointment(int id)
        {
            CommonResponse<int> response = new();
            try
            {
                var result = await _appointmentService.AcceptAppointment(id);
                if (result == true)
                {
                    response.Message = "Appointment accepted successfully.";
                }
                else
                {
                    response.Message = ConstHelper.AppointmentNotFound;
                }
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Status = ConstHelper.FailedCode;
            }

            return Ok(response);
        }

        [Authorize(Roles = ConstHelper.AllRoles)]
        [Route("CancelAppointment/{id:int}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            CommonResponse<int> response = new();
            try
            {
                var result = await _appointmentService.CancelAppointment(id);
                if (result == true)
                {
                    response.Message = "Appointment cancelled successfully.";
                }
                else
                {
                    response.Message = ConstHelper.AppointmentNotFound;
                }
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Status = ConstHelper.FailedCode;
            }

            return Ok(response);
        }

        [Authorize(Roles = ConstHelper.AllRoles)]
        [HttpPost]
        [Route("SaveCalendarData")]
        public async Task<IActionResult> SaveCalendarData(AppointmentViewModel model)
        {
            CommonResponse<int> response = new();

            try
            {
                if (User.IsInRole(ConstHelper.DoctorRole))
                {
                    model.DoctorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                }

                response.Status = await _appointmentService.AddOrUpdatePatient(model);
                if (response.Status == 1)
                {
                    response.Message = ConstHelper.AppointmentUpdated;
                }
                else if (response.Status == 2)
                {
                    response.Message = ConstHelper.AppointmentAdded;
                }
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Status = ConstHelper.FailedCode;
            }

            return Ok(response);
        }

        [Authorize(Roles = ConstHelper.AllRoles)]
        [HttpGet]
        [Route("GetCalendarData")]
        public async Task<IActionResult> GetCalendarData(string doctorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var response = new CommonResponse<IEnumerable<AppointmentViewModel>>();

            try
            {
                if (role == ConstHelper.DoctorRole)
                {
                    response.Data = await _appointmentService.DoctorEventsById(userId!);
                    response.Status = ConstHelper.SucceededCode;
                }
                else if (role == ConstHelper.PatientRole)
                {
                    response.Data = await _appointmentService.PatientsEventById(userId!);
                    response.Status = ConstHelper.SucceededCode;
                }
                else if (role == ConstHelper.AdminRole)
                {
                    response.Data = await _appointmentService.DoctorEventsById(doctorId);
                    response.Status = ConstHelper.SucceededCode;
                }
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Status = ConstHelper.FailedCode;
            }

            return Ok(response);
        }

        [Authorize(Roles = ConstHelper.AllRoles)]
        [HttpGet]
        [Route("GetCalendarDataById/{id:int}")]
        public async Task<IActionResult> GetCalendarDataById([FromRoute] int id)
        {
            var response = new CommonResponse<AppointmentViewModel>();

            try
            {
                response.Data = await _appointmentService.GetById(id);
                response.Status = ConstHelper.SucceededCode;
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Status = ConstHelper.FailedCode;
            }

            return Ok(response);
        }
    }
}
