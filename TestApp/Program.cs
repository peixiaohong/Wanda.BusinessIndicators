using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.OAMessage;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //string flowid = Guid.NewGuid().ToString();
            string flowid = "25B32D78-CBB0-4FD8-AFED-4151D28F6695";
            Console.WriteLine(flowid);
            string title = "测试-直接办结";
            string workflowName = "测试-流程名称";
            string nodeName = "创建";
            string pcurl = "192.168.50.82:84";
            string appurl = "";
            string createor = "zhengguilong";
            string reciever = "fanbing";
            //OAMessageBuilder.ReceiveTodo(flowid, title, workflowName, nodeName, pcurl, appurl, createor, reciever);
            //OAMessageBuilder.ReceiveDone(flowid, nodeName, reciever);
            OAMessageBuilder.ReceiveOver(flowid, nodeName, reciever);
            Console.ReadKey();
        }
    }
}
