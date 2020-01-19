using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRabbitMq
{
    public class Test
    {
        public static void test() {

            ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();

            BaseMQ mq = new WorkQuere(factory);
            //string[] messages = { "this is a work queue 1 ", " this is a work queue 2 ", " this is a work queue 3 ", " this is a work queue 4 ", };
            //mq.Send("this is a work queue 1 !");
            //mq.Send(messages);
            //mq.Received();

            mq = new SubscribeQueue(factory);
            //mq.Send("this is subscribeQueue ");
            //mq.Received();

            mq = new RouteSubscribeQueue(factory);
            //mq.Send("this is RouteSubscribeQueue ");
            mq.Received();

        }
    }
}
