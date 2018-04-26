If Object_Id('dbo.Pro_SystemReport') Is Not Null
Drop Procedure dbo.Pro_SystemReport
Go

Create Procedure dbo.Pro_SystemReport
(
	@SystemID NVarchar(38)='00000000-0000-0000-0000-000000000000',--系统ID
	@Years Int,--年份
	@Month Int,--月份
	@LoginName NVarchar(Max) --登陆人
)
As
Begin 
	Declare @Filter NVarchar(Max)='';
	If(@Years!=0)
	 Set @Filter+=' And  A.FinYear='+Convert(NVarchar(20),@Years)
	If(@Month!=0)
	 Set @Filter+=' And  A.FinMonth='+Convert(NVarchar(20),@Month)
	If(@SystemID!='00000000-0000-0000-0000-000000000000')
		Set @Filter+=' And  A.SystemID='''+@SystemID+'''';
	Begin --获取已经授权的板块 #UserPermissionsSystem
		If Object_Id('tempdb..#UserPermissionsSystem') Is Not Null
		Drop Table #UserPermissionsSystem
		Create Table #UserPermissionsSystem
		(
			ID UniqueIdentifier,
			SystemID UniqueIdentifier,
			CnName NVarchar(50)
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
		        ( [ID], [SystemID], [CnName] )
		Select Distinct [getSystemData].[ID],[getSystemData].[SystemID],[getSystemData].[CnName]  From [getSystemData] Where [Level]=2
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
			NAccumulativeDisplayRate NVarchar(100)
		 )

		Exec('
			 Insert Into [#resultData]
						 ( [SystemID] ,[CnName] ,[TargetName] ,[FinYear] ,[FinMonth] ,[NPlanAmmount] ,[NActualAmmount] ,[NDifference]
						  ,[NDisplayRate] ,[NAccumulativePlanAmmount] ,[NAccumulativeActualAmmount] ,[NAccumulativeDifference] ,[NAccumulativeDisplayRate])
			Select [A].[SystemID],[B].[CnName],C.[TargetName],[FinYear],[FinMonth],[NPlanAmmount],[NActualAmmount],[NDifference],[NDisplayRate],
						 [NAccumulativePlanAmmount],[NAccumulativeActualAmmount],[NAccumulativeDifference],[NAccumulativeDisplayRate]
			From [dbo].[A_MonthlyReportDetail] As A
			Inner Join [#UserPermissionsSystem] As B On A.[SystemID]=B.[SystemID]
			Inner Join [dbo].[C_Target] As C On A.[TargetID]=C.[ID] And C.[IsDeleted]=0
			Where 1=1'+@Filter+'  order by A.SystemID asc ')
	End 
	Select *,(Select Count(*) From [#resultData] As B Where A.SystemID=B.[SystemID]) As Number  From [#resultData] As A Order By A.[SystemID] Asc,A.[TargetName] Asc 
End 
