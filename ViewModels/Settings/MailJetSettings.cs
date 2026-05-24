namespace AppointmentManagementSystem.ViewModels.Settings
{
    /// <summary>Mailjet API credentials bound from the MailjetSettings configuration section.</summary>
    public class MailjetSettings
    {
        public string ApiKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
    }
}
