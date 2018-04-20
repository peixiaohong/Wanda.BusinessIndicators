using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPF.OAMQServices
{
    /// <summary>
    /// This object represents the properties and methods of a TemplateAttachment.
    /// </summary>
    public class OAMessageEntity 
    {
        /// <summary>
        /// ���ؽ��
        /// </summary>
        public string ResultData { get; set; }
        /// <summary>
        /// ����ID
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// ���̱���
        /// </summary>
        public string FlowTitle { get; set; }
        /// <summary>
        /// ʹ�ù���������
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// ��ǰ�û��ڵ�����
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// ���Զ�������ַ
        /// </summary>
        public string PCUrl { get; set; }
        /// <summary>
        /// �ֻ���������ַ
        /// </summary>
        public string AppUrl { get; set; }
        /// <summary>
        /// ���������û�
        /// </summary>
        public string CreateFlowUser { get; set; }
        /// <summary>
        /// ��������ʱ��
        /// </summary>
        public DateTime CreateFlowTime { get; set; }
        /// <summary>
        /// ���������û�
        /// </summary>
        public string ReceiverFlowUser { get; set; }
        /// <summary>
        /// ��������ʱ��
        /// </summary>
        public DateTime ReceiverFlowTime { get; set; }
        /// <summary>
        /// ���̴���״̬ 0���� 2�Ѱ� 4���
        /// </summary>
        public int FlowType { get; set; }
        /// <summary>
        /// ���̲鿴״̬ 0δ�� 1�Ѷ�
        /// </summary>
        public int ViewType { get; set; }


    }
}

