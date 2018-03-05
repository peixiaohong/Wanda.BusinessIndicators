using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wanda.Lib.LightWorkflow.Tools
{
    public static class StringHelper
    {
        public static string Join(IEnumerable<string> array, string separator, Func<string, string> replacer = null)
        {
            return Join<string>(array, separator, replacer);
        }

        public static string Join<T>(IEnumerable<T> array, string separator, Func<string, string> replacer = null)
        {
            if (array == null || array.Count() == 0) return string.Empty;
            if (string.IsNullOrEmpty(separator) && replacer == null) return string.Empty;

            string result = string.Empty;
            int lastIndex = array.Count() - 1;
            int i = 0;
            foreach (T item in array)
            {
                if (i != lastIndex)
                {
                    if (replacer == null)
                    {
                        result += item + separator;
                    }
                    else
                    {
                        result += replacer(Convert.ToString(item)) + separator;
                    }
                }
                else
                {
                    if (replacer == null)
                    {
                        result += item;
                    }
                    else
                    {
                        result += replacer(Convert.ToString(item));
                    }
                }
                ++i;
            }
            return result;
        }
    }
}

