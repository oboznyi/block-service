using Block.Sender.Configuration;
using Block.Sender.DTO;
using Block.Sender.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Block.Sender.Services
{
    public class BlockService: IHostedService
    {
        private readonly ILogger _logger;
        private readonly IBlockModel _model;
        private readonly int _delay;

        public BlockService(ILogger<BlockService> logger, IBlockModel model,IOptions<SenderConfiguration> config)
        {
            _logger = logger;
            _model = model;
            _delay = config.Value.BlockDelayInSeconds;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await _model.PublishBlockAsync(
                                    new BlockInfo
                                    {
                                        BlockNumber = 228,
                                        Date = DateTime.UtcNow
                                    });

                    await Task.Delay(_delay * 1000, cancellationToken);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error publishing block");
                throw;
            };
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
