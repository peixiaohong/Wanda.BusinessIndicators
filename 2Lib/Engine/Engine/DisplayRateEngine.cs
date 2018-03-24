using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Engine.Engine
{
     [Serializable]
    public class DisplayRateEngine
    {
         public DisplayRateEngine()
        {
            //注册
            AddHandle("*", new DefaultDisplayRate());
            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IGetDisplayRate");
            //注册扩展服务
            foreach (InterfaceInstance info in interfaces)
            {
                //根据InterfaceInstanceName，实现不同的处理方法
                IGetDisplayRate _interface = DisplayRateBuilder.Instance.DoBuild(info.Reference);
                if (_interface != null)
                    AddHandle(info.InterfaceInstanceName, _interface);
            }
        }

         /// <summary>
         /// 引擎实例
         /// </summary>	
         public static DisplayRateEngine DisplayRateService
         {
             get
             {
                 return _DisplayRateService;
             }
         }private static DisplayRateEngine _DisplayRateService = new DisplayRateEngine();

         private Dictionary<string, IGetDisplayRate> InterfaceInstanceList = new Dictionary<string, IGetDisplayRate>();

         private IGetDisplayRate DefaultHandle
         {
             get
             {
                 return InterfaceInstanceList["*"];
             }
         }

         protected void AddHandle(string InstanceName, IGetDisplayRate interfaceInstance)
         {
             if (!InterfaceInstanceList.ContainsKey(InstanceName))
             {
                 InterfaceInstanceList.Add(InstanceName, interfaceInstance);
             }
             else
             {
                 InterfaceInstanceList[InstanceName] = interfaceInstance;
             }
         }

         protected IGetDisplayRate this[string key]
         {
             get
             {
                 if (InterfaceInstanceList.ContainsKey(key))
                 {
                     return InterfaceInstanceList[key];
                 }
                 else
                 {
                     List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("IGetDisplayRate");
                     InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
                     if (newInstance != null)
                     {
                         IGetDisplayRate _interface = DisplayRateBuilder.Instance.DoBuild(newInstance.Reference);
                         if (_interface != null)
                         {
                             AddHandle(newInstance.InterfaceInstanceName, _interface);
                             return _interface;
                         }
                     }
                 }
                 return DefaultHandle;
             }
         }


    }
}
