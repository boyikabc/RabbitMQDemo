using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRabbitMq
{

    public class SubscribeQueue : BaseMQ
    {
        public const string Exchange_Name = "logs_fanout";
        public const string Queue_Name = "logs_fanout_queue";

        public ConnectionFactory factory = null;
        public SubscribeQueue(ConnectionFactory _factory)
        {
            factory = _factory;
        }

        public override void Send(string message)
        {
            if (message != null && message != "")
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Fanout,
                        durable: true, autoDelete: false, arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);
                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ConfirmSelect();
                    
                    //发布消息
                    channel.BasicPublish(exchange: Exchange_Name,
                                     routingKey: "",
                                     basicProperties: properties,
                                     body: body);

                    Console.WriteLine(" [x] Sent {0}", message);

                    if (channel.WaitForConfirms())
                    {
                        Console.WriteLine("发送成功");
                    }
                    else
                    {
                        Console.WriteLine("发送失败");
                    }
                }
            }

            
        }

        public override void Send(string[] messageList)
        {
            if (messageList != null && messageList.Length>0)
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //声明交换机
                    channel.ExchangeDeclare(exchange: Exchange_Name, type: ExchangeType.Fanout,
                       durable: true, autoDelete: false, arguments: null);
                   
                    //确认手动应答之前，只分发给同一个消费者一个消息
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.ConfirmSelect();
                    for (int i = 0; i < messageList.Length; i++)
                    {
                        //发布消息
                       
                        string message = messageList[i];
                        //发布消息
                        channel.BasicPublish(
                            exchange: Exchange_Name,
                            routingKey: "",
                             basicProperties: properties,
                             body: Encoding.UTF8.GetBytes(message)
                             );
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
                }
            }
        }

        public override void Received()
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
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] {0}", message);
                    //手动回执
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: queueOk.QueueName,
                                 autoAck: false,
                                 consumer: consumer);

            }
        }
    }
}
