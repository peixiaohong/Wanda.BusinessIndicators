using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LJTH.BusinessIndicators.Common
{
    public class AnonymousType
    {
        public AnonymousType(int value, string text, string fieldName)
        {
            this.Value = value;
            this.Text = text;
            this.FieldName = fieldName;
        }

        public int Value
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string FieldName
        {
            get;
            set;
        }
    }
}
