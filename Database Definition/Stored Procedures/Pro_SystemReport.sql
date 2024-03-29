﻿If Object_Id('dbo.Pro_SystemReport') Is Not Null
Drop Procedure dbo.Pro_SystemReport
Go

Create Procedure dbo.Pro_SystemReport
(
	@SystemID NVarchar(38)='00000000-0000-0000-0000-000000000000',--系统ID
	@Years Int,--年份
	@Month Int,--月份
	@LoginName NVarchar(Max), --登陆人
	@TargetPlanID nvarchar(38)='00000000-0000-0000-0000-000000000000' --版本ID
)
As
Begin 
	Declare @Filter NVarchar(Max)='';
	If(@Years!=0)
	 Set @Filter+=' And  A.FinYear='+Convert(NVarchar(20),@Years) +' And D.FinYear='+Convert(NVarchar(20),@Years)
	If(@Month!=0)
	 Set @Filter+=' And  A.FinMonth='+Convert(NVarchar(20),@Month)
	If(@SystemID!='00000000-0000-0000-0000-000000000000')
		Set @Filter+=' And  A.SystemID='''+@SystemID+'''';
	If(@TargetPlanID!='00000000-0000-0000-0000-000000000000')
		Set @Filter+=' And  A.TargetPlanID='''+@TargetPlanID+'''';
	Else 
		Set @Filter+=' And D.[VersionDefault]=1'
	Begin --获取已经授权的板块 #UserPermissionsSystem
		If Object_Id('tempdb..#UserPermissionsSystem') Is Not Null
		Drop Table #UserPermissionsSystem
		Create Table #UserPermissionsSystem
		(
			ID UniqueIdentifier,
			SystemID UniqueIdentifier,
			CnName NVarchar(50),
			[Sequence] Int
		);
		

		With getSystemData
		As 
		(
			Select A.* From [dbo].[S_Organizational] As A 
			Inner Join [S_Org_User] As B On A.[ID]=B.[CompanyID] And B.[IsDeleted]=0 And B.[LoginName]=@LoginName
			Inner Join [dbo].[Employee] As C On B.[LoginName]=C.[LoginName] And C.[IsDeleted]=0
			Union All
			Select A.* From [dbo].[S_Organizational] As A
			Inner Join [getSystemData] As B On A.[ID]=B.[ParentID] And B.[IsDeleted]=0
			Where A.[IsDeleted]=0
		)
		Insert Into [#UserPermissionsSystem]
		        ( [ID], [SystemID], [CnName],[Sequence] )
		Select Distinct [getSystemData].[ID],[getSystemData].[SystemID],[getSystemData].[CnName],B.[Sequence] 
		From [getSystemData]
		Inner Join [dbo].[C_System] As B On [getSystemData].[SystemID]=B.[ID] And B.[IsDeleted]=0   
		Where [Level]=2
	End

	Begin --获取有效的指标
	If Object_Id('tempdb..#cTargetTempl') Is Not Null
		Drop Table #cTargetTempl
		Create Table #cTargetTempl
		(
			ID UniqueIdentifier,
			SystemID UniqueIdentifier,
			TargetName NVarchar(50),
			[Sequence] Int
		);
		Insert Into #cTargetTempl
		Select C.[ID],C.[SystemID],C.[TargetName],C.[Sequence] From [dbo].[C_Target] As C
		Inner Join [#UserPermissionsSystem] As B On [B].[SystemID] = [C].[SystemID]
		Where C.[IsDeleted]=0 And DateDiff(Day,C.[VersionStart],GetDate())>=0 And DateDiff(Day,GetDate(),C.[VersionEnd])>=0
 
	End

	Begin --得到#resultData
		 If Object_Id('tempdb..#resultData') Is Not Null
		 Drop Table #resultData
		 Create Table [#resultData]
		 (
			SystemID UniqueIdentifier,
			CnName NVarchar(100),
			TargetName NVarchar(100),
			FinYear Int,
			FinMonth Int ,
			NPlanAmmount Decimal(37,7),
			NActualAmmount Decimal(37,7),
			NDifference Decimal(37,7),
			NDisplayRate NVarchar(100),
			NAccumulativePlanAmmount Decimal(37,7),
			NAccumulativeActualAmmount Decimal(37,7),
			NAccumulativeDifference Decimal(37,7),
			NAccumulativeDisplayRate NVarchar(100),
			SystemSequence Int,--板块排序
			TargetSequence Int --指标排序
		 )


		Exec('
			 Insert Into [#resultData]
						 ( [SystemID] ,[CnName] ,[TargetName] ,[FinYear] ,[FinMonth] ,[NPlanAmmount] ,[NActualAmmount] ,[NDifference]
						  ,[NDisplayRate] ,[NAccumulativePlanAmmount] ,[NAccumulativeActualAmmount] ,[NAccumulativeDifference] ,[NAccumulativeDisplayRate],[SystemSequence],[TargetSequence])
			Select
				  [A].[SystemID]
				 ,[B].[CnName]
				 ,C.[TargetName]
				 ,A.[FinYear]
				 ,[FinMonth]
				 ,Sum([NPlanAmmount])																										   As [NPlanAmmount]
				 ,Sum([NActualAmmount])																										   As [NActualAmmount]
				 ,(Sum([NActualAmmount])-Sum([NPlanAmmount]))																				   As [NDifference]
				 ,Case When Sum([NPlanAmmount])=0 then ''0.00%''
					   Else Convert(NVarchar(10), Cast(Round((Sum([NActualAmmount])/ Sum([NPlanAmmount])* 100), 0) As Real))+''%''
					   End  As [NDisplayRate]
				 ,Sum([NAccumulativePlanAmmount])																							   As [NAccumulativePlanAmmount]
				 ,Sum([NAccumulativeActualAmmount])																							   As [NAccumulativeActualAmmount]
				 ,(Sum([NAccumulativeActualAmmount])-Sum([NAccumulativePlanAmmount]))														   As [NAccumulativeDifference]
				 ,Case  When Sum([NAccumulativePlanAmmount])=0 Then ''0.00%''
						Else Convert(NVarchar(10), Cast(Round((Sum([NAccumulativeActualAmmount])/ Sum([NAccumulativePlanAmmount])* 100), 0) As Real))+''%''
						End  As [NAccumulativeDisplayRate]
				 ,B.[Sequence],C.[Sequence]
			From  [dbo].[A_MonthlyReportDetail] As A
			Inner Join [#UserPermissionsSystem] As B On A.[SystemID]=B.[SystemID]
			Inner Join [#cTargetTempl] As C On A.[TargetID]=C.[ID]
			Inner Join [dbo].[A_TargetPlan] As D On A.TargetPlanID=D.ID And D.[IsDeleted]=0
			Where 1=1'+@Filter+' 
			group by [A].[SystemID],[B].[CnName],C.[TargetName],A.[FinYear],[FinMonth],B.[Sequence],C.[Sequence]')
	End 
	Select *,(Select Count(*) From [#resultData] As B Where A.SystemID=B.[SystemID]) As Number  From [#resultData] As A Order By A.[SystemSequence] Asc,A.[TargetSequence] Asc 
End 

