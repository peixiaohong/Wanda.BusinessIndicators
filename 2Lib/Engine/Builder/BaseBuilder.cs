using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJTH.BusinessIndicators.Engine
{
    public class BaseBuilder<T>
    {
        private Dictionary<string, T> dict = new Dictionary<string, T>();

        public BaseBuilder()
        {
        }


        public T DoBuild(string typeString)
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
