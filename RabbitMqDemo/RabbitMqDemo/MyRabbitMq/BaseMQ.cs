using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRabbitMq
{
    public abstract class BaseMQ
    {

        public BaseMQ()
        { 
        
        }
        /// <summary>
        /// 发送单条消息
        /// </summary>
        /// <param name="message"></param>
        public abstract bool Send(string message);
        /// <summary>
        /// 发送多条消息
        /// </summary>
        /// <param name="messageList"></param>
        public abstract bool Send(string[] messageList);

        /// <summary>
        /// 接收消息
        /// </summary>
        public abstract void Received();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routingKey"></param>
        public abstract void Received(string routingKey);
    }
}
