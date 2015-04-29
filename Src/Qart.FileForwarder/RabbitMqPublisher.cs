using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qart.FileForwarder
{
    public class RabbitMqPublisher : IDisposable
    {
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        private string _exchange;
        private string _routingKey;

        public RabbitMqPublisher(string host, string queueName, string exchange, string routingkey)
        {
            _factory = new ConnectionFactory() { HostName = host };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queueName, false, false, false, null);
            _exchange = exchange;
            _routingKey = routingkey;
        }

        public void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(_exchange, _routingKey, null, body);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
