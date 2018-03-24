using Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJTH.Lib.AuthCenter.Model;

namespace LJTH.Lib.AuthCenter.DAL
{
    class VUserDefaultUrlAdapter : AuthBaseCompositionAdapterT<VUserDefaultUrl>
    {
        public static VUserDefaultUrlAdapter Instance = new VUserDefaultUrlAdapter();

        internal PartlyCollection<VUserDefaultUrl> GetList(Guid userID)
        {
            WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
            where.AppendItem("userID", userID);
            return base.GetList(where, "Weight");
        }
    }
}
