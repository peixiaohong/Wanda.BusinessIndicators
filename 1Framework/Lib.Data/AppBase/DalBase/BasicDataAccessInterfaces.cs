using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace LJTH.Lib.Data.AppBase
{


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">必须有一个空的构造函数， 并且有主键ID</typeparam>
    public interface IGetModelByID<T> where T : IIdentitfiable, new()
    {
        T GetModelByID(Guid ID);
    }


    public interface IInsertable<T> where T : new()
    {
        int Insert(T data);
    }


    public interface IUpdatable<T> where T : IIdentitfiable, new()
    {
        int Update(T data);
    }


    public interface IRemovable<T> where T : IIdentitfiable, new()
    {
        int Remove(T data);
        //int RemoveBatched(IEnumerable<T> datas);

    }

    /// <summary>
    /// 执行与Remove相反的动作——恢复Removed的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRetrievable<T> where T : IIdentitfiable, new()
    {
        IList<T> RetrieveAll();
        T RetrieveByID(string ID);
    }

    /// <summary>
    /// 在设计时确保应用该接口的Adapter，在删除之前需要将关联的数据关系清除
    /// </summary>
    public interface IUsage
    {
        int UsageCount(Guid ID);
    }

    /// <summary>
    /// 可数字排序
    /// </summary>
    public interface INumbicOrderable
    {
        int OrderNumber { get; set; }
    }

    /// <summary>
    /// 基本对象接口
    /// </summary>
    /// <typeparam name="T">IIdentitfiable， 且有公开的无参数构造函数</typeparam>
    /// <remarks>
    /// 可删除， 可更新，可新增， 可根据ID获得对象， 
    /// </remarks>
    public interface IBasicDataAccess<T> : IRemovable<T>, IUpdatable<T>, IGetModelByID<T>, IInsertable<T>
           where T : IIdentitfiable, new()
    {
    }
}
