using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyRabbitMq

{
    /// <summary>
    /// 简单分发模式
    /// 
    /// </summary>
    public class SimpleQueue : BaseMQ
    {
        public const string TAG = "SimpleQueue";
        public const string Queue_Name = "SimpleQueue";
        public ConnectionFactory factory = null;


        public SimpleQueue(ConnectionFactory _factory)
        {
            factory = _factory;
            NetLog.WriteTextLog("消息接收", "SimpleQueue");
        }


        public override bool Send(string message)
        {
            try
            {
                if (message != null && message != "")
                {
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        //声明队列
                        channel.QueueDeclare(queue: Queue_Name,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                        //确认手动应答之前，只分发给同一个消费者一个消息
                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.ConfirmSelect();
                        //发布消息
                        channel.BasicPublish(exchange: "",
                               routingKey: Queue_Name,
                               basicProperties: properties,
                               body: Encoding.UTF8.GetBytes(message));
                        Console.WriteLine("Sent {0}", message);
                        if (!channel.WaitForConfirms())
                        {
                            Console.WriteLine("发送失败");
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override bool Send(string[] messageList)
        {
            throw new NotImplementedException();
        }

        public override void Received()
        {
            try
            {
                using (var connection = factory.CreateConnection())
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
                        RabbitMqUtil.HandleReceived(message);
                    };

                    //消费者应答
                    channel.BasicConsume(queue: Queue_Name,
                                 autoAck: true,
                                 consumer: consumer);

                    while (true)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void Received(string routingKey)
        {
            throw new NotImplementedException();
        }
    }
}
