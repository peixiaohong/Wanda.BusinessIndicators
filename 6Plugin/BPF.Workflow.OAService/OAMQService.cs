using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Collections;


namespace BPF.OAMQServices
{
    public partial class OAMQService : ServiceBase
    {
        Thread workerThread = null;
        bool _shouldStop = false;
        //int intervalSeconds = 20;

        public OAMQService()
        {
            InitializeComponent();
        }

        public void Start()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            int intervalMilliseconds = GetIntervalSeconds() * 1000;
            
            workerThread = new Thread(delegate()
            {
                try
                {
                    
                    while (!_shouldStop)
                    {
                        //Common.Log.Info("开始发送消息，ManagedThreadId=" + Thread.CurrentThread.ManagedThreadId);
                        Common.Log.Info("开始发送消息...");
                        try
                        {
                            OAMQBll.Instance.SendMessages();
                        }
                        catch (Exception ex)
                        {
                            Common.Log.Error("发送消息失败：" + ex.ToString());
                        }
                        //Common.Log.Info("发送消息结束，ManagedThreadId=" + Thread.CurrentThread.ManagedThreadId);
                        Common.Log.Info("发送消息结束。");
                        Common.Log.Info("=======================等待下次发送========================");
                        Common.Log.Info("");
                        Thread.Sleep(intervalMilliseconds);
                    }
                    
                }
                catch (Exception e)
                {
                    Common.Log.Error("Service启动失败：" + e.ToString());
                }
            });
            Common.Log.Info("=========================准备启动==========================");
            Common.Log.Info("Service开始启动");
            workerThread.Start();
            while (!workerThread.IsAlive) ;
            Common.Log.Info("Service启动成功");
            Common.Log.Info("=========================准备发送==========================");
        }

        protected override void OnStop()
        {
            try
            {
                Common.Log.Info("=========================准备停止==========================");
                Common.Log.Info("Service开始停止");
                if (workerThread != null)
                {
                    Common.Log.Info("开始停止线程，ManagedThreadId=" + workerThread.ManagedThreadId);
                    _shouldStop = true;
                    workerThread.Join();
                    Common.Log.Info("停止线程结束");
                }
                Common.Log.Info("Service停止成功");
            }
            catch (Exception e)
            {
                Common.Log.Error("当试图停止Service时发生错误: " + e.ToString(), e);
            }
        }


        private int GetIntervalSeconds()
        {
            string sIntervalSeconds = System.Configuration.ConfigurationManager.AppSettings["IntervalSeconds"];
            int intervalSeconds = 0;
            if (!int.TryParse(sIntervalSeconds, out intervalSeconds) || intervalSeconds <= 0)
            {
                intervalSeconds = 20;
            }
            return intervalSeconds;
        }
    }
}
