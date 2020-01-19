using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;


namespace MSMqDemo
{
    public class MSMqHelper
    {
        public static string queuePath = "FormatName:DIRECT=TCP:192.168.31.125\\Private$\\myQueue";
        public static string queueTransPath = "FormatName:DIRECT=TCP:192.168.31.125\\Private$\\TransactionalQueue";


        private static MessageQueue myQueue;
        private static MessageQueue myQueueTrans;
       // private MessageQueue _transactionalQueue;
     


        public MSMqHelper()
        {
            if (myQueue==null)
            {
                myQueue = new MessageQueue(queuePath)
                {
                    Formatter = new XmlMessageFormatter(new Type[] { typeof(string) }) //序列化为字符串
                };
                Debug.WriteLine("Connect To " + queuePath + " Done!");
            }
            if (myQueueTrans == null)
            {
                myQueueTrans = new MessageQueue(queueTransPath)
                {
                    Formatter = new XmlMessageFormatter(new Type[] { typeof(string) }) //序列化为字符串
                };
                Debug.WriteLine("Connect To " + queuePath + " Done!");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void SendMessage()
        {
            try
            {
                Thread thread = new Thread(() =>
                {
                    string id = "10000000";
                    string s = JsonConvert.SerializeObject(MessageObj.GetMessageObj(id));
                    Message message = new Message();
                    message.Body = s;
                    //设置最高消息优先级
                    message.Priority = MessagePriority.Highest;
                    //发送消息到队列中
                    myQueue.Send(message);
                    Console.WriteLine("已发送");
                    //事务性消息需加上下面几句
                    //MessageQueueTransaction myTransaction = new MessageQueueTransaction();
                    //启动事务 myTransaction.Begin();
                    //myQueue.Send(myMessage, myTransaction);  //加了事务
                    //提交事务 myTransaction.Commit();
                });
                thread.IsBackground = true; // 设置为后台线程
                thread.Start();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ReceiveMessage()
        {
           
            ////注：由于消息的优先级是枚举类型，在直接messages[index].Priority.ToString();这种方式来获取优先级转化到字符串的时候，他需要一个过滤器(Filter)，否则会抛出一个InvalidCastExceptionle类型的异常，异常信息"接收消息时未检索到属性 Priority。请确保正确设置了 PropertyFilter。"，要解决这问题只需要把消息对象的MessageReadPropertyFilter（过滤器） 的Priority设置为true。
            //myQueue.MessageReadPropertyFilter.Priority = true;
            
            Message message = null;
            try
            {
                message = myQueue.Receive();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception:ReceiveMessage Exception:" + ex);
            }

            if (message != null)
            {
                //MessageObj messageObj = (MessageObj)message.Body;
                string msg = message.Body.ToString();
                Debug.WriteLine("Receive Message Done! (" + msg + ")");
                Console.WriteLine("Receive Message Done! (" + msg + ")");
            }
        }


        public void SendTransMessage()
        {
            try
            {

                if (!myQueueTrans.Transactional)
                {
                    return;
                }

                Thread thread = new Thread(() =>
                {
                    string id = "10000000";
                    string s = JsonConvert.SerializeObject(MessageObj.GetMessageObj(id));
                    var trans = new MessageQueueTransaction();
                    Message message = new Message();
                    message.Body = s;
                    //设置最高消息优先级
                    message.Priority = MessagePriority.Highest;
                    //发送消息到队列中
                    trans.Begin();
                    myQueueTrans.Send(message, trans);
                    trans.Commit();
                    Console.WriteLine("已发送");
          
                });
                thread.IsBackground = true; // 设置为后台线程
                thread.Start();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ReceiveTransMessage()
        {
            if (!myQueueTrans.Transactional)
            {
                return;
            }

            using (var messageEnumerator = myQueueTrans.GetMessageEnumerator2())
            {
                Message message = null;
                while (messageEnumerator.MoveNext())
                {
                    var trans = new MessageQueueTransaction();
                    try
                    {
                        trans.Begin();
                        message = myQueueTrans.Receive(trans);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {

                        trans.Abort();
                        Debug.WriteLine("Exception:ReceiveMessage Exception:" + ex);
                        Console.WriteLine("Exception:ReceiveMessage Exception:" + ex);
                    }
                    if (message != null)
                    {
                        var msg = message.Body.ToString();
                        Debug.WriteLine("Receive Message Done! (" + msg + ")");
                        Console.WriteLine("Receive Message Done! (" + msg + ")");
                    }
                }
            }
        }

    }
}
