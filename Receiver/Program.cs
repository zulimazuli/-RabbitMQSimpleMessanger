using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Receiver started.");
            Receive();
        }

        private static void Receive()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            //might be redundant
            channel.QueueDeclare("hello", false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var msg = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine(" [x] Received: {0}", msg);
            };

            channel.BasicConsume(queue: "hello", true, consumer);
            
            Console.ReadLine(); //wait for messages
        }
    }
}
