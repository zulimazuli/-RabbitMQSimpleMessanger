using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Send(string.Join(" ", args));
            }
            else
            {
                Console.WriteLine($"Enter messages to send task.");
                while (true)
                {
                    var msg = Console.ReadLine();
                    Send(msg);
                }
            }
        }

        private static void Send(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "task_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish("", "task_queue", properties, body);
            Console.WriteLine($" [x] Sent: {message}");
        }
    }
}
