using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyRabbitMq
{
    /// <summary>
    /// 工作队列，公平分发
    /// 必须手动应答模式
    /// 必须持久化
    /// </summary>
    class WorkQuere : BaseMQ{
       
        public const string Queue_Name = "WorkQuere";

        public ConnectionFactory factory = null;


        public WorkQuere(ConnectionFactory _factory)
        {
            factory = _factory;
        }


        public override void Send(string message)
        {
            if (message != null && message != "")
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //声明队列
                        channel.QueueDeclare(queue: Queue_Name,
                                     durable: true,
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
                        if (channel.WaitForConfirms())
                        {
                            Console.WriteLine("发送成功");
                        }
                        else
                        {
                            Console.WriteLine("发送失败");
                        }
                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                    }
                }
            }
           
        }

       
        public override void Send(string[] messageList)
        {
            if (messageList != null && messageList.Length >0)
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //声明队列
                        channel.QueueDeclare(queue: Queue_Name,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                        //确认手动应答之前，只分发给同一个消费者一个消息
                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.ConfirmSelect();
                        for (int i = 0; i < messageList.Length; i++)
                        {
                            string message = messageList[i];
                            //发布消息
                            channel.BasicPublish(exchange: "",
                                   routingKey: Queue_Name,
                                   basicProperties: properties,
                                   body: Encoding.UTF8.GetBytes(message));
                            Console.WriteLine("Sent {0}", message);
                        }
                       
                        if (channel.WaitForConfirms())
                        {
                            Console.WriteLine("发送成功");
                        }
                        else
                        {
                            Console.WriteLine("发送失败");
                        }

                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                    }
                }
            }
            
        }

        public override void Received()
        {
            //throw new NotImplementedException();
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //声明队列
                    channel.QueueDeclare(queue: Queue_Name,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    
                    //定义消费者
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        Thread.Sleep(1000);
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" Received {0}", message);

                       
                        //手动回执
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    //消费者应答
                    channel.BasicConsume(queue: Queue_Name,
                                 autoAck: false,
                                 consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
