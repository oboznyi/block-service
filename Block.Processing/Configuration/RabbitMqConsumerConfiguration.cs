using Swf.RabbitMQ.Abstractions.Models;

namespace Block.Processing.Configuration
{
    public class RabbitMqConsumerConfiguration
    {
        public bool AutoAck { get; set; }
        public ExchangeProperties ExchangeProperties { get; set; }
        public string[] RoutingKey { get; set; }
        public QueueProperties QueueProperties { get; set; }
    }
}
