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
        public const string TAG = "WorkQuere";
        public const string Queue_Name = "WorkQuere";

        public ConnectionFactory factory = null;


        public WorkQuere(ConnectionFactory _factory)
        {
            factory = _factory;
            NetLog.WriteTextLog("消息接收", "WorkQuere");
        }

        /// <summary>
        /// 发送单条消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool Send(string message)
        {
            try
            {
                using (var connection = factory.CreateConnection())
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
                    if (!channel.WaitForConfirms())
                    {
                        Console.WriteLine("发送失败");
                        return false;
                    }

                    //Console.WriteLine(" Press [enter] to exit.");
                    //Console.ReadLine();
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送多条消息
        /// </summary>
        /// <param name="messageList"></param>
        /// <returns></returns>
        public override bool Send(string[] messageList)
        {
            try
            {
                using (var connection = factory.CreateConnection())
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
                    if (!channel.WaitForConfirms())
                    {
                        Console.WriteLine("发送失败");
                        return false;
                    }

                    //Console.WriteLine(" Press [enter] to exit.");
                    //Console.ReadLine();
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        public override void Received() 
        {
            //throw new NotImplementedException();
            try
            {
                using (var connection = factory.CreateConnection())
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


        /// <summary>
        /// 不实现
        /// </summary>
        /// <param name="routingKey"></param>
        public override void Received(string routingKey)
        {
            throw new NotImplementedException();
        }
    }
}
