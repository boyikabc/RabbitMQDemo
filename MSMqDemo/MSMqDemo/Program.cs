using RabbitMQ.Client;
using MyRabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MSMqDemo
{
    class Program
    {
        static void Main(string[] args)
        {

           Console.WriteLine("进入");

           ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();

           RouteSubscribeQueue mq = new RouteSubscribeQueue(factory);
           mq.Received();

            Console.WriteLine("结束");
            Console.ReadKey();


            return;


            string ekQ = ".\\Private$\\MSMQDemo11";
            if (!MessageQueue.Exists(ekQ))
                MessageQueue.Create(ekQ);  


            Console.WriteLine("结束");
            Console.ReadKey();

            
            return;
            MSMqHelper msmq = new MSMqHelper();

            //msmq.SendMessage();
            //msmq.ReceiveMessage();


           // msmq.SendTransMessage();
            msmq.ReceiveTransMessage();

            Console.WriteLine("结束");
            Console.ReadKey();
        }
    }
}
