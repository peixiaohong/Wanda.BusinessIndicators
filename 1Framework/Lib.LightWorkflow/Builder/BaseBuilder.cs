using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanda.Lib.LightWorkflow.Configs;

namespace Wanda.Lib.LightWorkflow.Builder
{
    public class BaseBuilder<T>
    {
        private Dictionary<string, T> dict = new Dictionary<string, T>();

        public BaseBuilder()
        {
        }

        protected string DefaultDefine
        {
            get
            {
                return _DefaultDefine;
            }
            set
            {
                _DefaultDefine = value;
            }
        }private string _DefaultDefine = "";

        /// <summary>
        /// 构造默认的分词分析器
        /// </summary>
        /// <returns></returns>
        public T Build()
        {
            return Build(DefaultDefine);
        }

        public T Build(string defineStr)
        {
            string typeString = LightWorkflowSettings.Instance.DataExchangeProviderInfos[defineStr].Value;
            typeString = typeString.Trim();
            if (typeString == "")
                return default(T);
            else
                return DoBuild(typeString);
        }

        private T DoBuild(string typeString)
        {
            T t = default(T);
            lock (dict)
            {
                if (dict.ContainsKey(typeString))
                {
                    t = dict[typeString];
                }
                else
                {
                    Type type = Type.GetType(typeString, true);
                    object obj = Activator.CreateInstance(type);
                    if (obj is T)
                    {
                        t = (T)obj;
                        dict.Add(typeString, t);
                    }
                }
            }
            return t;
        }
    }
}
