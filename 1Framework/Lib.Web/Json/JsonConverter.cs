using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Lib.Web.Json
{
    public abstract class JsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
