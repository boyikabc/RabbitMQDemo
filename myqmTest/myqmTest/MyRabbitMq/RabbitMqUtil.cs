using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyRabbitMq
{
    /// <summary>
    /// 简单分发模式
    /// 
    /// </summary>
    public class RabbitMqUtil
    {

        public static bool isTest = false;

        public static string SqlServerConnection = string.Format("Data Source=47.92.104.120;Initial Catalog=StudentDB;Persist Security Info=True;User ID=sa;password=Hxt1597");

        public static ConnectionFactory GetConnectionFactory()
        {
            ConnectionFactory factory = new ConnectionFactory();

            if (isTest == false)
            {
                //正式环境
                factory.HostName = "47.92.104.120";
                factory.VirtualHost = "/vhost_school";
                factory.Port = 5672;
                factory.UserName = "school";
                factory.Password = "81234567";
            }
            else
            {
                //测试环境
                factory.HostName = "127.0.0.1";
                factory.VirtualHost = "/vhost_school";
                factory.Port = 5672;
                factory.UserName = "myrabbit";
                factory.Password = "81234567";
            }


            return factory;
        }

        public static void ReceivedTest()
        {
            ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();
            var thread = new Thread(e =>
            {
                BaseMQ mq = new WorkQuere(factory);
                mq.Received();
            });
            thread.IsBackground = true;
            thread.Start();
        }
        public static void SendTest()
        {
            var thread = new Thread(e =>
            {
                string message = "this is a work queue 1 !";
                int resultId = RabbitMqUtil.SaveMessage(SimpleQueue.TAG, message);
                try
                {
                    ConnectionFactory factory = RabbitMqUtil.GetConnectionFactory();
                    BaseMQ mq = new SimpleQueue(factory);
                    if (!mq.Send(message))
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

        }
        public static void HandleReceived(string msg)
        {
            string s = string.Format("Message:{0}", msg);
            Console.WriteLine(s);
            NetLog.WriteTextLog("Handle", s);
            Thread.Sleep(1000);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="msg"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static int SaveMessage(string className, string msg, int flag = 0)
        {
            int result = 0;
            try
            {
                string conTStr = string.Format("Data Source=47.92.104.120;Initial Catalog=StudentDB;Persist Security Info=True;User ID=sa;password=Hxt1597").ToString();
                using (SqlConnection conn = new SqlConnection(conTStr))
                {
                    string sql = "INSERT INTO RabbitMessageLog(className,msg,flag) VALUES(@className,@msg,@flag);select @@Identity";
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.Parameters.AddWithValue("@className", className);
                    cmd.Parameters.AddWithValue("@msg", msg);
                    cmd.Parameters.AddWithValue("@flag", flag);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;//设置SQL语句 
                    cmd.Connection = conn;//调用打开数据库连接方法 
                    object val = cmd.ExecuteScalar();
                    result = Convert.ToInt32(val);
                }
            }
            catch (Exception e)
            {
                string err = string.Format("内容：{0},异常：{1}", msg, e.Message);
                NetLog.WriteTextLog("消息保存失败：" + className, err);
            }
            return result;
        }

        public static int UpdateError(int id)
        {
            int result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(RabbitMqUtil.SqlServerConnection))
                {
                    string sql = "update RabbitMessageLog set flag=0 where id=@id";
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;//设置SQL语句 
                    cmd.Connection = conn;//调用打开数据库连接方法 
                    result = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {

            }
            return result;
        }



    }
}
