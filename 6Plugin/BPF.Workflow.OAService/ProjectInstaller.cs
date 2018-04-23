using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace BPF.OAMQServices
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Lvl.App.ServiceInstaller
    {
        public override string ServiceName
        {
            get
            {
                if (string.IsNullOrEmpty(base.ServiceName))
                {
                    return "BPF.OAMQServices";
                }
                return base.ServiceName;
            }
        }
    }
}
