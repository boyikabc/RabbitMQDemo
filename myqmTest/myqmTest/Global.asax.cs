using MyRabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace myqmTest
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            NetLog.WriteTextLog("Application_Start", "Application_Start");

            //Thread.Sleep(1000);
            //string url = "http://www.baidu.com";
            //HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            //Stream receiveStream = myHttpWebResponse.GetResponseStream();//得到回写的字节流  

            TestAction.recived();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}