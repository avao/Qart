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

        public RabbitMqPublisher(string host, int port, string user, string password, string exchange)
        {
            _factory = new ConnectionFactory() { HostName = host, Port = port, UserName = user, Password = password };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchange = exchange;
        }

        public void Publish(string message, string meta)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(_exchange, meta, null, body);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
