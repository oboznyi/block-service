using System.Threading.Tasks;
using Swf.RabbitMQ.Abstractions;
using Block.Sender.Configuration;
using Swf.RabbitMQ.Abstractions.Models;
using Block.Sender.DTO;
using System;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;

namespace Block.Sender.Services
{
    public class BlockPublisher
    {
        private IRmqPublisher _rabbitMqPublisher;
        private readonly ILogger<BlockPublisher> _logger;

        private const string _exchangeName = "e.ac.forward";
        private const string _routingKey = "r.ac.mining.block-new.83534d10-c917-4f9e-98be-a0e694ae7ce2";
        public BlockPublisher(ILogger<BlockPublisher> logger, IRmqConnection rmqConnection)
        {
            _logger = logger;

            CreatePublisher(rmqConnection);
        }

        private void CreatePublisher(IRmqConnection rmqConnection)
        {
            _rabbitMqPublisher = rmqConnection.CreatePublisher(new PublisherProperties()
            {
                Exchange = new ExchangeProperties()
                {
                    ExchangeName = _exchangeName,
                    ExchangeType = ExchangeType.topic,
                    Durable = true
                },
                RoutingKey = _routingKey
            });
        }

        public async Task PublishBlockAsync(BlockInfo block)
        {
            try
            {
                var jsonUtf8Bytes = JsonSerializer.Serialize(block);

                await PublishAsync(Encoding.UTF8.GetBytes(jsonUtf8Bytes));
            }catch(Exception e)
            {
                _logger.LogError(e, "Error while publiching block to rmq");
                throw;
            }
        }

        public async Task PublishAsync(byte[] message)
        {
            await _rabbitMqPublisher.PublishAsync(message,
                new PublisherProperties()
                {
                    Exchange = new ExchangeProperties()
                    {
                        ExchangeName = _exchangeName,
                        ExchangeType = ExchangeType.topic,
                        Durable = true
                    },
                    RoutingKey = _routingKey
                });
        }
    }
}
