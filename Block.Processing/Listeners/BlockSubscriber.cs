using Block.Processing.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Block.Processing.Listeners
{
    public class BlockSubscriber : IBlockNotificationSubscriber<BlockInfo>
    {
        private ILogger<BlockSubscriber> _logger;

        public event Func<BlockInfo, CancellationToken, Task<bool>> OnBlock;
        public BlockSubscriber(ILogger<BlockSubscriber> logger)
        {
            _logger = logger;
        }
        public async Task<bool> HandleNotification(BlockInfo block, CancellationToken cancellationToken)
        {
            if (OnBlock == null)
            {
                var message = "No delegate for BlockSubscriber handler";
                _logger.LogCritical(message);
                throw new Exception(message);
            }

            return await OnBlock.Invoke(block, cancellationToken);
        }

        public Task<bool> Pause()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Start()
        {
            throw new NotImplementedException();
        }
    }
}
