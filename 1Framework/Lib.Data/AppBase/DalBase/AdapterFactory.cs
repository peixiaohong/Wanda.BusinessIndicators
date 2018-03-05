using Lib.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Data.AppBase
{
    #region AdapterFactory
    public static class AdapterFactory
    {
        private static Dictionary<Type, object> _adapterDict = new Dictionary<Type, object>();

        /// <summary>
        /// 根据Model的类型找到对应的Adapter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T1 GetAdapter<T1>()
        {

            Type t1Type = typeof(T1);

            if (_adapterDict.ContainsKey(t1Type))
            {
                return (T1)_adapterDict[t1Type];
            }

            T1 result = (T1)CreateAdapter(t1Type);
            _adapterDict[t1Type] = result;
            return result;

        }


        private static object CreateAdapter(Type adapterType)
        {
            string typeFullName = adapterType.FullName;
            object result = null;
            if (!DataAdapterCache.Instance.TryGetValue(typeFullName, out result))
            {
                result = Activator.CreateInstance(adapterType, true); //TypeCreator.CreateInstance();
                DataAdapterCache.Instance.Add(typeFullName, result);
            }

            return result;

        }


    }

    public class AdapterNotFoundException : ApplicationException { }

    public class DataAdapterCache : CacheQueue<string, object>
    {
        public static readonly DataAdapterCache Instance = CacheManager.GetInstance<DataAdapterCache>();

        private DataAdapterCache()
        {
        }
    }
    #endregion
}
