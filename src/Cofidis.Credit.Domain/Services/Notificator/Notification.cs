namespace Cofidis.Credit.Domain.Services.Notificator
{
    public class Notification(string message)
    {
        public DateTime TimeStamp { get; } = DateTime.Now;
        public string Message { get; } = message;
    }
}
