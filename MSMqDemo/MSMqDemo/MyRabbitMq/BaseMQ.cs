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
       
        public BaseMQ() { 
        
        }
        public abstract void Send(string message);

        public abstract void Send(string[] messageList);

        public abstract void Received();
    }
}
