﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Engine;
using LJTH.BusinessIndicators.Model;

namespace LJTH.BusinessIndicators.Engine
{
    [Serializable]
    public class SystemDownExcelEngine
    {
        //public SystemDownExcelEngine()
        //{
        //    //注册
        //    AddHandle("*", new DefaultSystem());
        //    List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ISystem");
        //    //注册扩展服务
        //    foreach (InterfaceInstance info in interfaces)
        //    {
        //        //根据InterfaceInstanceName，实现不同的处理方法
        //        ISystem _interface = SystemBuilder.Instance.DoBuild(info.Reference);
        //        if (_interface != null)
        //            AddHandle(info.InterfaceInstanceName, _interface);
        //    }
        //}

        ///// <summary>
        ///// 引擎实例
        ///// </summary>	
        //public static SystemDownExcelEngine SystemDownExcelEngineService
        //{
        //    get
        //    {
        //        return _SystemDownExcelEngineService;
        //    }
        //}private static SystemDownExcelEngine _SystemDownExcelEngineService = new SystemDownExcelEngine();

        //private Dictionary<string, ISystem> InterfaceInstanceList = new Dictionary<string, ISystem>();

        //private ISystem DefaultHandle
        //{
        //    get
        //    {
        //        return InterfaceInstanceList["*"];
        //    }
        //}

        //protected void AddHandle(string InstanceName, ISystem interfaceInstance)
        //{
        //    if (!InterfaceInstanceList.ContainsKey(InstanceName))
        //    {
        //        InterfaceInstanceList.Add(InstanceName, interfaceInstance);
        //    }
        //    else
        //    {
        //        InterfaceInstanceList[InstanceName] = interfaceInstance;
        //    }
        //}

        //protected ISystem this[string key]
        //{
        //    get
        //    {
        //        if (InterfaceInstanceList.ContainsKey(key))
        //        {
        //            return InterfaceInstanceList[key];
        //        }
        //        else
        //        {
        //            List<InterfaceInstance> interfaces = InterfaceInstanceOperator.Instance.LoadListByName("ISystem");
        //            InterfaceInstance newInstance = interfaces.Find(I => I.InterfaceInstanceName == key);
        //            if (newInstance != null)
        //            {
        //                ISystem _interface = SystemBuilder.Instance.DoBuild(newInstance.Reference);
        //                if (_interface != null)
        //                {
        //                    AddHandle(newInstance.InterfaceInstanceName, _interface);
        //                    return _interface;
        //                }
        //            }
        //        }
        //        return DefaultHandle;
        //    }
        //}

        //public object GetSystem(object ViewModel, Guid _SystemID, List<Guid> TargetIDs)
        //{

        //    string InterfaceName = "*";
        //    C_System sys = StaticResource.Instance[_SystemID];
        //    if (sys != null && sys.Configuration.Element("Interfaces") != null)
        //    {
        //        if (sys.Configuration.Element("Interfaces").Elements("ISystem") != null && sys.Configuration.Element("Interfaces").Elements("ISystem").ToList().Count > 0)
        //        {
        //            foreach (XElement e in sys.Configuration.Element("Interfaces").Elements("ISystem").ToList())
        //            {
        //                InterfaceName = e.Attribute("InterfaceName").Value.Trim();
        //            }
        //        }
        //    }
        //    return this[InterfaceName].GetSystem(ViewModel, _SystemID, TargetIDs);
        //}
    }
}
