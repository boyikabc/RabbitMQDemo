using MyRabbitMq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqClient
{
    public class RabbitMQHelper
    {
        public const bool isTest = false;
        /// <summary>
        /// 创建ConnectionFactory
        /// </summary>
        /// <returns></returns>
        private ConnectionFactory factory { get; set; }

        public RabbitMQHelper()
        {
            if (factory == null)
            {
                factory = new ConnectionFactory();
                if (isTest == false)
                {
                    factory.HostName = "47.92.104.20";
                    factory.VirtualHost = "/vhost_school";
                    factory.Port = 5672;
                    factory.UserName = "school";
                    factory.Password = "81234567";
                }
                else
                {
                    //测试环境
                    factory.HostName = "127.0.0.1";
                    factory.VirtualHost = "/vhost_school";
                    factory.Port = 5672;
                    factory.UserName = "myrabbit";
                    factory.Password = "81234567";
                }
            }
        }

        #region 工作队列模式

        /// <summary>
        /// 工作队列 消息发送
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="MqName"></param>
        public bool Send(string message, string Queue_Name)
        {
            try
            {
                if (string.IsNullOrEmpty(message)) return false;
                if (string.IsNullOrEmpty(Queue_Name)) return false;
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
                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    channel.ConfirmSelect();  // 发送状态事务
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
        /// 工作队列 接受消息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="MqName"></param>
        /// <param name="entity"></param>
        /// <param name="action"></param>
        public void Received(string Queue_Name, Action<string> _action)
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

                    //定义消费者
                    var consumer = new EventingBasicConsumer(channel);
                    //消费者应答
                    channel.BasicConsume(queue: Queue_Name,
                                 autoAck: false,
                                 consumer: consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);  //手动回执
                        _action(message);
                        Thread.Sleep(1000);
                    };
                    while (true)
                    {
                        Thread.Sleep(1000 * 10);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        } 
        #endregion


        #region  消息订阅模式
        public bool SendByFanout(string message, string Exchange_Name)
        {
            try
            {
                if (string.IsNullOrEmpty(message)) return false;
                if (string.IsNullOrEmpty(Exchange_Name)) return false;
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Fanout,
                        durable: true, autoDelete: false, arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);
                    //确认手动应答之前，只分发给同一个消费者一个消息
                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ConfirmSelect();

                    //发布消息
                    channel.BasicPublish(exchange: Exchange_Name,
                                     routingKey: "",
                                     basicProperties: properties,
                                     body: body);
                    if (!channel.WaitForConfirms())
                    {
                        Console.WriteLine("发送失败");
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
        public void ReceivedByFanout(string Queue_Name, string Exchange_Name, Action<string> _action)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Fanout,
                           durable: true, autoDelete: false, arguments: null);
                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    QueueDeclareOk queueOk = channel.QueueDeclare(queue: Queue_Name,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                    channel.QueueBind(queue: queueOk.QueueName,
                                  exchange: Exchange_Name,
                                  routingKey: "");

                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: queueOk.QueueName,
                                     autoAck: false,
                                     consumer: consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);  //手动回执
                        _action(message);
                        Thread.Sleep(1000);
                    };

                    while (true)
                    {
                        Thread.Sleep(1000 * 10);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        #endregion


        #region 路由模式
        public bool SendByDirect(string message, string Exchange_Name, string RoutingKey)
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
                    var body = Encoding.UTF8.GetBytes(message);
                    //确认手动应答之前，只分发给同一个消费者一个消息
                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ConfirmSelect();

                    //发布消息
                    channel.BasicPublish(exchange: Exchange_Name,
                                     routingKey: RoutingKey,
                                     basicProperties: properties,
                                     body: body);
                    if (!channel.WaitForConfirms())
                    {
                        Console.WriteLine("发送失败");
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
        public void ReceivedByDirect(string Queue_Name, string Exchange_Name,string RoutingKey, Action<string> _action)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Direct,
                           durable: true, autoDelete: false, arguments: null);
                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    QueueDeclareOk queueOk = channel.QueueDeclare(queue: Queue_Name,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                    channel.QueueBind(queue: queueOk.QueueName,
                                  exchange: Exchange_Name,
                                  routingKey: RoutingKey);
                    //routingKey: "logs.#"
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: queueOk.QueueName,
                                     autoAck: false,
                                     consumer: consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);  //手动回执
                        _action(message);
                        Thread.Sleep(1000);
                    };

                    while (true)
                    {
                        Thread.Sleep(1000 * 10);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        } 
        
        #endregion


        #region 模糊路由模式
        public bool SendByTopic(string message, string Exchange_Name, string RoutingKey)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Topic,
                        durable: true, autoDelete: false, arguments: null);
                   
                    var body = Encoding.UTF8.GetBytes(message);
                    //确认手动应答之前，只分发给同一个消费者一个消息
                    //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ConfirmSelect();

                    //发布消息
                    channel.BasicPublish(exchange: Exchange_Name,
                                     routingKey: RoutingKey,
                                     basicProperties: properties,
                                     body: body);
                    if (!channel.WaitForConfirms())
                    {
                        Console.WriteLine("发送失败");
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
        public void ReceivedByTopic(string Queue_Name, string Exchange_Name, string RoutingKey, Action<string> _action)
        {
            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Topic,
                           durable: true, autoDelete: false, arguments: null);
                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    QueueDeclareOk queueOk = channel.QueueDeclare(queue: Queue_Name,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                    channel.QueueBind(queue: queueOk.QueueName,
                                  exchange: Exchange_Name,
                                  routingKey: RoutingKey);
                    //routingKey: "logs.#"  //routingKey: "logs.*" 
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queue: queueOk.QueueName,
                                     autoAck: false,
                                     consumer: consumer);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);  //手动回执
                        _action(message);
                        Thread.Sleep(1000);
                    };

                    while (true)
                    {
                        Thread.Sleep(1000 * 10);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region    从这里开始学习

        public void SendTest()
        {
            var thread = new Thread(e =>
            {
                string message = "this is a work queue 1 !";
                string queueName = "WorkQuere";
                string roukey = "";
                string exchange = "";
                int resultId = RabbitMqUtil.SaveMessage(SimpleQueue.TAG, "Send", message, queueName: queueName, routingKey: roukey, exchangeName: exchange);
                try
                {
                    RabbitMQHelper mqHelper = new RabbitMQHelper();
                    if (mqHelper.Send(message, queueName) == false)
                    {
                        RabbitMqUtil.UpdateError(resultId); //插入日至
                    }
                }
                catch (Exception)
                {
                    RabbitMqUtil.UpdateError(resultId); //插入日至
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }



        public void ReceivedTest()
        {

            var thread = new Thread(e =>
            {
                RabbitMQHelper mqHelper = new RabbitMQHelper();
                Action<string> _action = new Action<string>(SayHello);
                mqHelper.Received(Queue_Name: "WorkQuere", _action: _action);
            });
            thread.IsBackground = true;
            thread.Start();

        }
        public static void SayHello(string strMsg)
        {
            NetLog.WriteTextLog("接收到的消息", strMsg);
        }


        public void SendTest2()
        {
            var thread = new Thread(e =>
            {
                string message = "this is a work queue 1 !";
                string queueName = "";
                string roukey = "direct_login_record";
                string exchange = "direct_login";
                int resultId = RabbitMqUtil.SaveMessage(SimpleQueue.TAG, "SendByDirect", message, queueName: queueName, routingKey: roukey, exchangeName: exchange);
                try
                {
                    RabbitMQHelper mqHelper = new RabbitMQHelper();
                    //string message, string Exchange_Name, string RoutingKey
                    if (mqHelper.SendByDirect(message, exchange, roukey) == false)
                    {
                        RabbitMqUtil.UpdateError(resultId); //插入日至
                    }
                }
                catch (Exception)
                {
                    RabbitMqUtil.UpdateError(resultId); //插入日至
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }



        public void ReceivedTest2()
        {
           
            {
                string queueName = "direct_login_ok";
                string roukey = "direct_login_record";
                string exchange = "direct_login";
                var thread = new Thread(e =>
                {
                    //string Queue_Name, string Exchange_Name,string RoutingKey,
                    RabbitMQHelper mqHelper = new RabbitMQHelper();
                    Action<string> _action = new Action<string>(SayHello);
                    mqHelper.ReceivedByDirect(Queue_Name: queueName, Exchange_Name: exchange, RoutingKey: roukey, _action: _action);
                });
                thread.IsBackground = true;
                thread.Start();
            }
            {
                string queueName = "direct_login_ok";
                string roukey = "direct_login_rewards";
                string exchange = "direct_login";
                var thread = new Thread(e =>
                {
                    //string Queue_Name, string Exchange_Name,string RoutingKey,
                    RabbitMQHelper mqHelper = new RabbitMQHelper();
                    Action<string> _action = new Action<string>(SayHello);
                    mqHelper.ReceivedByDirect(Queue_Name: queueName, Exchange_Name: exchange, RoutingKey: roukey, _action: _action);
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }
   
        #endregion
    }
}
