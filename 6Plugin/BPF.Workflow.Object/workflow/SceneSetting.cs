using System;
using System.Collections.Generic;
using System.Text;
namespace BPF.Workflow.Object
{
    /// <summary>
    /// 场景设置
    /// 用户打开表单后，工作流根据当前用户寻找节点，并计算对应的场景设置
    /// </summary>
    public class SceneSetting
    {
        /// <summary>
        /// 是否显示流程导航条
        /// </summary>
        public bool ShowNavigationBar
        {
            get { return _ShowNavigationBar; }
            set { _ShowNavigationBar = value; }
        }private bool _ShowNavigationBar = false;
        /// <summary>
        /// 是否显示抄送导航条
        /// </summary>
        public bool ShowCCBar
        {
            get { return _ShowCCBar; }
            set { _ShowCCBar = value; }
        }private bool _ShowCCBar = false;
        /// <summary>
        /// 是否允许用户添加抄送用户（显示并允许点击添加抄送按钮）
        /// </summary>
        public bool AllowNewCC
        {
            get { return _AllowNewCC; }
            set { _AllowNewCC = value; }
        }private bool _AllowNewCC = false; 
        /// <summary>
        /// 是否显示审批意见文本框
        /// </summary>
        public bool ShowApprovalTextArea
        {
            get { return _ShowApprovalTextArea; }
            set { _ShowApprovalTextArea = value; }
        }private bool _ShowApprovalTextArea = false;
        /// <summary>
        /// 是否显示操作按钮
        /// </summary>
        public bool ShowButtonBar
        {
            get { return _ShowButtonBar; }
            set { _ShowButtonBar = value; }
        }private bool _ShowButtonBar = false;
        /// <summary>
        /// 可用按钮列表
        /// </summary>
        public List<ActionButton> ActionButtonList
        {
            get { return _ActionButtonList; }
            set { _ActionButtonList = value; }
        }private List<ActionButton> _ActionButtonList = null;
        /// <summary>
        /// 是否显示审批日志
        /// </summary>
        public bool ShowApprovalLog
        {
            get { return _ShowApprovalLog; }
            set { _ShowApprovalLog = value; }
        }private bool _ShowApprovalLog = false;
    }
}
