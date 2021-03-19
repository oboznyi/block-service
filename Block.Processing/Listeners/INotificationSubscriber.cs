using System.Threading;
using System.Threading.Tasks;

public interface INotificationSubscriber<in TNotification, TResponse>
{
    Task<TResponse> HandleNotification(TNotification notification, CancellationToken cancellationToken);
    Task<bool> Pause();
    Task<bool> Start();
}
