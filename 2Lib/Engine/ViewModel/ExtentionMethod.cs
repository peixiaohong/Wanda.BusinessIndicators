using Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LJTH.BusinessIndicators.ViewModel;
using LJTH.BusinessIndicators.Model;
using LJTH.BusinessIndicators.BLL;

namespace LJTH.BusinessIndicators.Engine
{
    public static class ExtensionMethod
    {
        public static Guid ToGuid(this string value)
        {
            Guid result = Guid.Empty;
            Guid.TryParse(value, out result);
            return result;
        }


        /// <summary>
        /// 扩展foreach方法
        /// </summary>
        /// <typeparam name="T">集合中元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="action">执行的方法</param>
        /// <returns>执行后的集合</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
            return list;
        }



        public static A_MonthlyReport ToAModel(this MonthlyReport value)
        {
            A_MonthlyReport result = new A_MonthlyReport
            {
                ID = value.ID,
                SystemID = value.SystemID,
                Description = value.Description,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                Status = value.Status,
                SystemBatchID = value.SystemBatchID
            };
            return result;
        }

        public static B_MonthlyReport ToBModel(this MonthlyReport value)
        {
            B_MonthlyReport result = new B_MonthlyReport
            {
                ID = value.ID,
                SystemID = value.SystemID,
                Description = value.Description,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                Status = value.Status,
                WFStatus = value.WFStatus,
                SystemBatchID = value.SystemBatchID
            };
            return result;
        }

        public static MonthlyReport ToVModel(this A_MonthlyReport value)
        {
            MonthlyReport result = new MonthlyReport
            {
                ID = value.ID,
                SystemID = value.SystemID,
                Description = value.Description,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                Status = value.Status,
                SystemBatchID = value.SystemBatchID,
                TargetPlanID=value.TargetPlanID
            };
            return result;
        }

        public static MonthlyReport ToVModel(this B_MonthlyReport value)
        {
            MonthlyReport result = new MonthlyReport
            {
                ID = value.ID,
                SystemID = value.SystemID,
                Description = value.Description,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                Status = value.Status,
                WFStatus = value.WFStatus,
                SystemBatchID = value.SystemBatchID,
                TargetPlanID = value.TargetPlanID
                
            };
            return result;
        }

        public static A_MonthlyReportDetail ToAModel(this MonthlyReportDetail value)
        {
            A_MonthlyReportDetail result = new A_MonthlyReportDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                OPlanAmmount = value.OPlanAmmount,
                OActualAmmount = value.OActualAmmount,
                OActualRate = value.OActualRate,
                ODisplayRate = value.ODisplayRate,
                NPlanAmmount = value.NPlanAmmount,
                NActualAmmount = value.NActualAmmount,
                NActualRate = value.NActualRate,
                NDisplayRate = value.NDisplayRate,
                OAccumulativePlanAmmount = value.OAccumulativePlanAmmount,
                OAccumulativeActualAmmount = value.OAccumulativeActualAmmount,
                OAccumulativeActualRate = value.OAccumulativeActualRate,
                OAcccumulativeDisplayRate = value.OAcccumulativeDisplayRate,
                NAccumulativePlanAmmount = value.NAccumulativePlanAmmount,
                NAccumulativeActualAmmount = value.NAccumulativeActualAmmount,
                NAccumulativeActualRate = value.NAccumulativeActualRate,
                NAccumulativeDisplayRate = value.NAccumulativeDisplayRate,
                IsMissTarget = value.IsMissTarget,
                Counter = value.Counter,
                FirstMissTargetDate = value.FirstMissTargetDate,
                PromissDate = value.PromissDate,
                CommitDate = value.CommitDate,
                MIssTargetReason = value.MIssTargetReason,
                MIssTargetDescription = value.MIssTargetDescription,
                CurrentMIssTargetReason = value.CurrentMIssTargetReason,
                CurrentMIssTargetDescription = value.CurrentMIssTargetDescription,
                ReturnType = value.ReturnType,
                Title = value.Title,
                IsMissTargetCurrent = value.IsMissTargetCurrent,
                NAccumulativeDifference =value.NAccumulativeDifference,
                NDifference =value.NDifference,
                OAccumulativeDifference =value.OAccumulativeDifference,
                ODifference =value.ODifference,
                GroupTile =value.GroupTile,
                MonthlyReportID = value.MonthlyReportID,
                MeasureRate = value.MeasureRate,
                ReturnDescription =value.ReturnDescription,
                Display=value.Display,
                CompanyProperty1=value.CompanyProperty1,
                IsCommitDate = value.IsCommitDate,
                CommitReason = value.CommitReason,
                IsDelayComplete = value.IsDelayComplete,
                CurrentMonthCommitDate = value.CurrentMonthCommitDate,
                CurrentMonthCommitReason = value.CurrentMonthCommitReason,
                ReturnType_Sub = value.ReturnType_Sub,
                NewCounter = value.NewCounter,
                CompanyProperty = value.CompanyProperty
            };
            return result;
        }

        public static B_MonthlyReportDetail ToBModel(this MonthlyReportDetail value)
        {
            B_MonthlyReportDetail result = new B_MonthlyReportDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                OPlanAmmount = value.OPlanAmmount,
                OActualAmmount = value.OActualAmmount,
                OActualRate = value.OActualRate,
                ODisplayRate = value.ODisplayRate,
                NPlanAmmount = value.NPlanAmmount,
                NActualAmmount = value.NActualAmmount,
                NActualRate = value.NActualRate,
                NDisplayRate = value.NDisplayRate,
                OAccumulativePlanAmmount = value.OAccumulativePlanAmmount,
                OAccumulativeActualAmmount = value.OAccumulativeActualAmmount,
                OAccumulativeActualRate = value.OAccumulativeActualRate,
                OAcccumulativeDisplayRate = value.OAcccumulativeDisplayRate,
                NAccumulativePlanAmmount = value.NAccumulativePlanAmmount,
                NAccumulativeActualAmmount = value.NAccumulativeActualAmmount,
                NAccumulativeActualRate = value.NAccumulativeActualRate,
                NAccumulativeDisplayRate = value.NAccumulativeDisplayRate,
                IsMissTarget = value.IsMissTarget,
                IsMissTargetCurrent = value.IsMissTargetCurrent,
                Counter = value.Counter,
                FirstMissTargetDate = value.FirstMissTargetDate,
                PromissDate = value.PromissDate,
                CommitDate = value.CommitDate,
                MIssTargetReason = value.MIssTargetReason,
                MIssTargetDescription = value.MIssTargetDescription,
                CurrentMIssTargetDescription = value.CurrentMIssTargetDescription,
                CurrentMIssTargetReason = value.CurrentMIssTargetReason,
                NAccumulativeDifference = value.NAccumulativeDifference,
                NDifference = value.NDifference,
                OAccumulativeDifference = value.OAccumulativeDifference,
                ODifference = value.ODifference,
                ReturnType = value.ReturnType,
                MonthlyReportID = value.MonthlyReportID,
                MeasureRate = value.MeasureRate,
                ReturnDescription = value.ReturnDescription,
                Display=value.Display,
                CompanyProperty1 = value.CompanyProperty1,
                IsCommitDate = value.IsCommitDate,
                CommitReason = value.CommitReason,
                CurrentMonthCommitDate = value.CurrentMonthCommitDate,
                CurrentMonthCommitReason = value.CurrentMonthCommitReason,
                ReturnType_Sub = value.ReturnType_Sub,
                NewCounter = value.NewCounter,
                CompanyProperty = value.CompanyProperty
            };
            return result;
        }

        public static MonthlyReportDetail ToVModel(this A_MonthlyReportDetail value)
        {
            //这是为了前台显示，美观强制去掉“\n”
            string tempMIssTargetReason = value.MIssTargetReason.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
            if (string.IsNullOrEmpty(tempMIssTargetReason)){
                tempMIssTargetReason = "";
            }
            else{
                tempMIssTargetReason = "\n" + tempMIssTargetReason;
            }

            string tempMIssTargetDescription = value.MIssTargetDescription.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
            if (string.IsNullOrEmpty(tempMIssTargetDescription)){
                tempMIssTargetDescription = "";
            }
            else{
                tempMIssTargetDescription = "\n" + tempMIssTargetDescription;
            }


            string tempCurrentMIssTargetReason = value.CurrentMIssTargetReason.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
            if (string.IsNullOrEmpty(tempCurrentMIssTargetReason))
            {
                tempCurrentMIssTargetReason = "";
            }
            else
            {
                tempCurrentMIssTargetReason = "\n" + tempCurrentMIssTargetReason;
            }

            string tempCurrentMIssTargetDescription = value.CurrentMIssTargetDescription.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
            if (string.IsNullOrEmpty(tempCurrentMIssTargetDescription))
            {
                tempCurrentMIssTargetDescription = "";
            }
            else
            {
                tempCurrentMIssTargetDescription = "\n" + tempCurrentMIssTargetDescription;
            }



            string tempReturnDescription = value.ReturnDescription.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
            if (string.IsNullOrEmpty(tempReturnDescription))
            {
                tempReturnDescription = "";
            }

            string tempCommitReason = value.CommitReason.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
            if (string.IsNullOrEmpty(tempCommitReason))
            {
                tempCommitReason = "";
            }
            string strSystemName=string.Empty;
            C_System _cSystem=StaticResource.Instance[value.SystemID,DateTime.Now];
            if (_cSystem != null)
            {
                strSystemName = _cSystem.SystemName;
            }

            string strCompanyName=string.Empty;
            C_Company _cCompany=StaticResource.Instance.GetCompanyModel(value.CompanyID);
            if(_cCompany!=null)
            {
                strCompanyName=_cCompany.CompanyName;
            }
            string strTargetName=string.Empty;
            C_Target _cTarget=StaticResource.Instance.TargetList[value.SystemID].Where(p => p.ID == value.TargetID).FirstOrDefault();
            if(_cTarget!=null)
            {
                strTargetName=_cTarget.TargetName;
            }


            MonthlyReportDetail result = new MonthlyReportDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                OPlanAmmount = value.OPlanAmmount,
                OActualAmmount = value.OActualAmmount,
                OActualRate = value.OActualRate,
                ODisplayRate = value.ODisplayRate,
                NPlanAmmount = value.NPlanAmmount,
                NActualAmmount = value.NActualAmmount,
                NActualRate = value.NActualRate,
                NDisplayRate = value.NDisplayRate,
                OAccumulativePlanAmmount = value.OAccumulativePlanAmmount,
                OAccumulativeActualAmmount = value.OAccumulativeActualAmmount,
                OAccumulativeActualRate = value.OAccumulativeActualRate,
                OAcccumulativeDisplayRate = value.OAcccumulativeDisplayRate,
                NAccumulativePlanAmmount = value.NAccumulativePlanAmmount,
                NAccumulativeActualAmmount = value.NAccumulativeActualAmmount,
                NAccumulativeActualRate = value.NAccumulativeActualRate,
                NAccumulativeDisplayRate = value.NAccumulativeDisplayRate,
                IsMissTarget = value.IsMissTarget,
                IsMissTargetCurrent = value.IsMissTargetCurrent,
                Counter = value.Counter,
                FirstMissTargetDate = value.FirstMissTargetDate,
                PromissDate = value.PromissDate,
                CommitDate = value.CommitDate,
                MIssTargetReason = tempMIssTargetReason,
                MIssTargetDescription = tempMIssTargetDescription,
                CurrentMIssTargetDescription = tempCurrentMIssTargetDescription,
                CurrentMIssTargetReason = tempCurrentMIssTargetReason,
                ReturnType = value.ReturnType,
                NAccumulativeDifference = value.NAccumulativeDifference,
                NDifference = value.NDifference,
                OAccumulativeDifference = value.OAccumulativeDifference,
                ODifference = value.ODifference,
                Title = value.Title,
                GroupTile = value.GroupTile,
                MonthlyReportID = value.MonthlyReportID,
                MeasureRate = value.MeasureRate,
                ReturnDescription = tempReturnDescription,
                SystemName = strSystemName,
                Company = _cCompany,
                CompanyName = strCompanyName,
                TargetName = strTargetName,
                Display=value.Display,
                CompanyProperty1 = value.CompanyProperty1,
                IsCommitDate = value.IsCommitDate,
                CommitReason = tempCommitReason,
                IsDelayComplete =value.IsDelayComplete,
                CurrentMonthCommitDate = value.CurrentMonthCommitDate,
                CurrentMonthCommitReason = value.CurrentMonthCommitReason,
                ReturnType_Sub = value.ReturnType_Sub,
                NewCounter = value.NewCounter,
                CompanyProperty = value.CompanyProperty
            };
            return result;
        }

        public static MonthlyReportDetail ToVModel(this B_MonthlyReportDetail value)
        {


            //这是为了前台显示，美观强制去掉“\n”
            string tempMIssTargetReason = string.Empty;
            if (value.MIssTargetReason != null)
            {
                tempMIssTargetReason = value.MIssTargetReason.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
                if (string.IsNullOrEmpty(tempMIssTargetReason))
                {
                    tempMIssTargetReason = "";
                }
                else
                {
                    tempMIssTargetReason = "\n" + tempMIssTargetReason;
                }
            }
            string tempMIssTargetDescription = string.Empty;
            if (value.MIssTargetDescription != null)
            {
                tempMIssTargetDescription = value.MIssTargetDescription.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
                if (string.IsNullOrEmpty(tempMIssTargetDescription))
                {
                    tempMIssTargetDescription = "";
                }
                else
                {
                    tempMIssTargetDescription = "\n" + tempMIssTargetDescription;
                }
            }

            string tempCurrentMIssTargetReason = string.Empty;
            if (value.CurrentMIssTargetReason!= null)
            {
                tempCurrentMIssTargetReason = value.CurrentMIssTargetReason.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
                if (string.IsNullOrEmpty(tempCurrentMIssTargetReason))
                {
                    tempCurrentMIssTargetReason = "";
                }
                else
                {
                    tempCurrentMIssTargetReason = "\n" + tempCurrentMIssTargetReason;
                }
            }
            string tempCurrentMIssTargetDescription = string.Empty;
            if (value.CurrentMIssTargetDescription != null)
            {
                tempCurrentMIssTargetDescription = value.CurrentMIssTargetDescription.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
                if (string.IsNullOrEmpty(tempCurrentMIssTargetDescription))
                {
                    tempCurrentMIssTargetDescription = "";
                }
                else
                {
                    tempCurrentMIssTargetDescription = "\n" + tempCurrentMIssTargetDescription;
                }
            }


            string tempReturnDescription = string.Empty;
            if (value.ReturnDescription != null)
            {
                 tempReturnDescription = value.ReturnDescription.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
                if (string.IsNullOrEmpty(tempReturnDescription))
                {
                    tempReturnDescription = "";
                }
            }

            string tempCommitReason = string.Empty;
            if (value.CommitReason != null)
            {
                tempCommitReason = value.CommitReason.TrimStart('\n').TrimStart('\r').TrimEnd('\n').TrimEnd('\r');
                if (string.IsNullOrEmpty(tempCommitReason))
                {
                    tempCommitReason = "";
                }
            }

            MonthlyReportDetail result = new MonthlyReportDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                OPlanAmmount = value.OPlanAmmount,
                OActualAmmount = value.OActualAmmount,
                OActualRate = value.OActualRate,
                ODisplayRate = value.ODisplayRate,
                NPlanAmmount = value.NPlanAmmount,
                NActualAmmount = value.NActualAmmount,
                NActualRate = value.NActualRate,
                NDisplayRate = value.NDisplayRate,
                OAccumulativePlanAmmount = value.OAccumulativePlanAmmount,
                OAccumulativeActualAmmount = value.OAccumulativeActualAmmount,
                OAccumulativeActualRate = value.OAccumulativeActualRate,
                OAcccumulativeDisplayRate = value.OAcccumulativeDisplayRate,
                NAccumulativePlanAmmount = value.NAccumulativePlanAmmount,
                NAccumulativeActualAmmount = value.NAccumulativeActualAmmount,
                NAccumulativeActualRate = value.NAccumulativeActualRate,
                NAccumulativeDisplayRate = value.NAccumulativeDisplayRate,
                IsMissTarget = value.IsMissTarget,
                IsMissTargetCurrent = value.IsMissTargetCurrent,
                Counter = value.Counter,
                FirstMissTargetDate = value.FirstMissTargetDate,
                PromissDate = value.PromissDate,
                CommitDate = value.CommitDate,
                MIssTargetReason = tempMIssTargetReason,
                MIssTargetDescription =tempMIssTargetDescription,
                CurrentMIssTargetReason = tempCurrentMIssTargetReason,
                CurrentMIssTargetDescription = tempCurrentMIssTargetDescription,
                NAccumulativeDifference = value.NAccumulativeDifference,
                NDifference = value.NDifference,
                OAccumulativeDifference = value.OAccumulativeDifference,
                ODifference = value.ODifference,
                ReturnType = value.ReturnType,
                MonthlyReportID = value.MonthlyReportID,
                MeasureRate = value.MeasureRate,
                ReturnDescription = tempReturnDescription,
                SystemName = StaticResource.Instance[value.SystemID,DateTime.Now].SystemName,
               
                Company = value.CompanyID==Guid.Empty?null:StaticResource.Instance.GetCompanyModel(value.CompanyID),

                CompanyName = value.CompanyID == Guid.Empty ? null : StaticResource.Instance.GetCompanyModel(value.CompanyID).CompanyName,
                TargetName = StaticResource.Instance.TargetList[value.SystemID].Where(p => p.ID == value.TargetID).FirstOrDefault().TargetName,
                TargetType = StaticResource.Instance.TargetList[value.SystemID].Where(p => p.ID == value.TargetID).FirstOrDefault().TargetType,
                Display = value.Display,
                CompanyProperty1 = value.CompanyProperty1,
                IsCommitDate = value.IsCommitDate,
                CommitReason = tempCommitReason,
                CurrentMonthCommitDate = value.CurrentMonthCommitDate,
                CurrentMonthCommitReason =value.CurrentMonthCommitReason,
                ReturnType_Sub = value.ReturnType_Sub,
                NewCounter = value.NewCounter,
                CompanyProperty = value.CompanyProperty

            };
            return result;
        }


     
        /// <summary>
        ///  B To A  这里个方法只是同于 提交审批最后进入最终的数据,IsBToA:True ,标识特殊处理， false：表示正常处理B 表到A表数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="IsBToA">是否B表数据转换成A表数据</param>
        /// <returns></returns>
        public static A_MonthlyReportDetail ToAModel(this B_MonthlyReportDetail value , bool IsBToA)
        {
            int _IsCommitDate = 0;

            if (IsBToA) //审批
            {
                if (value.PromissDate != null && value.PromissDate.Value.AddMonths(-1).Month == value.FinMonth && value.PromissDate.Value.AddMonths(-1).Year == value.FinYear)
                { 
                   _IsCommitDate = 1;
                }
            }
            else { 
                _IsCommitDate = value.IsCommitDate;
            }

            A_MonthlyReportDetail result = new A_MonthlyReportDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                OPlanAmmount = value.OPlanAmmount,
                OActualAmmount = value.OActualAmmount,
                OActualRate = value.OActualRate,
                ODisplayRate = value.ODisplayRate,
                NPlanAmmount = value.NPlanAmmount,
                NActualAmmount = value.NActualAmmount,
                NActualRate = value.NActualRate,
                NDisplayRate = value.NDisplayRate,
                OAccumulativePlanAmmount = value.OAccumulativePlanAmmount,
                OAccumulativeActualAmmount = value.OAccumulativeActualAmmount,
                OAccumulativeActualRate = value.OAccumulativeActualRate,
                OAcccumulativeDisplayRate = value.OAcccumulativeDisplayRate,
                NAccumulativePlanAmmount = value.NAccumulativePlanAmmount,
                NAccumulativeActualAmmount = value.NAccumulativeActualAmmount,
                NAccumulativeActualRate = value.NAccumulativeActualRate,
                NAccumulativeDisplayRate = value.NAccumulativeDisplayRate,
                IsMissTarget = value.IsMissTarget,
                Counter = value.Counter,
                FirstMissTargetDate = value.FirstMissTargetDate,
                PromissDate = value.PromissDate,
                CommitDate = value.CommitDate,
                MIssTargetReason = value.MIssTargetReason,
                MIssTargetDescription = value.MIssTargetDescription,
                CurrentMIssTargetReason = value.CurrentMIssTargetReason,
                CurrentMIssTargetDescription = value.CurrentMIssTargetDescription,
                ReturnType = value.ReturnType,
                IsMissTargetCurrent = value.IsMissTargetCurrent,
                NAccumulativeDifference = value.NAccumulativeDifference,
                NDifference = value.NDifference,
                OAccumulativeDifference = value.OAccumulativeDifference,
                ODifference = value.ODifference,
                MonthlyReportID = value.MonthlyReportID,
                MeasureRate = value.MeasureRate,
                ReturnDescription = value.ReturnDescription,
                Display = value.Display,
                CompanyProperty1 = value.CompanyProperty1,
                IsCommitDate = _IsCommitDate,
                CommitReason = value.CommitReason,
                IsDelayComplete =false,
                CurrentMonthCommitDate = value.CurrentMonthCommitDate,
                CurrentMonthCommitReason =value.CurrentMonthCommitReason,
                ReturnType_Sub = value.ReturnType_Sub,
                NewCounter = value.NewCounter,
                CompanyProperty = value.CompanyProperty
            };
            return result;
        }

        /// <summary>
        /// A To B
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static B_MonthlyReportDetail ToBModel(this A_MonthlyReportDetail value)
        {
            B_MonthlyReportDetail result = new B_MonthlyReportDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                OPlanAmmount = value.OPlanAmmount,
                OActualAmmount = value.OActualAmmount,
                OActualRate = value.OActualRate,
                ODisplayRate = value.ODisplayRate,
                NPlanAmmount = value.NPlanAmmount,
                NActualAmmount = value.NActualAmmount,
                NActualRate = value.NActualRate,
                NDisplayRate = value.NDisplayRate,
                OAccumulativePlanAmmount = value.OAccumulativePlanAmmount,
                OAccumulativeActualAmmount = value.OAccumulativeActualAmmount,
                OAccumulativeActualRate = value.OAccumulativeActualRate,
                OAcccumulativeDisplayRate = value.OAcccumulativeDisplayRate,
                NAccumulativePlanAmmount = value.NAccumulativePlanAmmount,
                NAccumulativeActualAmmount = value.NAccumulativeActualAmmount,
                NAccumulativeActualRate = value.NAccumulativeActualRate,
                NAccumulativeDisplayRate = value.NAccumulativeDisplayRate,
                IsMissTarget = value.IsMissTarget,
                IsMissTargetCurrent = value.IsMissTargetCurrent,
                Counter = value.Counter,
                FirstMissTargetDate = value.FirstMissTargetDate,
                PromissDate = value.PromissDate,
                CommitDate = value.CommitDate,
                MIssTargetReason = value.MIssTargetReason,
                MIssTargetDescription = value.MIssTargetDescription,
                CurrentMIssTargetDescription=value.CurrentMIssTargetDescription,
                CurrentMIssTargetReason = value.CurrentMIssTargetReason,
                NAccumulativeDifference = value.NAccumulativeDifference,
                NDifference = value.NDifference,
                OAccumulativeDifference = value.OAccumulativeDifference,
                ODifference = value.ODifference,
                ReturnType = value.ReturnType,
                MonthlyReportID = value.MonthlyReportID,
                MeasureRate = value.MeasureRate,
                ReturnDescription = value.ReturnDescription,
                Display = value.Display,
                CompanyProperty1 = value.CompanyProperty1,
                IsCommitDate = value.IsCommitDate,
                CommitReason = value.CommitReason,
                CurrentMonthCommitDate = value.CurrentMonthCommitDate,
                CurrentMonthCommitReason =value.CurrentMonthCommitReason,
                ReturnType_Sub = value.ReturnType_Sub,
                NewCounter = value.NewCounter,
                CompanyProperty = value.CompanyProperty
            };
            return result;
        }

        public static A_TargetPlanDetail ToAModel(this B_TargetPlanDetail value)
        {
            A_TargetPlanDetail result = new A_TargetPlanDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                Target=value.Target,
                Versionend=value.Versionend,
                VersionStart=value.VersionStart,
                
            };
            return result;
        }

        public static B_TargetPlanDetail ToBModel(this A_TargetPlanDetail value)
        {
            B_TargetPlanDetail result = new B_TargetPlanDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                Target = value.Target,
                Versionend = value.Versionend,
                VersionStart = value.VersionStart,
            };
            return result;
        }

        public static TargetPlanDetail ToVModel(this A_TargetPlanDetail value)
        {
            TargetPlanDetail result = new TargetPlanDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                Target = value.Target,
                Versionend = value.Versionend,
                VersionStart = value.VersionStart,
                CompanyName=StaticResource.Instance.GetCompanyModel(value.CompanyID).CompanyName,
                OpeningTime = StaticResource.Instance.GetCompanyModel(value.CompanyID).OpeningTime,
            };
            return result;
        }

        public static TargetPlanDetail ToVModel(this B_TargetPlanDetail value)
        {
            TargetPlanDetail result = new TargetPlanDetail
            {
                ID = value.ID,
                SystemID = value.SystemID,
                IsDeleted = value.IsDeleted,
                FinMonth = value.FinMonth,
                FinYear = value.FinYear,
                CreateTime = value.CreateTime,
                CreatorName = value.CreatorName,
                ModifierName = value.ModifierName,
                ModifyTime = value.ModifyTime,
                TargetID = value.TargetID,
                CompanyID = value.CompanyID,
                TargetPlanID = value.TargetPlanID,
                Target = value.Target,
                Versionend = value.Versionend,
                VersionStart = value.VersionStart,
                CompanyName = StaticResource.Instance.GetCompanyModel(value.CompanyID).CompanyName,
                OpeningTime = StaticResource.Instance.GetCompanyModel(value.CompanyID).OpeningTime,
            };
            return result;
        }

       
        public static List<MonthlyReportDetail> Filter(this List<MonthlyReportDetail> value, C_System System, Dictionary<string,List<string>> Filter )
        {
            List<MonthlyReportDetail> result = new List<MonthlyReportDetail>();

            value.ForEach(D => Filter.ContainsKey(D.Company.CompanyProperty1));

            return result;
        }

    }
}
