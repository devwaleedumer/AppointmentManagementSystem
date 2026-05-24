namespace AppointmentManagementSystem.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string? EndDate { get; set; }
        public int Duration { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public int IsDoctorApproved { get; set; }
        public string? AdminId { get; set; }


        public string? AdminName { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }

        public bool IsForClient { get; set; }

        public string AppointmentStatus
        {
            get
            {
                if (IsDoctorApproved == 0)
                {
                    return "Pending";
                }
                else if (IsDoctorApproved == 1)
                {
                    return "Approved";
                }
                else if (IsDoctorApproved == -1)
                {
                    return "Cancelled";
                }
               
                else
                {
                    return "Unknown";
                }
            }
        }
    }
}
