

using System;
namespace Wanda.Lib.Data.AppBase
{
    /// <summary>
    /// 可用ID识别的对象
    /// </summary>
    public interface IIdentitfiable
    {
        Guid ID { get; set; }
    }

}
