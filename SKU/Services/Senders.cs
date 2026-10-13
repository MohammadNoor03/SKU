namespace SKU.Services
{
    public interface ISmsSender
    {
        Task SendAsync(string phoneNumber, string message);
    }

    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string body);
    }

    // Development only: prints the message in the Output window instead of sending it.
    public class ConsoleSmsSender : ISmsSender
    {
        private readonly ILogger<ConsoleSmsSender> _logger;

        public ConsoleSmsSender(ILogger<ConsoleSmsSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("[SMS] To: {Phone} | {Message}", phoneNumber, message);
            return Task.CompletedTask;
        }
    }

    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string toEmail, string subject, string body)
        {
            _logger.LogInformation("[EMAIL] To: {Email} | {Subject} | {Body}", toEmail, subject, body);
            return Task.CompletedTask;
        }
    }
}