using MyRabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqClient
{
    public class SubscribeQueueAction
    {
        public const string TAG = "SubscribeQueueAction";
        public const string Queue_Name = "SubscribeQueueAction";
        public const string Exchange_Name = "fanout_login";

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="message">this is  SubscribeQueueAction 1 !</param>
        /// <returns></returns>
        public bool Send(string message)
        {
            if (string.IsNullOrEmpty(message)) return false;
           
            var thread = new Thread(e =>
            {
                int resultId = RabbitMqUtil.SaveMessage(SimpleQueue.TAG, "SendByFanout", message, queueName: "", routingKey: "", exchangeName: Exchange_Name);
                try
                {
                    RabbitMQHelper mqHelper = new RabbitMQHelper();
                    if (mqHelper.SendByFanout(message, Exchange_Name) == false)
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
            return true;
        }

        /// <summary>
        /// Received
        /// </summary>
        public void Received()
        {
            {
                string queueName = Queue_Name;
                var thread = new Thread(e =>
                {
                    RabbitMQHelper mqHelper = new RabbitMQHelper();
                    Action<string> _action = new Action<string>(Received_SayHello);
                    mqHelper.ReceivedByFanout(Queue_Name: queueName, Exchange_Name: Exchange_Name, _action: _action);
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }
        private static void Received_SayHello(string strMsg)
        {
            NetLog.WriteTextLog("接收到的消息", strMsg);
        }
    }
}
