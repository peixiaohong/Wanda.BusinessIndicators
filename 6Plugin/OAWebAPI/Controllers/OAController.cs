using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Plugin.Filters;
using Plugin.OAMessage;

namespace OAWebAPI.Controllers
{
    public class OAController : BaseController
    {
        [HttpPost]
        public string test(OAMessageVM p)
        {
            //return "Success!";
            throw new Exception("CESHI ");
        }
        /// <summary>
        /// 创建待办
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public commonResult ReceiveTodo(OAMessageVM p)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.syscode) || string.IsNullOrWhiteSpace(p.flowid) || string.IsNullOrWhiteSpace(p.workflowname)
                || string.IsNullOrWhiteSpace(p.nodename) || string.IsNullOrWhiteSpace(p.pcurl) || string.IsNullOrWhiteSpace(p.creator) || string.IsNullOrWhiteSpace(p.receiver)
                || string.IsNullOrWhiteSpace(p.requestname))
            {
               return commonResult.NullParameter();
            }
            if (p.createdatetime == null)
                p.createdatetime = DateTime.Now;
            if (p.receivedatetime == null)
                p.receivedatetime = DateTime.Now;
            string res = OAMessageBuilder.ReceiveTodo(p.syscode, p.flowid, p.requestname, p.workflowname, p.nodename, p.pcurl, p.appurl, p.creator, p.receiver);
            return commonResult.Success(res);
        }
        /// <summary>
        /// 待办变已办
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public commonResult ReceiveDone(OAMessageVM p)
        {
            //if (p == null || string.IsNullOrWhiteSpace(p.syscode) || string.IsNullOrWhiteSpace(p.flowid) || string.IsNullOrWhiteSpace(p.nodename) || string.IsNullOrWhiteSpace(p.receiver))
            //    return commonResult.NullParameter();
            string res = OAMessageBuilder.ReceiveDone(p.syscode,p.flowid,p.nodename,p.receiver);
            return commonResult.Success(res);
        }
        /// <summary>
        /// 待办变办结
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public commonResult ReceiveOver(OAMessageVM p)
        {
            //if (p == null || string.IsNullOrWhiteSpace(p.syscode) || string.IsNullOrWhiteSpace(p.flowid) || string.IsNullOrWhiteSpace(p.nodename) || string.IsNullOrWhiteSpace(p.receiver))
            //    return commonResult.NullParameter();

            string res = OAMessageBuilder.ReceiveOver(p.syscode, p.flowid, p.nodename, p.receiver);
            return commonResult.Success(res);
        }
        /// <summary>
        /// 待办变办结
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public commonResult ReceiveDirectOver(OAMessageVM p)
        {
            //if (p == null || string.IsNullOrWhiteSpace(p.syscode) || string.IsNullOrWhiteSpace(p.flowid) || string.IsNullOrWhiteSpace(p.nodename) || string.IsNullOrWhiteSpace(p.receiver))
            //    return commonResult.NullParameter();

            string res = OAMessageBuilder.ReceiveDirectOver(p.syscode, p.flowid, p.nodename, p.receiver);
            return commonResult.Success(res);
        }
        /// <summary>
        /// 撤销某人待办
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public commonResult Cancel(OAMessageVM p)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.syscode) || string.IsNullOrWhiteSpace(p.flowid) || string.IsNullOrWhiteSpace(p.receiver))
                return commonResult.NullParameter();

            string res = OAMessageBuilder.Cancel(p.syscode, p.flowid, p.receiver);
            return commonResult.Success(res);
        }
        /// <summary>
        /// 撤销整个流程
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public commonResult CancelProcess(OAMessageVM p)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.syscode) || string.IsNullOrWhiteSpace(p.flowid))
                return commonResult.NullParameter();

            string res = OAMessageBuilder.CancelProcess(p.syscode, p.flowid);
            return commonResult.Success(res);
        }
    }
}
