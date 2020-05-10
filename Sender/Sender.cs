using System;
using System.Text;
using RabbitMQ.Client;

namespace Sender
{
    class Sender
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Sender started. Enter messages to send.");
            while (true)
            {
                var msg = Console.ReadLine();
                Send(msg);
            }
        }

        private static void Send(string message)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("", "hello", null, body);
            Console.WriteLine($" [x] Sent: {message}");
        }
    }
}
