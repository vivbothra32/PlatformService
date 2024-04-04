using System.Text;
using System.Text.Json;
using PlatformService.DTOs;
using RabbitMQ.Client;

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
            var factory = new ConnectionFactory() {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            try
            {
                //Creating a connection using the ConnectionFactory object
                _connection = factory.CreateConnection();

                //Creating channel once connection is built
                _channel = _connection.CreateModel();

                //Declaring exchange on the channel : ExchangeType - FanOut
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                //Subscribing to the connection shutdown event
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("--> Connected to Message Bus.");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }
        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            //Json serialized string
            var message = JsonSerializer.Serialize(platformPublishedDto);
            if(_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection is open, sending message...");
                SendMessage(message);
            }
            else{
                Console.WriteLine("--> RabbitMQ connection is closed, cannot send message...");
            }

        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "trigger",
                routingKey: "",
                basicProperties: null,
                body: body
            );
            Console.WriteLine($"--> Message sent successfully: {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("--> Disposing Message Bus!");
            if(_channel.IsClosed)
            {
                _channel.Close();
                _connection.Close();
            }
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ shutting down.");
        }
    }
}