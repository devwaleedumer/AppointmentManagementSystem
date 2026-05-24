using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Helpers;
using AppointmentManagementSystem.Models;
using AppointmentManagementSystem.Services.Email;
using AppointmentManagementSystem.ViewModels;
using AppointmentManagementSystem.ViewModels.Appointments;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace AppointmentManagementSystem.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppointmentManagementSystemDbContext _db;
        private readonly IEmailService _emailService;
        public AppointmentService(AppointmentManagementSystemDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }


        public async Task<int> GetTotalAppointmentsByPatientId(string patientId) => await _db.Appointments.CountAsync(app => app.PatientId == patientId);
        public async Task<int> GetTotalAppointmentsByDoctorId(string doctorId) => await _db.Appointments.CountAsync(app => app.DoctorId == doctorId);
        public async Task<int> GetUpcomingAppointmentsByPatientId(string patientId) => await _db.Appointments.CountAsync(app => app.PatientId == patientId && app.StartDate > DateTime.Now);
        public async Task<int> GetUpcomingAppointmentsByDoctorId(string doctorId) => await _db.Appointments.CountAsync(app => app.DoctorId == doctorId && app.StartDate > DateTime.Now);
        public async Task<int> GetApprovedAppointmentsByPatientId(string patientId) => await _db.Appointments.CountAsync(app => app.PatientId == patientId && app.IsDoctorApproved == 1);
        public async Task<int> GetApprovedAppointmentsByDoctorId(string doctorId) => await _db.Appointments.CountAsync(app => app.DoctorId == doctorId && app.IsDoctorApproved == 1);
        public async Task<int> GetCancelledAppointmentsByDoctorId(string doctorId) => await _db.Appointments.CountAsync(app => app.DoctorId == doctorId && app.IsDoctorApproved == -1);
        
        public async Task<IEnumerable<AppointmentViewModel>> GetRecentAppointmentsByDoctorId(string doctorId)
        {
            var recentAppointments = await _db.Appointments
                .Where(app => app.DoctorId == doctorId)
                .OrderByDescending(app => app.StartDate)
                .Select(app => new AppointmentViewModel
                {
                    Id = app.Id,
                    Title = app.Title,
                    Description = app.Description,
                    StartDate = app.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndDate = app.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Duration = app.Duration,
                    DoctorId = app.DoctorId,
                    PatientId = app.PatientId
                })
                .Take(5)
                .ToListAsync();

            return recentAppointments;

        }

        public async Task<int> GetTotalNoOfDoctors()
        {
            var result = await _db.Users.Join(_db.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_db.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur.u, r })
                .Where(x => x.r.Name == ConstHelper.DoctorRole)
                .CountAsync();

            return result;
        }

        public async Task<int> GetTodaysAppointments()
        {
            var result = await _db.Appointments.CountAsync(app => app.StartDate.Date == DateTime.Now.Date);
            return result;
        }

        public async Task<bool> CancelAppointment(int appointmentId)
        {
            var appointment = await _db.Appointments.FindAsync(appointmentId);
            if (appointment is null)
            {
                return false;
            }

            appointment.IsDoctorApproved = -1; // Assuming -1 indicates cancellation 
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AcceptAppointment(int appointmentId)
        {
            var appointment = await _db.Appointments.FindAsync(appointmentId);
            if (appointment is null)
            {
                return false;
            }

            appointment.IsDoctorApproved = 1; // Assuming 1 indicates acceptance
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddOrUpdatePatient(AppointmentViewModel model)
        {
            var startDate = DateTime.Parse(model.StartDate);
            var endDate = startDate.AddMinutes(model.Duration);

            if (model.Id > 0 && model is not null)
            {

                var existingAppointment = await _db.Appointments.FindAsync(model.Id);
                existingAppointment.Description = model.Description;
                existingAppointment.Title = model.Title;
                existingAppointment.StartDate = startDate;
                existingAppointment.EndDate = endDate;
                existingAppointment.Duration = model.Duration;
                existingAppointment.DoctorId = model.DoctorId;
                existingAppointment.PatientId = model.PatientId;

                _db.Appointments.Update(existingAppointment);
                await _db.SaveChangesAsync();
                return 1;
            }
            else
            {
               
                Appointment app = new Appointment
                {
                    Description = model.Description,
                    Title = model.Title,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = model.Duration,
                    DoctorId = model.DoctorId,
                    PatientId = model.PatientId,
                    IsDoctorApproved = 0,
                    AdminId = model.AdminId
                };

                await _db.Appointments.AddAsync(app);
                await _db.SaveChangesAsync();

                var reminderTime = app.StartDate.AddMinutes(-1);
                var user = await _db.Users.FindAsync(app.PatientId);
                if (reminderTime > DateTime.Now)
                {
                    BackgroundJob.Schedule( () => _emailService.SendEmailAsync(
                        user.Email,
                        user.Name,
                        "Appointment Reminder",
                        $"This is a reminder for your upcoming appointment titled '{app.Title}' scheduled at {app.StartDate:yyyy-MM-dd HH:mm:ss}."
                    ),
                        reminderTime
                    );
                }
                return 2;
            }
        }

        public async Task<IEnumerable<AppointmentViewModel>> DoctorEventsById(string doctorId)
        {
            var result = await _db.Appointments.Where(app => app.DoctorId == doctorId)
                                   .Select(x => new AppointmentViewModel
                                   {
                                       Id = x.Id,
                                       Description = x.Description,
                                       Title = x.Title,
                                       StartDate = x.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                       EndDate = x.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                       Duration = x.Duration,
                                       IsDoctorApproved = x.IsDoctorApproved,
                                   }).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<DoctorViewModel>> GetAllDoctors()
        {
            var result = await (from users in _db.Users
                                join userRoles in _db.UserRoles
                                on users.Id equals userRoles.UserId
                                join roles in _db.Roles
                                on userRoles.RoleId equals roles.Id
                                where roles.Name == ConstHelper.DoctorRole
                                select new DoctorViewModel
                                {
                                    Id = users.Id,
                                    Name = users.Name,
                                    Qualification = users.Qualification,
                                    Specialization = users.Specialization,
                                }).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<PatientViewModel>> GetAllPatients()
        {
            var result = await (from users in _db.Users
                                join userRoles in _db.UserRoles
                                on users.Id equals userRoles.UserId
                                join roles in _db.Roles
                                on userRoles.RoleId equals roles.Id
                                where roles.Name == ConstHelper.PatientRole
                                select new PatientViewModel
                                {
                                    Id = users.Id,
                                    Name = users.Name
                                }).ToListAsync();

            return result;
        }

        public async Task<AppointmentViewModel> GetById(int id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            if (appointment == null)
                return null;

            return new AppointmentViewModel
            {
                Id = appointment.Id,
                Description = appointment.Description,
                Title = appointment.Title,
                StartDate = appointment.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = appointment.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Duration = appointment.Duration,
                IsDoctorApproved = appointment.IsDoctorApproved,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                PatientName = (await _db.Users.FindAsync(appointment.PatientId))?.Name,
                DoctorName = (await _db.Users.FindAsync(appointment.DoctorId))?.Name,
            };
        }

        public async Task<IEnumerable<AppointmentViewModel>> PatientsEventById(string patientId)
        {
            var result = await _db.Appointments.Where(app => app.PatientId == patientId)
                                  .Include(x => x.Patient)
                                    .Include(x => x.Doctor)
                                    .AsNoTracking()
                                 .Select(x => new AppointmentViewModel
                                 {
                                     Id = x.Id,
                                     Description = x.Description,
                                     Title = x.Title,
                                     StartDate = x.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                     EndDate = x.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                     Duration = x.Duration,
                                     IsDoctorApproved = x.IsDoctorApproved,
                                     PatientId = x.PatientId,
                                     DoctorId = x.DoctorId,
                                     DoctorName = x.Doctor.Name,
                                     PatientName = x.Patient.Name,
                                 }).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<AppointmentViewModel>> GetRecentAppointments()
        {
            var result = await _db.Appointments
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .AsNoTracking()
                                               .Take(10)
                                               .Select(app => new AppointmentViewModel
                                               {
                                                   Title = app.Title,
                                                   Description = app.Description,
                                                   StartDate = app.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                   EndDate = app.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                   Duration = app.Duration,
                                                   PatientId = app.PatientId,
                                                   DoctorId = app.DoctorId,
                                                   DoctorName = app.Doctor.Name,
                                                   PatientName = app.Patient.Name,
                                               }).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public async Task<int> DeleteAppointment(int appointmentId)
        {
            var appointment = await _db.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return 0;

            _db.Appointments.Remove(appointment);
            return await _db.SaveChangesAsync();
        }
    }
}
