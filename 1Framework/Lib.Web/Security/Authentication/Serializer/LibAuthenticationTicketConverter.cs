using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Lib.Core;

namespace Lib.Web.Security
{
    public class LibAuthenticationTicketConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            ILibAuthenticationTicket obj = CreateObject(dictionary, type, serializer);

            return obj;
        }

        protected virtual ILibAuthenticationTicket CreateObject(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new LibAuthenticationTicket();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(LibAuthenticationTicket));
                return types;
            }
        }
    }
}
