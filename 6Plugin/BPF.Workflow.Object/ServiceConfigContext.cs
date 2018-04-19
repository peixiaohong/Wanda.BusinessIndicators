using System;
using System.Collections.Generic;
using System.Text;

namespace Wanda.Workflow.Object
{
    public class ServiceConfigContext : BaseServeContext
    {
        public Dictionary<string, string> Configs
        {
            get { return _Configs; }
            set { _Configs = value; }
        }private Dictionary<string, string> _Configs = null;
    }
}
