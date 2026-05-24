namespace AppointmentManagementSystem.Services.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
    }
}
    