using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.LightWorkflow.Interface;

namespace Wanda.Lib.LightWorkflow.Builder
{
    public class DataExchangeHandleBuilder : BaseBuilder<IDataExchangeHandle>
    {
        private static DataExchangeHandleBuilder _Instance = new DataExchangeHandleBuilder("*");

        public DataExchangeHandleBuilder(string defaultDefine)
        {
            base.DefaultDefine = defaultDefine;
        }

        public static DataExchangeHandleBuilder Instance
        {
            get
            {
                return _Instance;
            }
        }
    }
}
