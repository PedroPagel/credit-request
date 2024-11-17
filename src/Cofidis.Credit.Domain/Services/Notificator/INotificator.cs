namespace Cofidis.Credit.Domain.Services.Notificator
{
    public interface INotificator
    {
        IEnumerable<Notification> GetErrorNotifications();
        void HandleError(Notification notification);
    }
}
