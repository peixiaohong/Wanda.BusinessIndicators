using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LJTH.Lib.Data.AppBase
{
    public abstract class CascadeObjectBuilder<T>
        where T : ICascadeObject
    {
        /// <summary>
        /// 将树形结构的数据， 顺序化排列
        /// </summary>
        /// <param name="TopLevels"></param>
        /// <returns></returns>
        protected List<T> Sequence(List<T> TopLevels)
        {
            List<T> result = new List<T>();

            foreach (T item in TopLevels)
            {
                FillWthChildren(result, item);
            }
            return result;
        }

        /// <summary>
        /// 递归的将子项插入到结果中
        /// </summary>
        /// <param name="vmReturn"></param>
        /// <param name="item"></param>
        private void FillWthChildren(List<T> vmReturn, T item)
        {
            vmReturn.Add(item);
            List<ICascadeObject> childrens = item.GetChildren();
            if (childrens.Count > 0)
            {
                foreach (T vm in childrens)
                {
                    FillWthChildren(vmReturn, vm);
                }
                item.RemoveChildren();
            }
        }

        public abstract List<T> Build();
    }
}
