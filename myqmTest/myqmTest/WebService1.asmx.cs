using MyRabbitMq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace myqmTest
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        [WebMethod]
        public string helloToString()
        {
            string s = "";

            TestAction.test();
            return "" + s;
        }
        [WebMethod]
        public string hello()
        {
            string s = "";

            ReceivedCalledAction.doHandleReviced();


            //s = TaskSettingAction.getTastList();
            //s = TaskSettingAction.newClass("MyRabbitMq", "SubscribeQueue");
            //s = TaskSettingAction.SimpleMethod("MyRabbitMq", "Test");
            //s = TaskSettingAction.ResultMethod("MyRabbitMq", "Test");
            //s = TaskSettingAction.ParemMethod("MyRabbitMq", "Test");
            //s = TaskSettingAction.OverloadMethod("MyRabbitMq", "Test");
            //s = TaskSettingAction.OverrideMethod("MyRabbitMq", "Test");
            //s = TaskSettingAction.ReciveMethod("MyRabbitMq", "SimpleQueue");
            
            return "Hello World s : "+s;
        }

        [WebMethod]
        public string initMq()
        {
            TestAction.test();
            return "Hello World";
        }

        [WebMethod]
        public string SendMul()
        {
            ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();
            BaseMQ mq = new WorkQuere(factory);
            string[] messages = { "this is a work queue 1 ", " this is a work queue 2 ", " this is a work queue 3 ", " this is a work queue 4 ", " this is a work queue 5", " this is a work queue 6 ", " this is a work queue 7", " this is a work queue 8 ", " this is a work queue 9 ", " this is a work queue 10" };
            mq.Send(messages);
            return "Hello World";
        }

        [WebMethod]
        public string SendOne()
        {
            ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();
            BaseMQ mq = new WorkQuere(factory);
            mq.Send("this is a work queue 1 !");
            return "Hello World";
        }

        [WebMethod]
        public string RouteSubscribeQueueSend()
        {
            ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();
            BaseMQ mq = new RouteSubscribeQueue(factory);
            string[] messages = { "this is a Route queue 1 ", " this is a Route queue 2 ", " this is a Route queue 3 ", " this is a Route queue 4 ", " this is a Route queue 5", " this is a Route queue 6 ", " this is a Route queue 7", " this is a Route queue 8 ", " this is a Route queue 9 ", " this is a Route queue 10" };
            mq.Send(messages);
            return "Hello World";
        }
    }
}
