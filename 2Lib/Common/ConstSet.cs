using Lib.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJTH.BusinessIndicators.Common
{
    public static class ConstSet
    {
        public const string SESSION_CURRENT_USER = "SESSION_CURRENT_USER";
        public const string URL_LOGIN = "/Login.aspx";
        public const string DEFAULT_URL = "/BusinessReport/TargetRpt.aspx";
        public const string TemplateTypes = "TemplateTypes";
        public const string EnumDictionary = "EnumDictionary";
        public const string WorkFlowTypes = "WorkFlowTypes";
        public const int AdminRoleID = 1;
        public const int Template_WorkFlow_CongID = 1;

        static string TemplateLaunch_ProcessCode = null;
        static public string Template_WorkFlow_TemplateLaunch_ProcessCode
        {
            get
            {
                if (string.IsNullOrEmpty(TemplateLaunch_ProcessCode))
                {
                    TemplateLaunch_ProcessCode = AppSettingConfig.GetSetting("TemplateLaunch_ProcessCode", null);
                }
                return TemplateLaunch_ProcessCode;
            }
        }

        static string TemplateSendOut_ProcessCode = null;
        static public string Template_WorkFlow_TemplateSendOut_ProcessCode
        {
            get
            {
                if (string.IsNullOrEmpty(TemplateSendOut_ProcessCode))
                {
                    TemplateSendOut_ProcessCode = AppSettingConfig.GetSetting("TemplateSendOut_ProcessCode", null);
                }
                return TemplateSendOut_ProcessCode;
            }
        }

        static string TaskSendOut_ProcessCode = null;
        static public string Template_WorkFlow_TaskSendOut_ProcessCode
        {
            get
            {
                if (string.IsNullOrEmpty(TaskSendOut_ProcessCode))
                {
                    TaskSendOut_ProcessCode = AppSettingConfig.GetSetting("TaskSendOut_ProcessCode", null);
                }
                return TaskSendOut_ProcessCode;
            }
        }

        static short tpi = -1;
        static public short Template_WorkFlow_TemplateSendOut_ProcessType
        {
            get
            {
                if (tpi < 0)
                {
                    if (!string.IsNullOrEmpty(Template_WorkFlow_TemplateSendOut_ProcessCode))
                    {
                        string t = Template_WorkFlow_TemplateSendOut_ProcessCode.ToUpper().Replace("BP", "");
                        short.TryParse(t, out tpi);
                    }
                }
                return tpi;
            }
        }

        static short tti = -1;
        static public short Template_WorkFlow_TaskSendOut_ProcessType
        {
            get
            {
                if (tti < 0)
                {
                    if (!string.IsNullOrEmpty(Template_WorkFlow_TaskSendOut_ProcessCode))
                    {
                        string t = Template_WorkFlow_TaskSendOut_ProcessCode.ToUpper().Replace("BP", "");
                        short.TryParse(t, out tti);
                    }
                }
                return tti;
            }
        }

        static public string Template_WorkFlow_TaskLaunch_ProcessCode
        {
            get
            {

                throw new NotImplementedException("使用ConglomerateOperator.Instance.GetProcessCode(kpiunitId)方法替换.");
            }
        }

        public const int ID_NOEXIST = -1; //不存在的ID
    }
}
