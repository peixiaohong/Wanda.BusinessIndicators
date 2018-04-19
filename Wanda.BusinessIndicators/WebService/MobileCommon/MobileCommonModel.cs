using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPF.Workflow.Object;

namespace LJTH.BusinessIndicators.Web.MobileCommon
{
    /// <summary>
    /// 返回结构
    /// </summary>
    public struct MobileResultModel
    {
        public string status;
        public message messageResult;

    }

    /// <summary>
    /// 获取流程信息参数据
    /// </summary>
    public struct GetProcessCodeParam
    {
        public string requestId;
        public string sysPtpurl;
    }

    public struct GetProcessInfoParam
    {
        public string processCode;
        public string requestId;
        public string userLoginId;
        public string userId;
        //二级页面需要的信息
        public string SubPageName;
        public string PlanID;
        public string CategoryID;
        public string Approve;
    }
    /// <summary>
    /// 发送流程审批数据参数
    /// </summary>
    public struct SumbitProcessParam
    {
        /// <summary>
        /// 流程类型编码
        /// </summary>
        public string processCode;
        /// <summary>
        /// 业务流程待办ID
        /// </summary>
        public string requestId;
        /// <summary>
        /// 当前操作用户登录ID（RTX账号）
        /// </summary>
        public string userLoginId;
        /// <summary>
        /// 当前操作用户ID
        /// </summary>
        public string userId;
        /// <summary>
        /// 审批意见输入内容
        /// </summary>
        public string remark;
        /// <summary>
        /// 审批操作类型
        /// </summary>
        public string action;
        /// <summary>
        /// 审批操作需要选择的用户ID列表（当需要多选用户时才会发送此参数，其他审批操作无此参数）【可选】
        /// </summary>
        public string uids;
        /// <summary>
        /// 统一工作流平台需要4个参数【可选】
        /// </summary>
        public string ButtonType;
        /// <summary>
        /// 统一工作流平台需要4个参数【可选】
        /// </summary>
        public string ButtonName;


        public string buttonText;
        /// <summary>
        /// 统一工作流平台需要4个参数【可选】
        /// </summary>
        public string ButtonMethodName;
        /// <summary>
        /// 统一工作流平台需要4个参数【可选】
        /// </summary>
        public string ButtonDisplayName;

        #region 为加签功能所加
        /// <summary>
        /// 加签类型
        /// </summary>
        public string addType;
        /// <summary>
        /// 加签名称
        /// </summary>
        public string addNodename;
        /// <summary>
        /// 加签节点ID
        /// </summary>
        public string addNodeId;
        /// <summary>
        /// 
        /// </summary>
        public string addAprovaltype;

        #endregion
    }
    /// <summary>
    /// 搜索用户参数
    /// </summary>
    public struct SearchUserParam
    {
        /// <summary>
        /// 搜索的用户姓名
        /// </summary>
        public string username;
        /// <summary>
        /// 当前页码
        /// </summary>
        public string pageNo;
        /// <summary>
        /// 每页数据条数
        /// </summary>
        public string pageSize;
        /// <summary>
        /// 流程类型编码
        /// </summary>
        public string processCode;
        /// <summary>
        /// 业务流程待办ID
        /// </summary>
        public string requestId;
        /// <summary>
        /// 当前操作用户登录ID（RTX账号）
        /// </summary>
        public string userLoginId;
        /// <summary>
        /// 当前操作用户ID
        /// </summary>
        public string userId;

    }

    public struct SearchUserMessage
    {
        public string status;
        public string message;
        public int totalCount;
        public List<User> rows;
    }
    public struct User
    {

        public string userId;
        public string userName;
        public string loginId { get; set; }
        public string department { get; set; }
    }

    /// <summary>
    /// 服务用到的常量
    /// </summary>
    public class ConstantWS
    {
        public const string success = "success";
        public const string error = "error";
        public const string successText = "成功";
        public const string errorText = "失败";
        /// <summary>
        /// 流程数据组类型——无表头
        /// </summary>
        public const string flowData_group_noHeader = "kv_table";
        /// <summary>
        /// 流程数据组类型——有表头多列表格
        /// </summary>
        public const string flowData_group_haveHeader = "cap_table";
        /// <summary>
        /// 流程数据组类型——统一平台工作流
        /// </summary>
        public const string flowData_group_workFlow = "workflow_approve";
    }

    public struct message
    {
        public string requestId;
        public string processCode;
        public string ErrMessage;
    }
    /// <summary>
    /// 错误信息
    /// </summary>
    public struct ErrMessage
    {
        public string status;
        public string message;
    }
    /// <summary>
    /// 获取流程CODE返回信息
    /// </summary>
    public struct GetProcessCodeMessage
    {
        public string status;
        public string message;
        public string requestId;
        public string processCode;
    }

    #region 展示流程数据用到的结构
    /// <summary>
    /// 流程页面展示数据
    /// </summary>
    public struct FlowPageShowData
    {
        public string mainTitle;
        public string requestId;
        public string processCode;
        public string status;
        public string message;
        public string remarkisshow;
        public List<Groups> groups;
    }
    public class Groups
    {
        public string type;


        public string subTitle;
        public List<Rows> rows;
        public List<Titles> titles;
        ///// <summary>
        ///// type:"approve"专用，不用解析
        ///// </summary>
        //public List<Rows> paramList;
        /// <summary>
        /// type:workflow_approve
        /// 统一工作流平台专用
        /// </summary>
        public string approvesubTitle;
        public string logsubTitle;
        public string AppCode;
        public string AppID;
        public string BusinessID;
        public UserInfo CurrentUser;
        public string WFToken;
        public Process ProcessInstance;
        public Dictionary<string, Node> NodeInstanceList;
        public List<ProcessLog> ProcessLogList { get; set; }

        public Dictionary<string, Node> CcNodeInstanceList;
        public string CurrentUserNodeID;
        public SceneSetting CurrentUserSceneSetting;
        public bool CurrentUserHasTodoTask;
        public bool CurrentUserTodoTaskIsRead;
        public Dictionary<string, List<ActivityProperty>> CurrentUserActivityPropertiesList;
        public Dictionary<string, string> ExtensionInfos;
        public string StatusCode;
        public string StatusMessage;
        public string LastException;




    }
    public struct Rows
    {
        public string key;
        public string content;
        public string color;
        public string type;
        /// <summary>
        /// 列的样式
        /// </summary>
        public string td_style;

        public List<MFiles> files;
        public List<Links> links;
        public List<Cells> cells;
        public List<Buttons> buttonList;
    }
    public struct MFiles
    {
        public string fileName;
        public string fileSize;
        public string fileId;
        public string downloadUrl;

    }
    public struct Links
    {
        public string linkName;
        public string subPageType;
        public LinkUrlParam linkUrlParam;
    }
    public struct LinkUrlParam
    {
        public string SubPageName;
        public string PlanID;
        public string CategoryID;
        public string Approve;
    }
    public struct Titles
    {
        public string content;
    }
    public struct Cells
    {
        public string content;
        public string color;
        public string type;
        public string rtxId;
        /// <summary>
        /// 列的样式
        /// </summary>
        public string td_style;

        public List<MFiles> files;
        public List<Links> links;
    }
    public struct Buttons
    {
        public string buttonText;
        public string buttonAction;
        public string buttonType;
    }

    public struct ButtonCN
    {
        public string CN;
    }
    #endregion

}