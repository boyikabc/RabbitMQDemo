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
    public class RabbitMqUtil
    {
        public static ConnectionFactory GetConnectionFactory()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            factory.VirtualHost = "/vhost_mmr";
            factory.Port = 5672;
            factory.UserName = "myrabbit";
            factory.Password = "81234567";
            return factory;
        }

    }
}
