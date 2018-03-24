

using System;
namespace LJTH.Lib.Data.AppBase
{
    /// <summary>
    /// 可用ID识别的对象
    /// </summary>
    public interface IIdentitfiable
    {
        Guid ID { get; set; }
    }

}
