using MyRabbitMq;
using RabbitMQ.Client;
using RabbitMqClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeskRabbitMqTest
{
    public partial class Form1 : Form
    {
        private bool isRecived=false;

        public Form1()
        {
            InitializeComponent();
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        //从配置文件接收
        private void button1_Click(object sender, EventArgs e)
        {
            if (isRecived)
                return;
            ReceivedCalledAction.doHandleReviced();
            isRecived = true;
        }

        //基本订阅模式发送
        private void button2_Click(object sender, EventArgs e)
        {
            SubscribeQueueAction wq = new SubscribeQueueAction();
            for (int i = 0; i < 10; i++)
            {
                string message = string.Format("this is SubscribeQueueAction {0} !", i + 1);
                wq.Send(message);
                Thread.Sleep(1000);
            }
        }

        //接收基本订阅模式
        private void button3_Click(object sender, EventArgs e)
        {
            SubscribeQueueAction wq = new SubscribeQueueAction();
            wq.Received();
        }
        //发送工作队列消息
        private void button4_Click(object sender, EventArgs e)
        {
            WorkQueueAction wq = new WorkQueueAction();

            for (int i = 0; i < 10; i++)
            {
                string message = string.Format("this is a work queue {0} !", i + 1);
                wq.Send(message);
                Thread.Sleep(1000);
            }
        }
       //接收工作队列
        private void button5_Click(object sender, EventArgs e)
        {
            WorkQueueAction wq = new WorkQueueAction();
            wq.Received();
        }

      
        private void button6_Click(object sender, EventArgs e)
        {
           
        }

       
        private void button7_Click(object sender, EventArgs e)
        {
            
        }

        





    }
}
