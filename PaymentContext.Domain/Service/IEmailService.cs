namespace PaymentContext.Domain.Service
{
    public interface IEmailService
    {
        void Send(string to, string email, string subject, string body);
    }
}