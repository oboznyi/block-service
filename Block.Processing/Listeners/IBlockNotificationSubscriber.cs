using System;
using System.Threading;
using System.Threading.Tasks;

namespace Block.Processing.Listeners
{
    public interface IBlockNotificationSubscriber<TClass>: INotificationSubscriber<TClass, bool>
    {
        event Func<TClass, CancellationToken, Task<bool>> OnBlock;
    }
}
