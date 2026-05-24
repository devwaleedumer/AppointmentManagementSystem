using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManagementSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public int IsDoctorApproved { get; set; }
        public string? AdminId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public ApplicationUser Patient { get; set; }
        [ForeignKey(nameof(DoctorId))]
        public ApplicationUser Doctor { get; set; }
    }
}
