
using MyRabbitMq;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Xml;

namespace RabbitMqClient
{
    public class ReceivedCalledAction
    {
        public static string doHandleReviced()
        {
            List<Received> Receiveds = ReceivedCalledAction.getReceivedList();
            int r_count = 1;
            foreach (Received received in Receiveds)
            {
                if (received.Ignore)
                    continue;
                initReceived(received);
            }
            NetLog.WriteTextLog("r_count：", "" + r_count++);
            NetLog.WriteTextLog("配置文件接收数量：",Receiveds.Count + "");
            return Receiveds.Count + "";
        }

        private static List<Received> getReceivedList()
        {
            List<Received> recevieds = new List<Received>();
            string path = AppDomain.CurrentDomain.BaseDirectory + "ReceivedSetting.xml";

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;//忽略文档里面的注释
            XmlReader reader = XmlReader.Create(@path, settings);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);
            XmlNodeList xNode = xmlDoc.SelectNodes("//Received");
            foreach (XmlElement element in xNode)
            {
                string id = element.Attributes["Id"].Value;
                string Ignore = element.Attributes["Ignore"].Value;
                string ReceivedName = element.Attributes["ReceivedName"].Value;
                string Namespace = element.GetElementsByTagName("Namespace")[0].InnerText;
                string _class = element.GetElementsByTagName("Class")[0].InnerText;
                string method = element.GetElementsByTagName("Method")[0].InnerText;
                Received re = new Received()
                {
                    Id = id,
                    Ignore = Convert.ToBoolean(Ignore),
                    ReceivedName = ReceivedName,
                    Namespace = Namespace,
                    Class = _class,
                    Method = method
                };
                recevieds.Add(re);
            }
            reader.Close();
            return recevieds;
        }

        private static void initReceived(Received received)
        {
            try
            {
                string Namespace = received.Namespace;
                string className = received.Class;
                Assembly mockAssembly = Assembly.GetExecutingAssembly();
                Type type = mockAssembly.GetType(Namespace + "." + className);
                object objPerson = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod("" + received.Method, new Type[0]);
                object result = method.Invoke(objPerson, null);
            }
            catch (Exception er)
            {
                throw er;
            }
        }

    }

    public class Received
    {
        public string Id
        {
            get;
            set;
        }
        public string ReceivedName
        {
            get;
            set;
        }
        public bool Ignore
        {
            get;
            set;
        }
        public string Namespace
        {
            get;
            set;
        }
        public string Class
        {
            get;
            set;
        }
        public string Method
        {
            get;
            set;
        }
    }
}