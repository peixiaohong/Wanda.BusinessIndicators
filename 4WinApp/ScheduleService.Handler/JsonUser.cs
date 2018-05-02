using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BPF.Workflow.Client;
using BPF.Workflow.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScheduleService.Handler
{
    public static class JsonUser
    {
        private static string _userStr = string.Empty;
        private static string fileName = "User.json";
        private static string UserStr
        {
            get
            {
                if (string.IsNullOrEmpty(_userStr))
                {
                    string fullpath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                    if (File.Exists(fullpath))
                    {
                        _userStr = File.ReadAllText(fullpath);
                    }
                    else
                        throw new Exception("缺少审批人配置文件");
                }
                return _userStr;
            }
        }
        public static Dictionary<string, List<UserInfo>> GetDynamicRoleUserList(string flowCode)
        {
            var dic = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<UserInfo>>>>(UserStr);
            if (dic != null && dic.Keys.Contains(flowCode))
            {
                return dic[flowCode];
            }
            else
                return null;
        }

    }
}
