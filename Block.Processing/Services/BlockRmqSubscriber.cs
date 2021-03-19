using System;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using Swf.RabbitMQ.Abstractions;
using Microsoft.Extensions.Logging;
using Swf.RabbitMQ.Abstractions.Models;
using Block.Processing.DTO;
using Block.Processing.Listeners;
using Block.Processing.Models;

namespace Block.Processing.Services
{
    public class BlockRmqSubscriber : BaseRabbitMqListener
    {
        private readonly ILogger _logger;
        private readonly IRmqConnection _rabbitMqConnection;
        private readonly IBlockNotificationSubscriber<BlockInfo> _subscriber;

        private const string _exchange = "e.ac.forward";
        private const string _routingKey = "r.ac.mining.block-new.#";
        private const string _queueName = "q.ac.mining.block-new.83534d10-c917-4f9e-98be-a0e694ae7ce2";

        public BlockRmqSubscriber(IRmqConnection rabbitMqConnection,
                                   ILogger<BlockRmqSubscriber> logger,
                                   IBlockNotificationSubscriber<BlockInfo> subscriber, BlockModel model)
        {
            _logger = logger;
            _rabbitMqConnection = rabbitMqConnection;
            _subscriber = subscriber;
        }

        public override IRmqConsumer CreateConsumer()
        {
            return _rabbitMqConnection.CreateConsumer(new ConsumerProperties()
            {
                Exchange = new ExchangeProperties()
                {
                    ExchangeName = _exchange,
                    ExchangeType = ExchangeType.topic,
                    Durable = true
                },
                RoutingKeys = new[]
                {
                    _routingKey
                },
                Queue = new QueueProperties()
                {
                    QueueName = _queueName
                }
            });
        }

        protected async override Task HandleMessageAsync(BlockInfo message, BasicDeliverEventArgs eventArgs, IRmqConsumer consumer)
        {
            try
            {
                await _subscriber.HandleNotification(message, default);
            }
            catch (Exception e)
            {
                _logger.LogError("BbpsServiceSubscriber Callback error", e);
            }
        }
    }
}
