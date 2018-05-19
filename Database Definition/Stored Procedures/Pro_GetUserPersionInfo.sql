If Object_Id('dbo.Pro_GetUserPersionInfo') Is Not Null
Drop Procedure dbo.Pro_GetUserPersionInfo
Go
Create Procedure dbo.Pro_GetUserPersionInfo
(
	 @LoginName NVarchar(Max)='' --员工名字
	,@RoleID NVarchar(Max) ='00000000-0000-0000-0000-000000000000'--角色ID
	,@KeyWord NVarchar(Max) ='' --模糊查询条件
	,@PageIndex As Int=1
    ,@PageSize  As Int= 20
)
As 
Begin 
	Begin --条件设计
		Declare @Fileter1 NVarchar(Max)=' And 1=1' --角色
			   ,@Fileter2 NVarchar(Max)=' And 1=1' --ResultData数据查询条件
			   ,@Fileter3 NVarchar(Max)=' And 1=1' --组织架构
			   ,@Type_Role NVarchar(10)='Left'
			   ,@Type_Org NVarchar(10)='Left'; 
		If(@LoginName!='')
		Begin 
			Set @Fileter1+=' And A.LoginName='''+@LoginName+'''';
			Set @Fileter2+=' And A.LoginName='''+@LoginName+'''';
			Set @Fileter3+=' And A.LoginName='''+@LoginName+'''';

		End 
		If(@RoleID!='00000000-0000-0000-0000-000000000000')
		Begin 
			Set @Type_Role='Inner';
			Set @Fileter1+=' And C.ID='''+@RoleID+'''';
		End 
		If(@KeyWord!='')
		Begin 
			Set @Fileter1+=' And CharIndex('''+@KeyWord+''',(IsNull(A.[LoginName],'''')+IsNull(A.[EmployeeName],'''')+IsNull(A.[JobTitle],'''')+IsNull(C.[CnName],'''')))>0';
			Set @fileter2+=' And CharIndex('''+@KeyWord+''',(IsNull(A.[LoginName],'''')+IsNull(A.[EmployeeName],'''')+IsNull(A.[JobTitle],'''')))>0';
			Set @fileter3+=' And CharIndex('''+@KeyWord+''',(IsNull(A.[LoginName],'''')+IsNull(A.[EmployeeName],'''')+IsNull(A.[JobTitle],'''')+IsNull(B.[DeptFullName],'''')+IsNull(C.[RoleName],'''')+IsNull(D.[OrgName],'''')))>0 ';
		End 

	End 
	Begin --用户和角色

	--创建用户_角色的临时表
	If Object_Id('tempdb..#User_RoleInfo') Is Not Null
    Drop Table #User_RoleInfo
	Create Table #User_RoleInfo
	(
		LoginName NVarchar(Max),--员工ID
		RoleName NVarchar(Max)--角色名称集合
	)


	If Object_Id('tempdb..#User_RoleInfo_Templ') Is Not Null
    Drop Table #User_RoleInfo_Templ
	Create Table #User_RoleInfo_Templ
	(
		LoginName NVarchar(Max),
		CnName NVarchar(50)
	)
	Exec('
		Insert Into [#User_RoleInfo_Templ]
				( [LoginName], [CnName] )
		Select A.[LoginName],C.[CnName]
		From [dbo].[Employee] As A
		Inner Join [dbo].[S_Role_User] As B On A.[LoginName]=B.[LoginName] And B.[IsDeleted]=0
		Inner Join  [dbo].[S_Role] As C On B.[RoleID]=C.[ID] And C.[IsDeleted]=0
		Where A.[IsDeleted]=0'+@Fileter1+';

		Insert Into [#User_RoleInfo]
	        ( [LoginName], [RoleName] )
		Select A.[LoginName],RoleName=(Stuff((Select '',''+[CnName] From [#User_RoleInfo_Templ] Where LoginName=A.LoginName For Xml Path('''')),1,1,'''')) From [#User_RoleInfo_Templ] As A Group By A.LoginName')

	
	End 

	Begin --用户和组织架构
	If Object_Id('tempdb..#User_OrgInfo') Is Not Null
    Drop Table #User_OrgInfo
	Create Table #User_OrgInfo
	(
		LoginName NVarchar(Max),--员工ID
		OrgName NVarchar(Max)--组织架构名称集合
	)
	If Object_Id('tempdb..#User_OrgInfo_Templ') Is Not Null
    Drop Table #User_OrgInfo_Templ
	Create Table #User_OrgInfo_Templ
	(
		LoginName NVarchar(Max),--员工ID
		CnName NVarchar(Max)--组织架构名称
	)
	Exec('
		With getSystemData
    	As 
    	(
    		Select A.*,B.[LoginName],C.[EmployeeName],C.[JobTitle] From [dbo].[S_Organizational] As A 
    		Inner Join [S_Org_User] As B On A.[ID]=B.[CompanyID] And B.[IsDeleted]=0
    		Inner Join [dbo].[Employee] As C On B.[LoginName]=C.[LoginName] And C.[IsDeleted]=0
    		Union All
    		 Select A.*,B.[LoginName],B.[EmployeeName],B.[JobTitle] From [dbo].[S_Organizational] As A
    		Inner Join [getSystemData] As B On A.[ID]=B.[ParentID] And B.[IsDeleted]=0
    		Where A.[IsDeleted]=0
    	)
    	Insert Into [#User_OrgInfo_Templ]
    	 ( [LoginName], [CnName] )
    	Select A.[LoginName],A.[CnName] From [getSystemData] As A 
    	Inner Join [dbo].[C_System] As B On A.[ID]=B.[ID] And B.[IsDeleted]=0
    	Where A.[Level]=2 '+@fileter2+'
    	Group By A.[LoginName],A.[CnName] Order By A.[LoginName]')

	Insert Into [#User_OrgInfo]
	        ( [LoginName], [OrgName] )
	Select A.[LoginName],OrgName=(Stuff((Select ','+[CnName] From [#User_OrgInfo_Templ] Where [LoginName]=A.[LoginName] For Xml Path('')),1,1,'')) From [#User_OrgInfo_Templ] As A Group By A.[LoginName]

	End 

	Begin --数据显示

	If Object_Id('tempdb..#resultData') Is Not Null
    Drop Table #resultData
	Create Table #resultData
	(
		RowID Int Identity(1,1)
	   ,EmployeeName NVarchar(Max)
	   ,LoginName NVarchar(Max)
	   ,JobTitle NVarchar(Max)
	   ,DeptFullName NVarchar(Max)
	   ,RoleName NVarchar(Max)
	   ,OrgName NVarchar(Max)
	)
	Exec('
		Insert Into [#resultData]
				( [EmployeeName] ,
				  [LoginName] ,
				  [JobTitle] ,
				  [DeptFullName] ,
				  [RoleName] ,
				  [OrgName]
				)
		Select A.[EmployeeName],A.[LoginName],A.[JobTitle],B.[DeptFullName],C.[RoleName],D.[OrgName]
		From [dbo].[Employee] As A
		Left Join [dbo].[Dept] As B On A.[DeptID]=B.[ID] And B.[IsDeleted]=0
		'+@Type_Role+' Join [#User_RoleInfo] As C On A.[LoginName]=C.[LoginName]
		'+@Type_Org+' Join [#User_OrgInfo] As D On A.[LoginName]=D.[LoginName]
		Where A.[IsDeleted]=0'+@Fileter3)

	---第一页
	If (@PageIndex=1)
	Begin 
		Select A.[LoginName],A.[EmployeeName],A.[JobTitle],A.[DeptFullName],A.[RoleName],A.[OrgName] From [#resultData] As A
		Where @PageIndex = 1
                      And A.[RowID] <= @PageIndex * @PageSize
                Order By [A].[EmployeeName] Asc,A.[LoginName] Asc 
	End 
	---其他页
	Else
    Begin 
		Select A.[LoginName],A.[EmployeeName],A.[JobTitle],A.[DeptFullName],A.[RoleName],A.[OrgName] From [#resultData] As A
			Where @PageIndex != 1
                      And (
                            @PageSize * (@PageIndex - 1) < A.[RowID]
                            And A.[RowID] <= @PageSize * @PageIndex
                          )
					Order By [A].[EmployeeName] Asc,A.[LoginName] Asc 
	End 

		--一共多少记录      
    Select Count(1) As TotalCount From [#resultData];

	End

	Begin 	--删除临时表
		If Object_Id('tempdb..#User_RoleInfo') Is Not Null
		Drop Table #User_RoleInfo
		If Object_Id('tempdb..#User_RoleInfo_Templ') Is Not Null
		Drop Table #User_RoleInfo_Templ
		If Object_Id('tempdb..#User_OrgInfo') Is Not Null
		Drop Table #User_OrgInfo
		If Object_Id('tempdb..#User_OrgInfo_Templ') Is Not Null
	    Drop Table #User_OrgInfo_Templ
		If Object_Id('tempdb..#resultData') Is Not Null
		Drop Table #resultData
	End 
End 
