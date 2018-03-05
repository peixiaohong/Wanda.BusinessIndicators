using System;
using System.Collections.Generic;
using Lib.Web.Json;

namespace Lib.Web.Security
{
    public class LoginUser : ILoginUser
    {
        #region Construct

        public LoginUser(string userID, string loginName, string displayName)
            : this(string.Empty, userID, loginName, displayName, string.Empty, string.Empty)
        { }

        public LoginUser(string applicationID, string userID, string loginName, string displayName, string originalUserID, string domain)
        {
            this.applidationID = applicationID;
            this.userID = userID;
            this.loginName = loginName;
            this.displayName = displayName;
            this.originalUserID = originalUserID;
            this.domain = domain;
        }

        #endregion

        #region Private Field

        private string applidationID;
        private string userID;
        private string loginName;
        private string displayName;
        private string originalUserID;
        private string domain;

        private Dictionary<string, object> properities = null;

        #endregion Private Field

        #region Public Field

        public string ApplicationID
        {
            get
            {
                return this.applidationID;
            }
            set
            {
                this.applidationID = value;
            }
        }

        public string UserID
        {
            get
            {
                return this.userID;
            }
            set
            {
                this.userID = value;
            }
        }

        public string LoginName
        {
            get
            {
                return this.loginName;
            }
            set
            {
                this.loginName = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        public string OriginalUserID
        {
            get
            {
                return this.originalUserID;
            }
            set
            {
                this.originalUserID = value;
            }
        }

        public string Domain
        {
            get
            {
                return this.domain;
            }
            set
            {
                this.domain = value;
            }
        }



        public Dictionary<string, object> Properties
        {
            get
            {
                if (this.properities == null)
                    this.properities = new Dictionary<string, object>();
                return this.properities;
            }
        }

        #endregion Public Field

        #region Public Method

        public string Serialize()
        {
            return JsonHelper.Serialize(this);
        }

        public ILoginUser Deserialize(string dataString)
        {
            return JsonHelper.Deserialize<LoginUser>(dataString);
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public int Persist()
        {
            return 0;
        }

        #endregion Public Method
    }
}
