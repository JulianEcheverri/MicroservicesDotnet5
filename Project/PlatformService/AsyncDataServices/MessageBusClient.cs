using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _rmqConnection;
        private readonly IModel _rmqChannel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            var rmqConnectionFactory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _rmqConnection = rmqConnectionFactory.CreateConnection();
                _rmqChannel = _rmqConnection.CreateModel();
                _rmqChannel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                // Subscribed for executing event handler function in case that the connection has shut down
                _rmqConnection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);
            if (_rmqConnection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection is open... sending message");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed...");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _rmqChannel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

            Console.WriteLine($"--> We have sent: {message}");
        }

        public void Dipose()
        {
            Console.WriteLine("MessageBus Disposed");
            
            if (_rmqChannel.IsOpen)
            {
                _rmqChannel.Close();
                _rmqConnection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}