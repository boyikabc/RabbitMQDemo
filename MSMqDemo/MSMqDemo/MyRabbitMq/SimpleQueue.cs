using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRabbitMq

{
    /// <summary>
    /// 简单分发模式
    /// 
    /// </summary>
    public class SimpleQueue : BaseMQ
    {

        public const string Queue_Name = "SimpleQueue";
        public ConnectionFactory factory = null;


        public SimpleQueue(ConnectionFactory _factory)
        {
            factory = _factory;
        }


        public override void Send(string message)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: Queue_Name,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                   
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "",
                                routingKey: Queue_Name,
                                basicProperties: null,
                                body: body);
                    Console.WriteLine(" [x] Sent {0}", message);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        public override void Send(string[] messageList)
        {
            throw new NotImplementedException();
        }

        public override void Received()
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: Queue_Name,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);

                    };

                    //消费者应答
                    channel.BasicConsume(queue: Queue_Name,
                                 autoAck: true,
                                 consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
