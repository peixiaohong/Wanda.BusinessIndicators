using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plugin.OAMessage
{
    public class QueryCondition
    {
        public string syscode { get; set; }
        public string flowid { get; set; }
        public string nodename { get; set; }
        public string receiver { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public int pagesize { get; set; }
        public int pageindex { get; set; }

        public string BuildWhere()
        {
            string where = string.Empty;
            //if(!string.IsNullOrWhiteSpace(syscode))
            //{
            //    where +=string.Format(" and syscode='{0}'",syscode);
            //}
            if (!string.IsNullOrWhiteSpace(flowid))
            {
                where += string.Format(" and REQUESTID='{0}'", flowid);
            }
            if (!string.IsNullOrWhiteSpace(nodename))
            {
                where += string.Format(" and CURRENTNODENAME='{0}'", nodename);
            }
            if (!string.IsNullOrWhiteSpace(receiver))
            {
                where += string.Format(" and JSRLOGINID='{0}'", receiver);
            }
            if (startdate!=null)
            {
                where += string.Format(" and RECEIVE>='{0}'", startdate);
            }
            if (enddate != null)
            {
                where += string.Format(" and RECEIVE<='{0}'", enddate);
            }
            if(pagesize>0&& pageindex>0)
            {
                int start = (pageindex - 1) * pagesize;
                int end = pageindex * pagesize;
                where += string.Format(" and rownumb>{0} and rownumb<={1}", start, end);
            }
            return where;
        }
    }
}