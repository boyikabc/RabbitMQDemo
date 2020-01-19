using RabbitMQ.Client;
using MyRabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            TestAction.test();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        
        }



    }
}
