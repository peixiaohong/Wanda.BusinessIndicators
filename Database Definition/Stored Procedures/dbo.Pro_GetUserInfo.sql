
If Object_Id('dbo.Pro_GetUserInfo') Is Not Null
Drop Procedure dbo.Pro_GetUserInfo
Go
Create Procedure dbo.Pro_GetUserInfo
(
	 @RoleID NVarchar(Max) ='00000000-0000-0000-0000-000000000000'--角色ID
	,@KeyWord NVarchar(Max) ='' --模糊查询条件
	,@PageIndex As Int=1
    ,@PageSize  As Int= 20
)
As 
Begin
	Declare @Filter NVarchar(Max)='';
	If(@KeyWord!='')
	Set @Filter+=' And CharIndex('''+@KeyWord+''',(IsNull(A.[LoginName],'''')+IsNull(A.[EmployeeName],'''')+IsNull(A.[JobTitle],'''')+IsNull(B.[DeptFullName],'''')))>0 ';

	If Object_Id('tempdb..#resultData') Is Not Null
    Drop Table #resultData
	Create Table #resultData
	(
		RowID Int Identity(1,1)
	   ,EmployeeName NVarchar(Max)
	   ,LoginName NVarchar(Max)
	   ,JobTitle NVarchar(Max)
	   ,DeptFullName NVarchar(Max)
	   ,IsChecked Int
	)
	Exec('Insert Into [#resultData]
	        ( [EmployeeName] ,
	          [LoginName] ,
	          [JobTitle] ,
	          [DeptFullName] ,
	          [IsChecked]
	        )
		Select[EmployeeName],[LoginName],[JobTitle],B.[DeptFullName],
		Case When '''+@RoleID+'''='''' Or '''+@RoleID+'''=''00000000-0000-0000-0000-000000000000'' Then 0
			 When (Select Count(*) From  [dbo].[S_Role_User] As C Where C.[RoleID]='''+@RoleID+''' And C.[IsDeleted]=0 And C.[LoginName]=A.LoginName)>0 Then 1
			 Else 0
			 End IsChecked
		From [dbo].[Employee] As A
		Left Join [dbo].[Dept] As B On A.[DeptID]=B.[ID] And B.[IsDeleted]=0
		Where A.[IsDeleted]=0'+@Filter)
	---第一页
	If (@PageIndex=1)
	Begin 
		Select A.[LoginName],A.[EmployeeName],A.[JobTitle],A.[DeptFullName],A.[IsChecked] From [#resultData] As A
		Where @PageIndex = 1
                      And A.[RowID] <= @PageIndex * @PageSize
                Order By [A].[EmployeeName] Asc,A.[LoginName] Asc 
	End 
	---其他页
	Else
    Begin 
		Select A.[LoginName],A.[EmployeeName],A.[JobTitle],A.[DeptFullName],A.[IsChecked] From [#resultData] As A
			Where @PageIndex != 1
                      And (
                            @PageSize * (@PageIndex - 1) < A.[RowID]
                            And A.[RowID] <= @PageSize * @PageIndex
                          )
					Order By [A].[EmployeeName] Asc,A.[LoginName] Asc 
	End 

		--一共多少记录      
    Select Count(1) As TotalCount From [#resultData];

	If Object_Id('tempdb..#resultData') Is Not Null
    Drop Table #resultData

End
