using AppointmentManagementSystem.ViewModels.Settings;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppointmentManagementSystem.Services.Email
{
    /// <summary>
    /// Sends HTML email through Mailjet. Invoked directly or via Hangfire background jobs.
    /// </summary>
    public class MailjetEmailService : IEmailService
    {
        private readonly MailjetSettings _settings;

        public MailjetEmailService(IOptions<MailjetSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                var client = new MailjetClient(_settings.ApiKey, _settings.SecretKey);

                var request = new MailjetRequest
                {
                    Resource = Send.Resource
                }
                .Property(Send.FromEmail, _settings.FromEmail)
                .Property(Send.FromName, _settings.FromName)
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, htmlBody)
                .Property(Send.Recipients, new JArray
                {
            new JObject
            {
                { "Email", toEmail },
                { "Name", toName }
            }
                });

                var response = await client.PostAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
