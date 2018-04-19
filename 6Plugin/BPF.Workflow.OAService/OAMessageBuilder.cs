using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Plugin.OAMessage.OAMessage.WebServices;

namespace BPF.OAMQServices
{
    public static class OAMessageBuilder
    {
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
        public static void CommonReceive(string flowid, string flowtitle, string workflowname, string nodename, string pcurl, string appurl, string creator, string receiver, int isremark, int viewtype, DateTime createtime)
        {
            try
            {
                using (Plugin.OAMessage.OAMessage.WebServices.OfsTodoDataWebService service = new Plugin.OAMessage.OAMessage.WebServices.OfsTodoDataWebService())
                {
                    var receiveTime = DateTime.Now;
                    var paramList = new Dictionary<string, string>();
                    paramList.Add("syscode", ConfigurationManager.AppSettings["OA.SysCode"]);
                    paramList.Add("flowid", flowid);
                    paramList.Add("requestname", flowtitle);
                    paramList.Add("workflowname", workflowname);
                    paramList.Add("nodename", nodename);
                    paramList.Add("pcurl", SSOToolkit.Instance.GetAuthOAUrlWithSSO(pcurl, receiver, flowid));
                    paramList.Add("appurl", SSOToolkit.Instance.GetAuthOAUrlWithSSO(appurl, receiver, flowid));
                    paramList.Add("creator", creator);
                    paramList.Add("isremark", isremark.ToString());
                    paramList.Add("viewtype", viewtype.ToString());
                    paramList.Add("createdatetime", createtime.ToString("yyyy-MM-dd HH:mm:ss"));
                    paramList.Add("receiver", receiver);
                    paramList.Add("receivedatetime", receiveTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    var array = paramList.Select(x => new Plugin.OAMessage.OAMessage.WebServices.anyType2anyTypeMapEntry()
                    {
                        key = x.Key,
                        value = x.Value
                    }).ToArray();
                    var result = service.receiveRequestInfoByMap(array);
                }
            }
            catch (Exception ex)
            {
                throw new OAMessageException("调用OA接口出现异常", ex);
            }
        }      
        ///// <summary>
        ///// 取消整个流程
        ///// </summary>
        ///// <param name="flowid"></param>
        //public static void CancelProcess(string flowid)
        //{
        //    using (OAMessage.WebServices.OfsTodoDataWebService service = new OAMessage.WebServices.OfsTodoDataWebService())
        //    {
        //        var paramList = new Dictionary<string, string>();
        //        paramList.Add("syscode", ConfigurationManager.AppSettings["OA.SysCode"]);
        //        paramList.Add("flowid", flowid);

        //        var array = paramList.Select(x => new OAMessage.WebServices.anyType2anyTypeMapEntry()
        //        {
        //            key = x.Key,
        //            value = x.Value
        //        }).ToArray();
        //        var result = service.deleteRequestInfoByMap(array);
        //    }
        //}
        ///// <summary>
        ///// 撤销 如果之前为办结 无法撤销
        ///// </summary>
        ///// <param name="flowid"></param>
        ///// <param name="receiver"></param>
        //public static void Cancel(string flowid, string receiver)
        //{
        //    using (OAMessage.WebServices.OfsTodoDataWebService service = new OAMessage.WebServices.OfsTodoDataWebService())
        //    {
        //        var paramList = new Dictionary<string, string>();
        //        paramList.Add("syscode", ConfigurationManager.AppSettings["OA.SysCode"]);
        //        paramList.Add("flowid", flowid);
        //        paramList.Add("userid", receiver);

        //        var array = paramList.Select(x => new OAMessage.WebServices.anyType2anyTypeMapEntry()
        //        {
        //            key = x.Key,
        //            value = x.Value
        //        }).ToArray();
        //        var result = service.deleteUserRequestInfoByMap(array);
        //    }
        //}
    }
}
