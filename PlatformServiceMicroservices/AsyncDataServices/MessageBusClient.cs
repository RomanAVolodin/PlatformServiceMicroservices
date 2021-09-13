using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = Convert.ToInt16(_configuration["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to the message bus");
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformDto)
        {
            var message = JsonSerializer.Serialize(platformDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection Opened, sending message ...");

                SendMessage(message);
            } else
            {
                Console.WriteLine("--> RabbitMQ connection is Closed, sending failed ...");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbiMQ connection shutdown");
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
            Console.WriteLine($"--> We have just sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message bus disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _channel.Dispose();
            }
        }
    }
}
