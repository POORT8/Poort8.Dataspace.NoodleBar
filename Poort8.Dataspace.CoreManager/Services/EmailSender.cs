using Microsoft.AspNetCore.Identity;
using Poort8.Dataspace.Identity;
using System.Text.Json.Serialization;

namespace Poort8.Dataspace.CoreManager.Services;

public class EmailSender : IEmailSender<User>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IHttpClientFactory httpClientFactory, ILogger<EmailSender> logger, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.DefaultRequestHeaders.Add("api-key", configuration["BrevoApiKey"]);
        _logger = logger;
    }

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        _logger.LogInformation("P8.inf - Sending confirmation link to {email}", email);
        await SendEmailAsync(email, email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        _logger.LogInformation("P8.inf - Sending reset code to {email}", email);
        await SendEmailAsync(email, email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        _logger.LogInformation("P8.inf - Sending reset link to {email}", email);
        await SendEmailAsync(email, email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
    }

    private async Task SendEmailAsync(string emailTo, string nameTo, string subject, string body)
    {
        var email = new Email
        {
            Sender = new Sender
            {
                Name = "Poort8",
                Email = "hello@poort8.nl"
            },
            To = [new To { Email = emailTo, Name = nameTo }],
            Subject = subject,
            HtmlContent = body
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("https://api.brevo.com/v3/smtp/email", email);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            _logger.LogCritical("P8.crit - Could not send email to {email}: {msg}", emailTo, e.Message);
        }
    }

    private class Email
    {
        [JsonPropertyName("sender")]
        public required Sender Sender { get; set; }

        [JsonPropertyName("to")]
        public required List<To> To { get; set; }

        [JsonPropertyName("subject")]
        public required string Subject { get; set; }

        [JsonPropertyName("htmlContent")]
        public required string HtmlContent { get; set; }
    }

    private class Sender
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("email")]
        public required string Email { get; set; }
    }

    private class To
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
