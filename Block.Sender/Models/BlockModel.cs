using System.Threading;
using System.Threading.Tasks;
using Block.Sender.DTO;
using Block.Sender.Services;
using Microsoft.Extensions.Logging;

namespace Block.Sender.Models
{
    public class BlockModel: IBlockModel
    {
        private readonly ILogger _logger;
        private readonly BlockPublisher _publisher;

        public BlockModel(ILogger<BlockModel> logger, BlockPublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        public async Task PublishBlockAsync(BlockInfo block)
        {
            await _publisher.PublishBlockAsync(block);
            _logger.LogInformation("New Block was published");
        }
    }
}