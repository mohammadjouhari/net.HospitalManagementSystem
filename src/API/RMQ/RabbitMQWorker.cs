using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace API.RMQ
{
    public class RabbitMQWorker: BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQWorker()
        {
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Ensure the queue exists
            _channel.QueueDeclare(queue: "notification", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // LOGIC: This is where the background work happens
                ProcessOrder(message);

                // Acknowledge the message was processed
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "notification", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private void ProcessOrder(string message)
        {
            Log.Logger.Information("Test");
            // Here you would call your Email Service or Database
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

    }
}
