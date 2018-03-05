using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lib.Core
{
    public static class RuntimeInfo
    {
        public static string GetMethodInfo()
        {
            var method = new StackFrame(1).GetMethod(); // 这里忽略1层堆栈，也就忽略了当前方法GetMethodName，这样拿到的就正好是外部调用GetMethodName的方法信息
            return null;
            //var property = (
            //          from p in method.DeclaringType.GetProperties(
            //                   BindingFlags.Instance |
            //                   BindingFlags.Static |
            //                   BindingFlags.Public |
            //                   BindingFlags.NonPublic)
            //          where p.GetGetMethod(true) == method || p.GetSetMethod(true) == method
            //          select p).FirstOrDefault();
            //return property == null ? method.Name : property.Name;
        }
    }
}
