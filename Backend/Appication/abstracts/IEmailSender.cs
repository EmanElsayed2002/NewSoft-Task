namespace Application.abstracts
{
    public interface IEmailSender
    {
        public Task<string> SendEmail(string email, string Message, string? reason);

    }
}
