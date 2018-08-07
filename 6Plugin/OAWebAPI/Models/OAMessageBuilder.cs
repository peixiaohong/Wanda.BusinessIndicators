using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using OAWebAPI.OAMessage.WebServices;
using Newtonsoft.Json;

namespace Plugin.OAMessage
{
    public static class OAMessageBuilder
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// 接收待办
        /// </summary>
        /// <param name="flowid"></param>
        /// <param name="flowtitle"></param>
        /// <param name="workflowname"></param>
        /// <param name="nodename"></param>
        /// <param name="pcurl"></param>
        /// <param name="appurl"></param>
        /// <param name="creator"></param>
        /// <param name="receiver"></param>
        public static string ReceiveTodo(string syscode,string flowid, string flowtitle, string workflowname, string nodename, string pcurl, string appurl, string creator, string receiver)
        {
            return CommonReceive(syscode,flowid, flowtitle, workflowname, nodename, pcurl, appurl, creator, receiver, 0, 0, DateTime.Now);
        }
        /// <summary>
        /// 接收已办
        /// </summary>
        /// <param name="flowid"></param>
        /// <param name="flowtitle"></param>
        /// <param name="nodename"></param>
        /// <param name="receiver"></param>
        /// <param name="pcurl"></param>
        /// <param name="appurl"></param>
        public static string ReceiveDone(string syscode, string flowid, string nodename, string receiver)
        {
            var finder = OAMessageOperator.LoadOAMessage(flowid, nodename, receiver,syscode,RequestType.Done);
            if (finder == null)
            {
                throw new OAMessageException("当前待办未找到(flowid+nodename+receiver)无法设置为已办状态");
            }
            string appUrl = finder.APPURL;
            string pcUrl = finder.PCURL;
            DateTime dt = DateTime.Now;
            DateTime.TryParse(finder.CREATETIME, out dt);
            //return CommonReceive(syscode, finder.FlowID, finder.FlowTitle, finder.WorkflowName, finder.NodeName, finder.PCUrl, finder.AppUrl, finder.CreateFlowUser, finder.ReceiverFlowUser, 2, 0, finder.CreateFlowTime);
            return CommonReceive(syscode, finder.REQUESTID,finder.REQUESTNAME, finder.WORKFLOWNAME, finder.CURRENTNODENAME,pcUrl, appUrl, finder.LOGINID, finder.JSRLOGINID, 2, 0, dt);
        }
        /// <summary>
        /// 接收办结
        /// </summary>
        /// <param name="flowid"></param>
        /// <param name="flowtitle"></param>
        /// <param name="nodename"></param>
        /// <param name="receiver"></param>
        /// <param name="pcurl"></param>
        /// <param name="appurl"></param>
        public static string ReceiveOver(string syscode, string flowid, string nodename, string receiver)
        {
            var finder = OAMessageOperator.LoadOAMessage(flowid, nodename, receiver,syscode,RequestType.Over);
            if (finder == null)
            {
                throw new OAMessageException("当前待办未找到(flowid+nodename+receiver)无法设置为办结状态");
            }
            string appUrl = finder.APPURL;
            string pcUrl = finder.PCURL;
            DateTime dt = DateTime.Now;
            DateTime.TryParse(finder.CREATETIME, out dt);
            //return CommonReceive(syscode, finder.FlowID, finder.FlowTitle, finder.WorkflowName, finder.NodeName, finder.PCUrl, finder.AppUrl, finder.CreateFlowUser, finder.ReceiverFlowUser, 4, 0, finder.CreateFlowTime);
            return CommonReceive(syscode, finder.REQUESTID, finder.REQUESTNAME, finder.WORKFLOWNAME, finder.CURRENTNODENAME, pcUrl, appUrl, finder.LOGINID, finder.JSRLOGINID, 4, 0,dt);
        }
        /// <summary>
        /// 直接办结
        /// </summary>
        /// <param name="flowid"></param>
        /// <param name="flowtitle"></param>
        /// <param name="nodename"></param>
        /// <param name="receiver"></param>
        /// <param name="pcurl"></param>
        /// <param name="appurl"></param>
        public static string ReceiveDirectOver(string syscode, string flowid, string nodename, string receiver)
        {
            var finder = OAMessageOperator.LoadOAMessage(flowid, nodename, receiver, syscode, RequestType.DirectOver);
            if (finder == null)
            {
                throw new OAMessageException("当前待办未找到(flowid+nodename+receiver)无法设置为办结状态");
            }
            string appUrl = finder.APPURL;
            string pcUrl = finder.PCURL;
            DateTime dt = DateTime.Now;
            DateTime.TryParse(finder.CREATETIME, out dt);
            //return CommonReceive(syscode, finder.FlowID, finder.FlowTitle, finder.WorkflowName, finder.NodeName, finder.PCUrl, finder.AppUrl, finder.CreateFlowUser, finder.ReceiverFlowUser, 4, 0, finder.CreateFlowTime);
            return CommonReceive(syscode, finder.REQUESTID, finder.REQUESTNAME, finder.WORKFLOWNAME, finder.CURRENTNODENAME, pcUrl, appUrl, finder.LOGINID, finder.JSRLOGINID, 4, 0, dt);
        }
        ///// <summary>
        ///// 接收已读
        ///// </summary>
        ///// <param name="flowid"></param>
        ///// <param name="nodename"></param>
        ///// <param name="receiver"></param>
        //public static string ReceiveRead(string syscode,string flowid, string nodename, string receiver)
        //{
        //    var finder = OAMessageOperator.LoadOAMessage(flowid, nodename, receiver);
        //    if (finder == null)
        //    {
        //        throw new OAMessageException("当前待办未找到(flowid+nodename+receiver)无法设置为已读状态");
        //    }
        //    //如果当前待办为已读 则不再进行更新
        //    //if (finder.ViewType == 1)
        //    //{
        //    //    return "";
        //    //}
        //    return CommonReceive(syscode,finder.FlowID, finder.FlowTitle, finder.WorkflowName, finder.NodeName, finder.PCUrl, finder.AppUrl, finder.CreateFlowUser, finder.ReceiverFlowUser, finder.FlowType, 1, finder.CreateFlowTime);
        //}
        /// <summary>
        /// 通用接收待办类
        /// </summary>
        /// <param name="flowid">流程ID</param>
        /// <param name="flowtitle">流程标题</param>
        /// <param name="workflowname">工作流名称</param>
        /// <param name="nodename">当前节点</param>
        /// <param name="pcurl">PC端审批地址</param>
        /// <param name="appurl">App端审批地址</param>
        /// <param name="creator">流程创建人</param>
        /// <param name="receiver">流程接收人</param>
        /// <param name="isremark">流程处理状态 0待办 2已办 4办结</param>
        /// <param name="viewtype">流程查看状态 0未读 1已读</param>
        private static string CommonReceive(string syscode,string flowid, string flowtitle, string workflowname, string nodename, string pcurl, string appurl, string creator, string receiver, int isremark, int viewtype, DateTime createtime)
        {
            try
            {
                using (OfsTodoDataWebService service = new OfsTodoDataWebService())
                {
                    var receiveTime = DateTime.Now;
                    var paramList = new Dictionary<string, string>();
                    paramList.Add("syscode", syscode);
                    paramList.Add("flowid", flowid);
                    paramList.Add("requestname", flowtitle);
                    paramList.Add("workflowname", workflowname);
                    paramList.Add("nodename", nodename);
                    paramList.Add("pcurl", pcurl);
                    paramList.Add("appurl", appurl);
                    paramList.Add("creator", creator);
                    paramList.Add("isremark", isremark.ToString());
                    paramList.Add("viewtype", viewtype.ToString());
                    paramList.Add("createdatetime", createtime.ToString("yyyy-MM-dd HH:mm:ss"));
                    paramList.Add("receiver", receiver);
                    paramList.Add("receivedatetime", receiveTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    var array = paramList.Select(x => new anyType2anyTypeMapEntry()
                    {
                        key = x.Key,
                        value = x.Value
                    }).ToArray();

                    var result = service.receiveRequestInfoByMap(array);
                    string ResultData = string.Join(",", result.ToList().Select(x => string.Format("{0}:{1}", x.key, x.value)));
                    logger.Info("【参数】" + JsonConvert.SerializeObject(array) + "【OA返回值】" + ResultData);
                    return ResultData;
                }
            }
            catch (Exception ex)
            {
                throw new OAMessageException("调用OA接口出现异常", ex);
            }
        }
        /*
                 /// <summary>
                /// 待办 如果之前为已办 可以重新更新成待办 如果之前为办结 可以重新更新成待办 已经删除的待办可以重新发  已经是待办的待办也可以重新发
                /// </summary>
                /// <param name="flowid">待办业务唯一编号</param>
                /// <param name="flowtitle">待办标题</param>
                /// <param name="workflowname">所属流程名称</param>
                /// <param name="nodename">当前待办人节点名称</param>
                /// <param name="pcurl">PC端待办地址</param>
                /// <param name="appurl">App端待办地址</param>
                /// <param name="creator">待办创建人</param>
                /// <param name="receiver">待办接收人</param>
                public static void Todo(string flowid, string flowtitle, string workflowname, string nodename, string pcurl, string appurl, string creator, string receiver)
                {
                    using (OfsTodoDataWebService service = new OfsTodoDataWebService())
                    {
                        var paramList = new Dictionary<string, string>();
                        paramList.Add("syscode", ConfigurationManager.AppSettings["OA.SysCode"]);
                        paramList.Add("flowid", flowid);
                        paramList.Add("requestname", flowtitle);
                        paramList.Add("workflowname", workflowname);
                        paramList.Add("nodename", nodename);
                        paramList.Add("pcurl", AppendSign(pcurl, receiver));
                        paramList.Add("appurl", AppendSign(appurl, receiver));
                        paramList.Add("creator", creator);
                        paramList.Add("createdatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        paramList.Add("receiver", receiver);
                        paramList.Add("receivedatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        var array = paramList.Select(x => new anyType2anyTypeMapEntry()
                        {
                            key = x.Key,
                            value = x.Value
                        }).ToArray();
                        var result = service.receiveTodoRequestByMap(array);
                    }
                }
                /// <summary>
                /// 已办 如果当前任务为办结更新不回已办
                /// </summary>
                /// <param name="flowid"></param>
                /// <param name="flowtitle"></param>
                /// <param name="workflowname"></param>
                /// <param name="nodename"></param>
                /// <param name="receiver"></param>
                public static void Done(string flowid, string flowtitle, string workflowname, string nodename, string receiver)
                {
                    using (OfsTodoDataWebService service = new OfsTodoDataWebService())
                    {
                        var paramList = new Dictionary<string, string>();
                        paramList.Add("syscode", ConfigurationManager.AppSettings["OA.SysCode"]);
                        paramList.Add("flowid", flowid);
                        paramList.Add("requestname", flowtitle);
                        paramList.Add("workflowname", workflowname);
                        paramList.Add("nodename", nodename);
                        paramList.Add("receiver", receiver);
                        var array = paramList.Select(x => new anyType2anyTypeMapEntry()
                        {
                            key = x.Key,
                            value = x.Value
                        }).ToArray();
                        var result = service.processDoneRequestByMap(array);
                    }
                }
                /// <summary>
                /// 办结 更新办结时不会管之前的状态是不是已办都可以直接办结
                /// </summary>
                /// <param name="flowid"></param>
                /// <param name="flowtitle"></param>
                /// <param name="workflowname"></param>
                /// <param name="nodename"></param>
                /// <param name="receiver"></param>
                public static void Over(string flowid, string flowtitle, string workflowname, string nodename, string receiver)
                {
                    using (OfsTodoDataWebService service = new OfsTodoDataWebService())
                    {
                        var paramList = new Dictionary<string, string>();
                        paramList.Add("syscode", ConfigurationManager.AppSettings["OA.SysCode"]);
                        paramList.Add("flowid", flowid);
                        paramList.Add("requestname", flowtitle);
                        paramList.Add("workflowname", workflowname);
                        paramList.Add("nodename", nodename);
                        paramList.Add("receiver", receiver);
                        var array = paramList.Select(x => new anyType2anyTypeMapEntry()
                        {
                            key = x.Key,
                            value = x.Value
                        }).ToArray();
                        var result = service.processOverRequestByMap(array);
                    }
                }
               
                     */
        /// <summary>
        /// 取消整个流程
        /// </summary>
        /// <param name="flowid"></param>
        public static string CancelProcess(string syscode,string flowid)
        {
            using (OfsTodoDataWebService service = new OfsTodoDataWebService())
            {
                var paramList = new Dictionary<string, string>();
                paramList.Add("syscode", syscode);
                paramList.Add("flowid", flowid);

                var array = paramList.Select(x => new anyType2anyTypeMapEntry()
                {
                    key = x.Key,
                    value = x.Value
                }).ToArray();
                var result = service.deleteRequestInfoByMap(array);
                return string.Join(",", result.ToList().Select(x => string.Format("{0}:{1}", x.key, x.value)));
            }
        }
        /// <summary>
        /// 撤销 如果之前为办结 无法撤销
        /// </summary>
        /// <param name="flowid"></param>
        /// <param name="receiver"></param>
        public static string Cancel(string syscode,string flowid, string receiver)
        {
            using (OfsTodoDataWebService service = new OfsTodoDataWebService())
            {
                var paramList = new Dictionary<string, string>();
                paramList.Add("syscode", syscode);
                paramList.Add("flowid", flowid);
                paramList.Add("userid", receiver);

                var array = paramList.Select(x => new anyType2anyTypeMapEntry()
                {
                    key = x.Key,
                    value = x.Value
                }).ToArray();
                var result = service.deleteUserRequestInfoByMap(array);
                return string.Join(",", result.ToList().Select(x => string.Format("{0}:{1}", x.key, x.value)));
            }
        }
    }
}
