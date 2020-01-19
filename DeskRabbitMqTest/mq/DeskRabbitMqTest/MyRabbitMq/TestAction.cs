using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyRabbitMq
{
    public class TestAction
    {
        public static void test()
        {
            NetLog.WriteTextLog("TestAction", "test()");

            ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();

            try
            {
                
                //工作队列
                TestAction.WorkQuereAction(factory);

                //消息订阅
                TestAction.SubscribeQueueAction(factory);

                //路由消息管理
                TestAction.RouteSubscribeQueueAction(factory);

            }
            catch (Exception e)
            {
                Console.WriteLine(" 失败！ " + e.Message);
            }

        }
        
        /// <summary>
        /// 工作队列
        /// </summary>
        /// <param name="factory"></param>
        public static void WorkQuereAction(ConnectionFactory factory)
        {
            BaseMQ mq = new WorkQuere(factory);
            //string[] messages = { "this is a work queue 1 ", " this is a work queue 2 ", " this is a work queue 3 ", " this is a work queue 4 ", " this is a work queue 5", " this is a work queue 6 ", " this is a work queue 7", " this is a work queue 8 ", " this is a work queue 9 ", " this is a work queue 10" };
            //mq.Send(messages);
            //mq.Send("this is a work queue 1 !");

            var thread = new Thread(e =>
            {
                mq.Received();
            });
            thread.IsBackground = true;
            thread.Start();

        }

        /// <summary>
        /// 消息订阅
        /// </summary>
        /// <param name="factory"></param>
        public static void SubscribeQueueAction(ConnectionFactory factory)
        {
            BaseMQ mq = new SubscribeQueue(factory);
            // mq.Send("this is subscribeQueue !");

            string[] messages = { "this is a SubscribeQueueAction 1 ", " this is a SubscribeQueueAction 2 ", " this is a SubscribeQueueAction 3 ", " this is a work queue 4 ", " this is a work queue 5", " this is a work queue 6 ", " this is a work queue 7", " this is a work queue 8 ", " this is a work queue 9 ", " this is a work queue 10" };
            mq.Send(messages);

            //var thread = new Thread(e =>
            //{
            //    mq.Received();
            //});
            //thread.IsBackground = true;
            //thread.Start();
        }
        /// <summary>
        /// 路由消息管理
        /// </summary>
        /// <param name="factory"></param>
        public static void RouteSubscribeQueueAction(ConnectionFactory factory)
        {
            RouteSubscribeQueue mq = new RouteSubscribeQueue(factory);
            //mq.Send("this is a sigel RouteSubscribeQueue 1!");

            string[] messages = { "this is a RouteSubscribeQueue 1 ", " this is a RouteSubscribeQueue 2 ", " this is a RouteSubscribeQueue 3 ", " this is a RouteSubscribeQueue 4 ", " this is a RouteSubscribeQueue 5", " this is a RouteSubscribeQueue 6 ", " this is a RouteSubscribeQueue 7", " this is a RouteSubscribeQueue 8 ", " this is a RouteSubscribeQueue 9 ", " this is a RouteSubscribeQueue 10" };
            //mq.Send(messages, "roukey_error");
             mq.Send(messages, "roukey_login_record");
            //mq.Send(messages, "rewards_record");
           
        }


        public static void recived()
        {
            ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();

            {
                BaseMQ mq = new WorkQuere(factory);
                var thread = new Thread(e =>
                {
                    mq.Received();
                });
                thread.IsBackground = true;
                thread.Start();
            }

            {
                BaseMQ mq = new SubscribeQueue(factory);
                var thread = new Thread(e =>
                {
                    mq.Received();
                });
                thread.IsBackground = true;
                thread.Start();
            }
            {
                BaseMQ mq = new SubscribeQueue(factory);
                var thread = new Thread(e =>
                {
                    mq.Received(SubscribeQueue.Queue_Name2);
                });
                thread.IsBackground = true;
                thread.Start();
            }
           

            //{
                
            //    RouteSubscribeQueue mq = new RouteSubscribeQueue(factory);
            //    var thread2 = new Thread(e =>
            //    {
            //        mq.Received();
            //    });
            //    thread2.IsBackground = true;
            //    thread2.Start();
            //}

            {
                RouteSubscribeQueue mq = new RouteSubscribeQueue(factory);
                var thread = new Thread(e =>
                {
                    mq.Received2("test_route_drict_fail", "roukey_error");
                });
                thread.IsBackground = true;
                thread.Start();
            }
            {
                RouteSubscribeQueue mq = new RouteSubscribeQueue(factory);
                var thread = new Thread(e =>
                {
                    mq.Received2("test_route_drict_fail", "roukey_login_record");
                });
                thread.IsBackground = true;
                thread.Start();
            }
            {
                RouteSubscribeQueue mq = new RouteSubscribeQueue(factory);
                var thread = new Thread(e =>
                {
                    mq.Received2("test_route_drict_ok", "roukey_login_record");
                });
                thread.IsBackground = true;
                thread.Start();
            }
            {
                RouteSubscribeQueue mq = new RouteSubscribeQueue(factory);
                var thread = new Thread(e =>
                {
                    mq.Received2("test_route_drict_ok", "roukey_ rewards_record");
                });
                thread.IsBackground = true;
                thread.Start();
            }
            {
                RouteSubscribeQueue mq = new RouteSubscribeQueue(factory);
                var thread = new Thread(e =>
                {
                    mq.Received2("WorkQuere", "roukey_ roukey_login_record");
                });
                thread.IsBackground = true;
                thread.Start();
            }
            
        }


    }
}
