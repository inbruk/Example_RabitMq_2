using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static Func<Task> CreateTask(int time2sleep, String routingKey)
        {
            return () =>
            {
                var counter = 0;
                do
                {
                    var timeToSleep = new Random().Next(1000, time2sleep);
                    Thread.Sleep(timeToSleep);

                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);

                        var message = $"{routingKey} message from producer N:{counter++}";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "direct_logs", routingKey: routingKey, basicProperties: null, body: body);

                        Console.WriteLine(message);
                    }
                }
                while (true);
            };
        }

        static void Main(String[] args)
        {
            Task.Run(CreateTask(2000, "info"));
            Task.Run(CreateTask(3000, "warning"));
            Task.Run(CreateTask(4000, "error"));


            Console.ReadKey();
        }
    }
}