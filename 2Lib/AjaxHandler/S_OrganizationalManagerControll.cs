using Lib.Data;
using Lib.Web;
using Lib.Web.MVC.Controller;
using LJTH.BusinessIndicators.BLL;
using LJTH.BusinessIndicators.BLL.BizBLL;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.Model.BizModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LJTH.BusinessIndicators.Web.AjaxHandler
{
    /// <summary>
    /// 组织架构
    /// </summary>
    public class S_OrganizationalManagerControll : BaseController
    {
        /// <summary>
        /// 获取所有组织架构的数据
        /// </summary>
        /// <returns></returns>
        [LibAction]
        public object GetAllOrgData()
        {
            try
            {
                var data = S_OrganizationalActionOperator.Instance.GetAllData();
                var success = 1;
                return new
                {
                    Data = data,
                    Success = success,
                    Message = "查询数据没有问题"
                };
            }
            catch (Exception e)
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// 保存组织架构数据
        /// </summary>
        /// <param name="type">新增 Add 修改 Edit </param>
        /// <param name="data">组织架构数据</param>
        /// <param name="IsCompany">是否添加的是项目公司</param>
        /// <returns></returns>
        [LibAction]
        public object SaveData(string type, string data, bool IsCompany)
        {
            string Message = string.Empty;//返回信息
            int Success = 0;//返回状态  1：成功 0：失败
            List<S_Organizational> updateEntity = new List<S_Organizational>();//变量用于存放：新增项目，该项目在表中存在，则进行修改
            List<S_Organizational> removeEntity = new List<S_Organizational>();//变量用于存放：新增项目，该项目在表中存在，需要从新增项目对象集合中删除的项目数据
            List<S_Org_User> orgUserEnitity = new List<S_Org_User>();//变量用于存放：新增项目，该需要同步当前大区的权限

            //因为需要插入/修改 多张表数据，因此开启事物
            using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
            {
                try
                {
                    //放序列化前端传递过来的数据，得到要新增或者修改的数据的对象集合
                    List<S_Organizational> entitys = JsonConvert.DeserializeObject<List<S_Organizational>>(data);
                    //初步判断是否有数据
                    if (entitys == null || entitys.Count <= 0)
                    {
                        return new
                        {
                            Data = "",
                            Success = 0,
                            Message = "参数丢失"
                        };
                    }

                    //新增数据
                    if (type == "Add")
                    {
                        //循环数据集，更新默认字段值
                        foreach (var entity in entitys)
                        {
                            entity.CreateTime = entity.ModifyTime = DateTime.Now;
                            entity.CreatorName = entity.ModifierName = base.CurrentUserName;
                            entity.IsDeleted = false;
                            //是项目 
                            if (IsCompany)
                            {
                                //新增项目需要插入当前大区下项目一样的权限
                                //得到 这个大区授权的人
                                List<S_Org_User> orgUserEntitys = S_Org_UserActionOperator.Instance.GetRegionalPermissions(entity.ParentID);
                                if (orgUserEntitys != null && orgUserEntitys.Count > 0)
                                {
                                    foreach (var item in orgUserEntitys)
                                    {
                                        item.CreateTime = item.ModifyTime = DateTime.Now;
                                        item.CreatorName = item.ModifierName = base.CurrentUserName;
                                        item.IsDeleted = false;
                                        item.ID = Guid.NewGuid();
                                        item.SystemID = entity.SystemID;
                                        item.CompanyID = entity.ID;

                                        orgUserEnitity.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                //得到这个板块下面不是项目的数据
                                var disctinctNames = S_OrganizationalActionOperator.Instance.GetSystemsubsetCnName(entity.SystemID, entity.CnName).Where(i => i.IsCompany == false).ToList();
                                if (disctinctNames.Count > 0)
                                {
                                    return new
                                    {
                                        Data = "",
                                        Success = 0,
                                        Message = "同一个板块一下大区的名称不能重复"
                                    };
                                }
                                //大区的ID是自动生成的
                                entity.ID = Guid.NewGuid();
                            }
                        }

                        if (IsCompany)
                        {
                            //获取数据库中当前板块下面的项目数据
                            var oldData = S_OrganizationalActionOperator.Instance.GetCompanyInfoBySystemID(entitys[0].SystemID);

                            //如果项目在数据库中存在
                            if (oldData != null && oldData.Count > 0)
                            {
                                for (int i = 0; i < oldData.Count; i++)
                                {
                                    foreach (var item in entitys)
                                    {
                                        //如果项目在数据库中存在且又存在于新增的项目数据集中
                                        if (oldData[i].SystemID == item.SystemID && oldData[i].ID == item.ID)
                                        {
                                            oldData[i].CnName = item.CnName;
                                            oldData[i].CreateTime = oldData[i].ModifyTime = DateTime.Now;
                                            oldData[i].CreatorName = oldData[i].ModifierName = base.CurrentUserName;
                                            oldData[i].Level = item.Level;
                                            oldData[i].IsDeleted = false;
                                            oldData[i].ParentID = item.ParentID;
                                            oldData[i].Code = item.Code;
                                            updateEntity.Add(oldData[i]);
                                            removeEntity.Add(oldData[i]);
                                        }
                                    }
                                }
                            }
                        }


                        //插入组织架构数据
                        if (IsCompany)
                        {
                            if (updateEntity.Count > 0)
                            {
                                S_OrganizationalActionOperator.Instance.UpdateListData(updateEntity);
                            }
                            if (removeEntity.Count > 0)
                            {
                                foreach (var item in removeEntity)
                                {
                                    entitys.Remove(item);
                                }
                            }
                            if (orgUserEnitity.Count > 0)
                            {
                                S_Org_UserActionOperator.Instance.InsertListData(orgUserEnitity);
                            }
                        }
                        int number = S_OrganizationalActionOperator.Instance.InsertListData(entitys);

                        scope.Complete();

                        Success = 1;
                        Message = "添加成功";
                    }

                    //修改数据
                    else
                    {
                        //修改数据只会一次性改一条
                        var disctinctNames = S_OrganizationalActionOperator.Instance.GetSystemsubsetCnName(entitys[0].SystemID, entitys[0].CnName);
                        if (disctinctNames.Where(i => i.ID != entitys[0].ID).Count() > 0)
                        {
                            return new
                            {
                                Data = "",
                                Success = 0,
                                Message = "同一个板块一下不能有重复的名称"
                            };
                        }
                        int number = S_OrganizationalActionOperator.Instance.UpdateData(entitys[0]);
                        scope.Complete();
                        Success = 1;
                        Message = "修改成功";

                    }
                    return new
                    {
                        Data = "",
                        Success = Success,
                        Message = Message
                    };
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return new
                    {
                        Data = "",
                        Success = 0,
                        Message = ex.Message
                    };
                }
            }

        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [LibAction]
        public object DeleteData(string id, bool isCompany)
        {
            if (id == "")
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = "参数丢失"
                };
            }
            string Message = string.Empty;
            int Success = 0;
            try
            {
                if (S_OrganizationalActionOperator.Instance.GetChildDataByID(id.ToGuid()).Count > 0)
                {
                    return new
                    {
                        Data = "",
                        Success = 0,
                        Message = "已经拥有子目录的节点不允许删除"
                    };
                }
                //if (S_Org_UserActionOperator.Instance.GetDataByOrgID(id.ToGuid()).Count > 0)
                //{
                //    return new
                //    {
                //        Data = "",
                //        Success = 0,
                //        Message = "已经授权的组织不能删除"
                //    };
                //}
                //if (isCompany)
                //{
                //    C_Company company= C_CompanyOperator.Instance.GetCompany(id.ToGuid());
                //    company.IsDeleted = true;
                //    company.VersionEnd = DateTime.Now;
                //    C_CompanyOperator.Instance.UpdateCompany(company);
                //}
                var number = S_OrganizationalActionOperator.Instance.DeleteData(id.ToGuid());
                if (number > 0)
                {
                    Success = 1;
                    Message = "删除成功";
                }
                else
                {
                    Success = 0;
                    Message = "删除失败";
                }
                return new
                {
                    Data = "",
                    Success = Success,
                    Message = Message
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = "",
                    Success = 0,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取板块信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [LibAction]
        public object GetSystemInfo(string id)
        {
            string Message = string.Empty;
            int Success = 0;
            try
            {
                var data = C_SystemOperator.Instance.GetSystem(id.ToGuid());
                return new
                {
                    Data = data,
                    Success = 1,
                    Message = Message
                };
            }
            catch (Exception ex)
            {

                return new
                {
                    Data = "",
                    Success = 0,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取当前板块下没有添加的项目
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        [LibAction]
        public object GetCompanyInfo(string systemID, string keyWord, int pageIndex, int pageSize)
        {
            int TotalCount = 0;
            try
            {
                var data = C_CompanyOperator.Instance.GetCompanyInfoBySystem(systemID.ToGuid(), keyWord, pageIndex, pageSize, out TotalCount);
                return new
                {
                    Data = data,
                    TotalCount = TotalCount,
                    Success = 1,
                    Message = "查询成功"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Data = "",
                    TotalCount = TotalCount,
                    Success = 0,
                    Message = ex.Message
                };
            }
        }
    }
}
