using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Block.Processing.Configuration;
using Block.Processing.DTO;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Swf.RabbitMQ.Abstractions;
using Swf.RabbitMQ.Abstractions.Models;

namespace Block.Processing.Services
{
    public abstract class BaseRabbitMqListener: IHostedService
    {
        private IRmqConsumer _consumer;

        public BaseRabbitMqListener()
        {
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer = CreateConsumer();

            _consumer.OnMessageAsync += HandleMessageAsync;

            _consumer.Subscribe();

            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            if (_consumer.SubscribeIsActive)
                _consumer.Unsubscribe();

            _consumer.OnMessageAsync -= HandleMessageAsync;

            return Task.CompletedTask;
        }

        public abstract IRmqConsumer CreateConsumer();

        protected abstract Task HandleMessageAsync(BlockInfo message, BasicDeliverEventArgs eventArgs, IRmqConsumer consumer);

        private Task HandleMessageAsync(byte[] body, BasicDeliverEventArgs eventArgs, IRmqConsumer consumer)
        {
            var block = JsonConvert.DeserializeObject<BlockInfo>(Encoding.UTF8.GetString(body));

            return HandleMessageAsync(block, eventArgs, consumer);
        }

        protected ConsumerProperties CreateConsumerProperty(RabbitMqConsumerConfiguration configuration)
        {
            return new ConsumerProperties
            {
                AutoAck = configuration.AutoAck,
                Exchange = configuration.ExchangeProperties,
                RoutingKeys = configuration.RoutingKey,
                Queue = configuration.QueueProperties
            };
        }
    }
}