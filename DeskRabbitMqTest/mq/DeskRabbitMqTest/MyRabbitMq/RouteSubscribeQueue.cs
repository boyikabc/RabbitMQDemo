using MyRabbitMq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace MyRabbitMq
{
    public class RouteSubscribeQueue : BaseMQ
    {
        public const string TAG = "RouteSubscribeQueue";
        public const string Exchange_Name = "logs_direct";
        public const string Queue_Name = "logs_direct_queue_RouteSubscribe";
        public const string Queue_Name2 = "logs_direct_queue_RouteSubscribe2";
        public const string RoutingKey = "logs.info";
        //public const string RoutingKey2 = "logs.info2";

        
        public ConnectionFactory factory = null;
        public RouteSubscribeQueue(ConnectionFactory _factory)
        {
            factory = _factory;
            NetLog.WriteTextLog("消息接收", "RouteSubscribeQueue");
        }
        public override bool Send(string message)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Direct,
                        durable: true, autoDelete: false, arguments: null);
                    //type: ExchangeType.Topic

                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ConfirmSelect();
                    //发布消息
                    channel.BasicPublish(exchange: Exchange_Name,
                                     routingKey: RoutingKey,
                                     basicProperties: properties,
                                     body: Encoding.UTF8.GetBytes(message));
                    Console.WriteLine(" [x] Sent {0}", message);

                    if (!channel.WaitForConfirms())
                    {
                        Console.WriteLine(" 发送失败！ ");
                        return false;
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
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Direct,
                        durable: true, autoDelete: false, arguments: null);
                    //type: ExchangeType.Topic

                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ConfirmSelect();

                    for (int i = 0; i < messageList.Length; i++)
                    {
                        string message = messageList[i];
                        //发布消息
                        channel.BasicPublish(exchange: Exchange_Name,
                                     routingKey: RoutingKey,
                                     basicProperties: properties,
                                     body: Encoding.UTF8.GetBytes(message));
                        Console.WriteLine("Sent {0}", message);
                    }

                    if (!channel.WaitForConfirms())
                    {
                        Console.WriteLine(" 发送失败！ ");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public override void Received()
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Direct,
                           durable: true, autoDelete: false, arguments: null);

                    QueueDeclareOk queueOk = channel.QueueDeclare(queue: Queue_Name,
                                   durable: true,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);


                    channel.QueueBind(
                        queue: queueOk.QueueName,
                        exchange: Exchange_Name,
                        routingKey: RoutingKey
                    );
                    //routingKey: "logs.#"

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [ Received ] {0},queueName:{1}  ,routKey:{2}", message, queueOk.QueueName, RoutingKey);
                        RabbitMqUtil.HandleReceived(message);
                        //手动回执
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    channel.BasicConsume(queue: queueOk.QueueName,
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

        public override void Received(string routingKey)
        {
            throw new NotImplementedException();
        }







        public bool Send(string[] messageList, string _routingKey)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Direct,
                        durable: true, autoDelete: false, arguments: null);
                    //type: ExchangeType.Topic

                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ConfirmSelect();

                    for (int i = 0; i < messageList.Length; i++)
                    {
                        string message = messageList[i];
                        //发布消息
                        channel.BasicPublish(exchange: Exchange_Name,
                                     routingKey: _routingKey,
                                     basicProperties: properties,
                                     body: Encoding.UTF8.GetBytes(message));
                        Console.WriteLine("Sent {0}", message);
                    }

                    if (!channel.WaitForConfirms())
                    {
                        Console.WriteLine(" 发送失败！ ");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public void Received2(string _queueName, string _routingKey)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Direct,
                           durable: true, autoDelete: false, arguments: null);

                    QueueDeclareOk queueOk = channel.QueueDeclare(queue: _queueName,
                                   durable: true,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);


                    channel.QueueBind(
                        queue: queueOk.QueueName,
                        exchange: Exchange_Name,
                        routingKey: _routingKey
                    );
                    //routingKey: "logs.#"

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [ Received ] {0},queueName:{1}  ,routKey:{2}", message, queueOk.QueueName, _routingKey);
                        string s = string.Format("{0} \t :{1} \t:{2}", queueOk.QueueName, _routingKey, message).ToString();
                        RabbitMqUtil.HandleReceived(s);
                        //手动回执
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    channel.BasicConsume(queue: queueOk.QueueName,
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

    }
}
