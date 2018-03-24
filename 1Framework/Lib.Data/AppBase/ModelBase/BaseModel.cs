using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Data;



namespace LJTH.Lib.Data.AppBase
{
    /// <summary>
    /// C/B 类数据实体基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BaseModel : IIdentitfiable
    {
        public BaseModel()
        { }

        [ORFieldMapping("ID", PrimaryKey = true)]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.All, DefaultExpression = "NEWID()")]
        public virtual Guid ID { get; set; }


        [ORFieldMapping("CreateTime")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Insert | ClauseBindingFlags.Select | ClauseBindingFlags.Where,
            DefaultExpression = "getdate()")]
        public virtual DateTime CreateTime { get; set; }


        [ORFieldMapping("CreatorName")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Insert | ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
        public virtual string CreatorName { get; set; }

        [ORFieldMapping("ModifierName")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Insert | ClauseBindingFlags.Update | ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
        public virtual string ModifierName { get; set; }


        [ORFieldMapping("ModifyTime")]
        [SqlBehavior(BindingFlags = ClauseBindingFlags.Insert | ClauseBindingFlags.Update | ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
        public virtual DateTime ModifyTime { get; set; }

        /// <summary>
        /// 标示是否已删除（标记删除）
        /// </summary>
        [ORFieldMapping("IsDeleted")]
        public virtual bool IsDeleted { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }


            return obj.ToString() == this.ToString();

        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return ID.ToString();
        }
    }

    /// <summary>
    /// 带层级结构的 C/B 类数据实体基类
    /// </summary>
    public class BaseNodeModel : BaseModel
    {
        //Attribute 会被子类复用， 所以此处不能标NoMapping
        //[NoMapping] 
        public virtual int ParentID { get; set; }
    }



    /// <summary>
    /// 关联数据模型， 仅供Model使用， 不需要Public
    /// </summary>
    public abstract class BaseAssociationModel : IIdentitfiable
    {


        /// <summary>
        /// 标示是否已删除（标记删除）
        /// </summary>
        [ORFieldMapping("ISDELETED")]
        public virtual bool IsDeleted { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public virtual Guid ID { get; set; }

        /// <summary>
        /// 外键1
        /// </summary>
        [NoMapping]
        public abstract string AID { get; }


        /// <summary>
        /// 外键2
        /// </summary>
        [NoMapping]
        public abstract string BID { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        [ORFieldMapping("CreatorName")]
        public string CreatorName { get; set; }

    }
}
