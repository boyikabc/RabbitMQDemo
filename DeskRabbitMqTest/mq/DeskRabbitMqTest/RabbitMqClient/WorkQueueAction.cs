using MyRabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqClient
{
    public class WorkQueueAction
    {
        public const string TAG = "WorkQueueAction";
        public const string Queue_Name = "WorkQueueAction";

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="message">this is WorkQueueAction 1 !</param>
        /// <returns></returns>
        public bool Send(string message)
        {
            if (string.IsNullOrEmpty(message)) return false;
            var thread = new Thread(e =>
            {
                int resultId = RabbitMqUtil.SaveMessage(SimpleQueue.TAG, "Send", message, queueName: Queue_Name, routingKey: "", exchangeName: "");
                try
                {
                    RabbitMQHelper mqHelper = new RabbitMQHelper();
                    if (mqHelper.Send(message, Queue_Name) == false)
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
            var thread = new Thread(e =>
            {
                RabbitMQHelper mqHelper = new RabbitMQHelper();
                Action<string> _action = new Action<string>(Received_SayHello);
                mqHelper.Received(Queue_Name: Queue_Name, _action: _action);
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public static void Received_SayHello(string strMsg)
        {
            NetLog.WriteTextLog("接收到的消息", strMsg);
        }


    }
}
