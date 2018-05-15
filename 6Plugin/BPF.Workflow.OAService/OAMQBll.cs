using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using Plugin.OAMessage.OAMessage.WebServices;
using BPF.OAMQMessages.DAL;
using BPF.OAMQMessages.Entities;


namespace BPF.OAMQServices
{
    /// <summary>
    /// 与OA的MQ交互
    /// </summary>
    public class OAMQBll
    {
        public static OAMQBll _Instance = new OAMQBll();
        /// <summary>
        /// 实例
        /// </summary>
        public static OAMQBll Instance
        {
            get
            {
                return _Instance;
            }
        }

        public int SendMode
        {
            get
            {
                if (_SendMode == null)
                {
                    string sendMode = System.Configuration.ConfigurationManager.AppSettings["SendMode"];
                    sendMode = string.IsNullOrEmpty(sendMode) ? string.Empty : sendMode.Trim();
                    switch (sendMode)
                    {
                        case "":
                        case "0":
                            _SendMode = 0;
                            break;
                        case "1":
                            _SendMode = 1;
                            break;
                        case "2":
                            _SendMode = 2;
                            break;
                        default:
                            throw new Exception("SenderMode配置错误");
                    }
                }
                return (int)_SendMode;
            }
        }
        private int? _SendMode = null;

        public string SenderCode
        {
            get
            {
                if (_SenderCode == null)
                {
                    _SenderCode = System.Configuration.ConfigurationManager.AppSettings["SenderCode"];
                    _SenderCode = _SenderCode == null ? string.Empty : _SenderCode.Trim();
                    if (_SenderCode == string.Empty)
                    {
                        throw new Exception("SenderCode值为空或没有找到SenderCode配置项");
                    }
                }
                return _SenderCode;
            }
        }
        private string _SenderCode = null;

        /// <summary>
        /// 错误消息接收人
        /// </summary>
        public string[] ErrorReceiver
        {
            get
            {
                if (_ErrorReceiver == null)
                {
                    string receivers = System.Configuration.ConfigurationManager.AppSettings["ErrorReceiver"] ?? string.Empty;
                    _ErrorReceiver = receivers.Split(',');
                    for (int i = 0; i < _ErrorReceiver.Length; i++)
                    {
                        _ErrorReceiver[i] = _ErrorReceiver[i].Trim();
                    }
                }
                return _ErrorReceiver;
            }
        }
        private string[] _ErrorReceiver = null;

        //发送消息
        public void SendMessages()
        {
            int successCount = 0;
            List<OAMQMessage> list = null;
            try
            {
                int num = 1;
                OAMQAdapter.Instance.DbConnection.Open();
                do
                {
                    Common.Log.Info(num + ".准备获取待发送消息队列...");
                    list = LoadMessageListFromSource();
                    if (list.Count > 0)
                    {
                        Common.Log.Info("  获取 {0} 条待发送消息，准备发送...", list.Count);
                        int i = SendMessageList2MQ(list);
                        Common.Log.Info("  成功发送 {0} 条消息到MQ。", i);
                        successCount += i;
                    }
                    else
                    {
                        Common.Log.Info("  没有获取到可发送的消息。");
                    }
                    num++;
                    System.Threading.Thread.Sleep(1);
                } while (list.Count > 0);
                Common.Log.Info("");
                Common.Log.Info("本次消息发送完成，共发送成功 {0} 条消息。", successCount);
            }
            finally
            {
                OAMQAdapter.Instance.DbConnection.Close();
            }
        }

        /// <summary>
        /// 加载等待发送的消息
        /// </summary>
        /// <returns></returns>
        private List<OAMQMessage> LoadMessageListFromSource()
        {
            int count = 10;
            string str = System.Configuration.ConfigurationManager.AppSettings["ActivceMQ_SendCount"];
            if (!int.TryParse(str, out count))
            {
                count = 10;
            }
            return OAMQAdapter.Instance.LoadList(count);
        }

        /// <summary>
        /// 发送消息列表到MQ
        /// </summary>
        /// <param name="list"></param>
        private int SendMessageList2MQ(List<OAMQMessage> list)
        {
            int successCount = 0;
            int errorCount = 10;
            string strActiveMQErrorCount = System.Configuration.ConfigurationManager.AppSettings["ActivceMQ_ErrorCount"];
            Int32.TryParse(strActiveMQErrorCount, out errorCount);
            try
            {
                foreach (OAMQMessage message in list)
                {

                    try
                    {
                        if (message.Flowmess == 9)
                        {
                            var arrayParam = BuildMQMessageDelete(message);
                            using (Plugin.OAMessage.OAMessage.WebServices.OfsTodoDataWebService service = new Plugin.OAMessage.OAMessage.WebServices.OfsTodoDataWebService())
                            {
                                var result = service.deleteUserRequestInfoByMap(arrayParam);
                                message.MessageRemark = string.Join(",", result.ToList().Select(x => string.Format("{0}:{1}", x.key, x.value)));
                            }
                        }
                        else
                        {
                            var arrayParam = BuildMQMessage(message);
                            using (Plugin.OAMessage.OAMessage.WebServices.OfsTodoDataWebService service = new Plugin.OAMessage.OAMessage.WebServices.OfsTodoDataWebService())
                            {
                                Common.Log.Info("【输入】{0}",Newtonsoft.Json.JsonConvert.SerializeObject(arrayParam));
                                var result = service.receiveRequestInfoByMap(arrayParam);
                                message.MessageRemark = string.Join(",", result.ToList().Select(x => string.Format("{0}:{1}", x.key, x.value)));

                            }
                        }
                        message.Status = 1;
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        message.ErrorCount += 1;
                        //如果大于重发次数，标记为发送失败
                        if (message.ErrorCount >= errorCount)
                            message.Status = 2;
                        else
                            message.Status = 0;

                        //将下面的异常写入到log中
                        BPF.OAMQServices.Common.Log.Error("发送消息到MQ出错，消息编号：" + message.MessageId, ex);
                    }
                    //更新消息表
                    OAMQAdapter.Instance.Update(message);
                }
                return successCount;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 构造一个MQ消息对象
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prod"></param>
        /// <returns></returns>
        private anyType2anyTypeMapEntry[] BuildMQMessage(OAMQMessage message)
        {

            var receiveTime = DateTime.Now;
            var paramList = new Dictionary<string, string>();
            paramList.Add("syscode", ConfigurationManager.AppSettings["OA.SysCode"]);
            paramList.Add("flowid", message.ProcessID);
            paramList.Add("requestname", message.Title);
            paramList.Add("workflowname", message.Flowtype);
            paramList.Add("nodename", message.Nodename);
            paramList.Add("pcurl", SSOToolkit.Instance.GetAuthOAUrlWithSSO(message.PtpUrl.Trim(), message.Userid, message.FlowID));
            if (message.Nodename=="填报人")
            {
                paramList.Add("appurl", "");
            }
            else
            {
                paramList.Add("appurl", SSOToolkit.Instance.GetAuthOAUrlWithSSO(message.MobileUrl.Trim(), message.Userid, message.FlowID));
            }
            paramList.Add("creator", message.Creator);
            paramList.Add("isremark", message.Flowmess.ToString());
            //paramList.Add("viewtype", message.Viewtype.ToString());
            paramList.Add("createdatetime", message.Createtime.ToString("yyyy-MM-dd HH:mm:ss"));
            paramList.Add("receiver", message.Userid);
            paramList.Add("receivedatetime", receiveTime.ToString("yyyy-MM-dd HH:mm:ss"));
            var array = paramList.Select(x => new Plugin.OAMessage.OAMessage.WebServices.anyType2anyTypeMapEntry()
            {
                key = x.Key,
                value = x.Value
            }).ToArray();
            return array;
        }

        private anyType2anyTypeMapEntry[] BuildMQMessageDelete(OAMQMessage message)
        {
            var paramList = new Dictionary<string, string>();
            paramList.Add("syscode", ConfigurationManager.AppSettings["OA.SysCode"]);
            paramList.Add("flowid", message.FlowID);
            paramList.Add("userid", message.Userid);

            var array = paramList.Select(x => new Plugin.OAMessage.OAMessage.WebServices.anyType2anyTypeMapEntry()
            {
                key = x.Key,
                value = x.Value
            }).ToArray();
            return array;
        }

        /// <summary>
        /// 处理消息标题的长度
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string ProcessTitle(string title)
        {
            if (title.Length > 66)
            {
                return title.Substring(0, 66) + "...";
            }
            return title;
        }
    }
}
