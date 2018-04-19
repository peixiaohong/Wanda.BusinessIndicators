using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPF.OAMQMessages.Entities
{

    [Serializable]
    public class OAMQMessage
    {
        public const string SourceTable = "OAMQMessages";
        private string _MessageId = string.Empty;
        private string _Sender = string.Empty;
        private DateTime _Sendertime;
        private string _Flowtype = string.Empty;
        private string _FlowID;
        private string _Title = string.Empty;
        private string _Nodename = string.Empty;
        private string _PtpUrl = string.Empty;
        private string _Userid = string.Empty;
        private string _Creator = string.Empty;
        private DateTime _Createtime;
        private DateTime _Operatetime;
        private int _Flowmess;
        private int _Viewtype;
        private int _Status;
        private int _ErrorCount;
        private DateTime _MessageCreateTime;
        private int _AllowMobile;
        private string _SenderCode = string.Empty;
        private string _MessageRemark = string.Empty;
        private string _ProcessID = string.Empty;

        public string MessageId
        {
            get
            {
                return this._MessageId;
            }
            set
            {
                this._MessageId = value;
            }
        }
        public string Sender
        {
            get
            {
                return this._Sender;
            }
            set
            {
                this._Sender = value;
            }
        }
        public DateTime Sendertime
        {
            get
            {
                return this._Sendertime;
            }
            set
            {
                this._Sendertime = value;
            }
        }
        public string Flowtype
        {
            get
            {
                return this._Flowtype;
            }
            set
            {
                this._Flowtype = value;
            }
        }
        public string FlowID
        {
            get
            {
                return this._FlowID;
            }
            set
            {
                this._FlowID = value;
            }
        }
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }
        public string Nodename
        {
            get
            {
                return this._Nodename;
            }
            set
            {
                this._Nodename = value;
            }
        }
        public string PtpUrl
        {
            get
            {
                return this._PtpUrl;
            }
            set
            {
                this._PtpUrl = value;
            }
        }
        public string Userid
        {
            get
            {
                return this._Userid;
            }
            set
            {
                this._Userid = value;
            }
        }
        public string Creator
        {
            get
            {
                return this._Creator;
            }
            set
            {
                this._Creator = value;
            }
        }
        public DateTime Createtime
        {
            get
            {
                return this._Createtime;
            }
            set
            {
                this._Createtime = value;
            }
        }
        public DateTime Operatetime
        {
            get
            {
                return this._Operatetime;
            }
            set
            {
                this._Operatetime = value;
            }
        }
        public int Flowmess
        {
            get
            {
                return this._Flowmess;
            }
            set
            {
                this._Flowmess = value;
            }
        }
        public int Viewtype
        {
            get
            {
                return this._Viewtype;
            }
            set
            {
                this._Viewtype = value;
            }
        }
        public int Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
            }
        }
        public int ErrorCount
        {
            get
            {
                return this._ErrorCount;
            }
            set
            {
                this._ErrorCount = value;
            }
        }
        public DateTime MessageCreateTime
        {
            get
            {
                return this._MessageCreateTime;
            }
            set
            {
                this._MessageCreateTime = value;
            }
        }

        public int AllowMobile
        {
            get
            {
                return this._AllowMobile;
            }
            set
            {
                this._AllowMobile = value;
            }
        }
        public string SenderCode
        {
            get
            {
                return this._SenderCode;
            }
            set
            {
                this._SenderCode = value;
            }
        }

        public string ProcessID
        {
            get
            {
                return this._ProcessID;
            }
            set
            {
                this._ProcessID = value;
            }
        }

        public string MessageRemark
        {
            get
            {
                return this._MessageRemark;
            }
            set
            {
                this._MessageRemark = value;
            }
        }
    }
}
