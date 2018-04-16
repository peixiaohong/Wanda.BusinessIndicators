using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Plugin.OAMessage
{
    /// <summary>
    /// This object represents the properties and methods of a TemplateAttachment.
    /// </summary>
    public class OAMessageEntity
    {
        #region
        ///// <summary>
        ///// ���ؽ��
        ///// </summary>
        //public string ResultData { get; set; }
        ///// <summary>
        ///// ����ID
        ///// </summary>
        //public string FlowID { get; set; }
        ///// <summary>
        ///// ���̱���
        ///// </summary>
        //public string FlowTitle { get; set; }
        ///// <summary>
        ///// ʹ�ù���������
        ///// </summary>
        //public string WorkflowName { get; set; }
        ///// <summary>
        ///// ��ǰ�û��ڵ�����
        ///// </summary>
        //public string NodeName { get; set; }
        ///// <summary>
        ///// ���Զ�������ַ
        ///// </summary>
        //public string PCUrl { get; set; }
        ///// <summary>
        ///// �ֻ���������ַ
        ///// </summary>
        //public string AppUrl { get; set; }
        ///// <summary>
        ///// ���������û�
        ///// </summary>
        //public string CreateFlowUser { get; set; }
        ///// <summary>
        ///// ��������ʱ��
        ///// </summary>
        //public DateTime CreateFlowTime { get; set; }
        ///// <summary>
        ///// ���������û�
        ///// </summary>
        //public string ReceiverFlowUser { get; set; }
        ///// <summary>
        ///// ��������ʱ��
        ///// </summary>
        //public DateTime ReceiverFlowTime { get; set; }
        ///// <summary>
        ///// ���̴���״̬ 0���� 2�Ѱ� 4���
        ///// </summary>
        //public int FlowType { get; set; }
        ///// <summary>
        ///// ���̲鿴״̬ 0δ�� 1�Ѷ�
        ///// </summary>
        //public int ViewType { get; set; }
        #endregion
            
        /// <summary>
        /// ������
        /// </summary>
        public string LOGINID { get; set; }
        /// <summary>
        /// ϵͳ����
        /// </summary>
        public string SYSSHORTNAME { get; set; }
        /// <summary>
        /// PC�����ӵ�ַ
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// flowId
        /// </summary>
        public string REQUESTID { get; set; }
        /// <summary>
        /// δ֪��ֵ��Ϊ��
        /// </summary>
        public string REQUESTMARK { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string CREATEDATE { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public string CREATETIME { get; set; }
        /// <summary>
        /// δ֪��ֵΪ���֣��²�Ϊ������id
        /// </summary>
        private int CREATER { get; set; }
        /// <summary>
        /// δ֪��ֵ��Ϊ0
        /// </summary>
        public int CREATERTYPE { get; set; }
        /// <summary>
        /// δ֪����ͬ���̴���ֵ��ͬ�����
        /// </summary>
        public int WORKFLOWID { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string REQUESTNAME { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string REQUESTNAMENEW { get; set; }
        /// <summary>
        /// δ֪���ֶξ�Ϊ��ֵ
        /// </summary>
        public string STATUS { get; set; }
        /// <summary>
        /// ����Ĭ��Ϊ-1
        /// </summary>
        public int REQUESTLEVEL { get; set; }
        /// <summary>
        /// ��ǰ���id��Ĭ��Ϊ-1
        /// </summary>
        public int CURRENTNODEID { get; set; }
        /// <summary>
        /// ��ǰ�ڵ�����
        /// </summary>
        public string CURRENTNODENAME { get; set; }
        /// <summary>
        /// ���̲鿴״̬��0��δ����1���Ѷ���
        /// </summary>
        public int VIEWTYPE { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string JSRLOGINID { get; set; }
        /// <summary>
        /// ����ʱ�䣨yyyy-MM-dd HH:mm:ss��
        /// </summary>
        public string RECEIVE { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string RECEIVEDATE { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public string RECEIVETIME { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public string OPERATE { get; set; }
        /// <summary>
        /// ���̴���״̬ 0�����죻2���Ѱ죻4�����
        /// </summary>
        public string ISREMARK { get; set; }
        /// <summary>
        /// δ֪��ֵΪ0
        /// </summary>
        public string NODEID { get; set; }
        /// <summary>
        /// δ֪��ֵΪ-1��
        /// </summary>
        public string AGENTORBYAGENTID { get; set; }
        /// <summary>
        /// δ֪��ֵΪ0
        /// </summary>
        public string AGENTTYPE { get; set; }
        /// <summary>
        /// δ֪��ֵΪ0
        /// </summary>
        public string ISPROCESSED { get; set; }
        /// <summary>
        /// δ֪��ֵΪ1
        /// </summary>
        public string SYSTYPE { get; set; }
        /// <summary>
        /// δ֪����ϵͳֵ��һ��
        /// </summary>
        public string WORKFLOWTYPE { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        public string WORKFLOWNAME { get; set; }
        /// <summary>
        /// ϵͳ����
        /// </summary>
        public string SYSCODE { get; set; }

    }
}

