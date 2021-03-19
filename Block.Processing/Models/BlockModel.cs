using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Block.Processing.Dao;
using Block.Processing.DTO;
using Block.Processing.Listeners;
using Block.Processing.Providers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Block.Processing.Models
{
    public class BlockModel
    {
        private readonly ILogger _logger;
        private readonly BlockProviderPg _blockProviderPg;
        private IBlockNotificationSubscriber<BlockInfo> _blockSubscriber;
        private int _lastBlockId = 1;
        private readonly IDistributedCache _cache;

        public BlockModel(ILogger<BlockModel> logger,
                          BlockProviderPg blockProviderPg,
                          IBlockNotificationSubscriber<BlockInfo> blockSubscriber,
                          IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
            _blockProviderPg = blockProviderPg;
            _blockSubscriber = blockSubscriber;
            _blockSubscriber.OnBlock += HandleBlock;
        }

        private Task<bool> HandleBlock(BlockInfo block, CancellationToken arg2)
        {
            try
            {
                _blockProviderPg.InsertBlock(new BlockInfoDao { Id = _lastBlockId++, BlockNumber = block.BlockNumber, Date = block.Date });
                _logger.LogInformation("Block was inserted to db");

                IncrementInCache();
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "HandleBlock error");
                throw;
            }
        }

        private void IncrementInCache()
        {
            var jsonUtf8Bytes = JsonSerializer.Serialize(_lastBlockId);
            _cache.Set("block01", Encoding.UTF8.GetBytes(jsonUtf8Bytes));

            _logger.LogInformation("Redis cache is updated");
        }
    }
}