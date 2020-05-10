using System;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            Receive();
        }

        private static void Receive()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("task_queue", true, false, false, null);
            channel.BasicQos(0, 1, false);
            
            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body;
                var msg = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine(" [x] Received: {0}", msg);

                if (!msg.Contains("."))
                {
                    ((EventingBasicConsumer)sender).Model.BasicReject(ea.DeliveryTag, false);
                    Console.WriteLine(" [x] Message rejected.");
                }
                else
                {
                    //Simulate execution time for each dot in a message
                    var dots = msg.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);
                    Console.WriteLine(" [x] Done");

                    //((EventingBasicConsumer)sender).Model == channel
                    ((EventingBasicConsumer) sender).Model.BasicAck(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsume(queue: "task_queue", false, consumer);

            Console.ReadLine(); //wait for messages
        }
    }
}
