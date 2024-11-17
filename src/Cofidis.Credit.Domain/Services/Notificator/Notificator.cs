namespace Cofidis.Credit.Domain.Services.Notificator
{
    public class Notificator : INotificator
    {
        private readonly List<Notification> _errorsNotification;

        public Notificator()
        {
            _errorsNotification = [];
        }

        public IEnumerable<Notification> GetErrorNotifications()
        {
            return _errorsNotification;
        }

        public void HandleError(Notification notification)
        {
            _errorsNotification.Add(notification);
        }
    }
}
