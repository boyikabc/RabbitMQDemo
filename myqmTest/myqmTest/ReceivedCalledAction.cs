
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

namespace myqmTest
{
    public class ReceivedCalledAction
    {
        public static string doHandleReviced()
        {
            List<Received> Receiveds = ReceivedCalledAction.getReceivedList();

            ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();

            foreach (Received r in Receiveds)
            {
                initReceived(factory,r);
            }

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
                Received re = new Received()
                {
                    Id = id,
                    Ignore = Convert.ToBoolean(Ignore),
                    ReceivedName = ReceivedName,
                    Namespace = Namespace,
                    Class = _class
                };
                recevieds.Add(re);
            }
            reader.Close();
            return recevieds;
        }

        private static void initReceived(ConnectionFactory factory, Received received)
        {
            string Namespace = received.Namespace;
            string className = received.Class;
            if(!received.Ignore){
                var thread2 = new Thread(e =>
                {
                    try
                    {
                        Assembly mockAssembly = Assembly.GetExecutingAssembly();
                        Type type = mockAssembly.GetType(Namespace + "." + className);// Assembly.Load(Namespace).GetType(className);
                        object objPerson = Activator.CreateInstance(type, new object[] { factory });
                        MethodInfo method = type.GetMethod("Received", new Type[0]);
                        object result = method.Invoke(objPerson, null);
                    } catch (Exception er)   { }
                });
                thread2.IsBackground = true;
                thread2.Start();
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
    }
}