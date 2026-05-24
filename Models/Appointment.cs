using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManagementSystem.Models
{
    /// <summary>
    /// Scheduled visit linking one patient and one doctor.
    /// </summary>
    public class Appointment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }

        /// <summary>Computed from StartDate + Duration when saved.</summary>
        public DateTime EndDate { get; set; }

        /// <summary>Length of the appointment in minutes.</summary>
        public int Duration { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }

        /// <summary>
        /// Workflow state: 0 = pending, 1 = approved, -1 = cancelled.
        /// Values are defined on <see cref="Helpers.ConstHelper"/>.
        /// </summary>
        public int IsDoctorApproved { get; set; }

        /// <summary>Optional admin who created the booking on behalf of a patient.</summary>
        public string? AdminId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public ApplicationUser Patient { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public ApplicationUser Doctor { get; set; }
    }
}
