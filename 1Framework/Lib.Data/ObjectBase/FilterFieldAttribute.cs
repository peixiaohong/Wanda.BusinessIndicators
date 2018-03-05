using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterFieldAttribute : Attribute
    {
        private FieldAndOperator F_A_O = new FieldAndOperator();
        public FieldAndOperator Fao { get { return F_A_O; } }
        public string TableName { get { return F_A_O.TableName; } }
        public string FieldName { get { return F_A_O.FieldName; } }
        //public string[] FieldNames { get { return F_A_O.FieldNames; } }
        public string Operator { get { return F_A_O.Operator; } }
        //public bool AllowDefault { get { return F_A_O.AllowDefault; } }
        public object DefaultV { get; set; } // 当filter值为DefaultV时， 则认为不需要过滤该条件

        public FilterFieldAttribute(string fieldName)
        {
            this.F_A_O.FieldName = fieldName;
            // this.F_A_O.FieldNames = new string[] { fieldName };
            this.F_A_O.Operator = "=";
            this.F_A_O.DefaultV = DefaultV;
        }

        public FilterFieldAttribute(string fieldName, string operatorString)
        {
            this.F_A_O.FieldName = fieldName;
            //  this.F_A_O.FieldNams = new string[] { fieldName };
            this.F_A_O.Operator = operatorString;
            this.F_A_O.DefaultV = DefaultV;
            //   this.F_A_O.AllowDefault = false;
        }

        //public FilterFieldAttribute(string[] fieldNames, string operatorString)
        //{
        //    this.F_A_O.FieldNames = fieldNames;
        //    this.F_A_O.Operator = operatorString;
        //    this.F_A_O.DefaultV = DefaultV;
        //    //   this.F_A_O.AllowDefault = false;
        //}


    }

    public class FieldAndOperator
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public object DefaultV { get; set; }
        // public string[] FieldNames { get; set; }

        //  public bool AllowDefault { get; set; }
        public string TableFieldName
        {
            get
            {
                if (string.IsNullOrEmpty(TableName))
                {
                    return FieldName;
                }
                return string.Format("{0}.{1}", TableName, FieldName);
            }
        }

    }

}
