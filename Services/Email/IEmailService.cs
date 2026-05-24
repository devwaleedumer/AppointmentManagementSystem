namespace AppointmentManagementSystem.Services.Email
{
    /// <summary>Abstraction for outbound transactional email (welcome, lockout, appointment updates).</summary>
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
    }
}
    