namespace AppointmentManagementSystem.ViewModels.Settings
{
    public class MailjetSettings
    {
        public string ApiKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
    }
}
