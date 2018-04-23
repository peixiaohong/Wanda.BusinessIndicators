using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPF.OAMQMessages.Entities;

namespace BPF.OAMQMessages.DAL
{
    interface IOAMQMessage
    {
        List<OAMQMessage> LoadList(int count);

        int Update(OAMQMessage message);
    }
}
