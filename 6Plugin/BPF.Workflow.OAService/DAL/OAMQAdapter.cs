using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPF.OAMQMessages.Entities;

namespace BPF.OAMQMessages.DAL
{
    public class OAMQAdapter : DALBase, IOAMQMessage
    {
        private static OAMQAdapter _instance = null;
        public static OAMQAdapter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OAMQAdapter();
                }
                return _instance;
            }
        }

        private IOAMQMessage _dataSource = null;
        private IOAMQMessage DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    DataBaseTypes dbType = this.GetDataBaseType();
                    if (dbType == DataBaseTypes.Oracle)
                    {
                        _dataSource = OAMQMessagesOracle.Instance;
                        OAMQMessagesOracle.Instance.DbConnection = this.DbConnection;
                    }
                    else if (dbType == DataBaseTypes.MSSQLServer)
                    {
                        _dataSource = OAMQMessagesSQLServer.Instance;
                        OAMQMessagesSQLServer.Instance.DbConnection = this.DbConnection;
                    }
                }
                return _dataSource;
            }
        }

        public List<OAMQMessage> LoadList(int count)
        {
            return this.DataSource.LoadList(count);
        }

        public int Update(OAMQMessage message)
        {
            return this.DataSource.Update(message);
        }
    }
}
