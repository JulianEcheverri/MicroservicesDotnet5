using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _rmqConnection;
        private IModel _rmqChannel;
        private string _rmqQueueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var rmqConnectionFactory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            _rmqConnection = rmqConnectionFactory.CreateConnection();
            _rmqChannel = _rmqConnection.CreateModel();
            _rmqChannel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _rmqQueueName = _rmqChannel.QueueDeclare().QueueName;
            _rmqChannel.QueueBind(queue: _rmqQueueName, exchange: "trigger", routingKey: "");

            Console.WriteLine("--> Listening on the Message Bus...");

            // Subscribed for executing event handler function in case that the connection has been shut down
            _rmqConnection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public override void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");

            if (_rmqChannel.IsOpen)
            {
                _rmqChannel.Close();
                _rmqConnection.Close();
            }

            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_rmqChannel);
            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event Received!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _rmqChannel.BasicConsume(queue: _rmqQueueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}