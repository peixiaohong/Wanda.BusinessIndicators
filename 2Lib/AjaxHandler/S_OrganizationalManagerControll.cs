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
            string Message = string.Empty;
            int Success = 0;
            try
            {
                //变量，用来判断如果是项目  在company表中存在的数据
                int companyNameNumbers = 0;
                //存放 如果是项目，根据项目名称查询到项目表的数据
                List<Model.C_Company> oldCompanys = new List<Model.C_Company>();

                S_Organizational entity = JsonConvert.DeserializeObject<S_Organizational>(data);
                //如果用户添加的是项目
                if (IsCompany)
                {
                    //得到用户保存的项目名称在company中存在的数量
                    oldCompanys = C_CompanyOperator.Instance.GetCompanyInfoByName(entity.CnName,entity.SystemID);
                    companyNameNumbers = oldCompanys.Count();
                }
                //新增数据
                if (type == "Add")
                {
                    entity.CreateTime = entity.ModifyTime = DateTime.Now;
                    entity.CreatorName = entity.ModifierName = base.CurrentUserName;
                    //entity.ID = Guid.NewGuid();
                    entity.IsDeleted = false;

                    var disctinctNames=S_OrganizationalActionOperator.Instance.GetSystemsubsetCnName(entity.SystemID, entity.CnName).Where(i=>i.IsCompany==false).ToList();
                    if (disctinctNames.Count > 0)
                    {
                        return new
                        {
                            Data = "",
                            Success = 0,
                            Message = "同一个板块一下不能有重复的名称"
                        };
                    }

                    ////是项目 且名字已经存在company表
                    //if (IsCompany && companyNameNumbers > 0)
                    //{
                    //    return new
                    //    {
                    //        Data = "",
                    //        Success = 0,
                    //        Message = "项目名称已经存在，不允许录入重复的项目"
                    //    };
                    //}
                    //是项目 
                    if (IsCompany)
                    {
                        entity.IsCompany = true;

                        //插入相同的权限
                        //1.得到 这个大区授权的人
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
                            }
                            //2 批量插入权限数据
                            S_Org_UserActionOperator.Instance.InsertListData(orgUserEntitys);
                        }
                    }
                    else
                    {
                        entity.ID = Guid.NewGuid();
                    }
                    //插入组织架构数据
                    int number = S_OrganizationalActionOperator.Instance.InsertData(entity);
                    if (number > 0)
                    {
                        Success = 1;
                        Message = "添加成功";
                    }
                    else
                    {
                        Success = 0;
                        Message = "添加失败";
                    }
                }
                //修改数据
                else
                {

                    var disctinctNames = S_OrganizationalActionOperator.Instance.GetSystemsubsetCnName(entity.SystemID, entity.CnName);
                    if (disctinctNames.Where(i=>i.ID!=entity.ID).Count() > 0)
                    {
                        return new
                        {
                            Data = "",
                            Success = 0,
                            Message = "同一个板块一下不能有重复的名称"
                        };
                    }

                    ////如果是项目
                    //if (IsCompany)
                    //{
                    //    Model.C_Company company = JsonConvert.DeserializeObject<Model.C_Company>(companyData);
                    //    company.IsDeleted = false;
                    //    company.ModifierName = base.CurrentUserName;
                    //    company.ModifyTime = DateTime.Now;
                    //    if (oldCompanys.Where(o => o.ID != entity.ID).Count() > 0)
                    //    {
                    //        return new
                    //        {
                    //            Data = "",
                    //            Success = 0,
                    //            Message = "项目名称重复"
                    //        };
                    //    }
                    //    var oldCompany = C_CompanyOperator.Instance.GetCompany(entity.ID);
                    //    company.CreateTime = oldCompany.CreateTime;
                    //    company.CreatorName = oldCompany.CreatorName;
                    //    company.VersionEnd = oldCompany.VersionEnd;
                    //    company.VersionStart = oldCompany.VersionStart;
                    //    company.ID = oldCompany.ID;
                    //    C_CompanyOperator.Instance.UpdateCompany(company);
                    //}
                    int number = S_OrganizationalActionOperator.Instance.UpdateData(entity);
                    if (number > 0)
                    {
                        Success = 1;
                        Message = "修改成功";
                    }
                    else
                    {
                        Success = 0;
                        Message = "修改失败";
                    }
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
        /// 删除节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [LibAction]
        public object DeleteData(string id,bool isCompany)
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
        public object GetCompanyInfo(string systemID, string keyWord, int pageIndex,int pageSize)
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
