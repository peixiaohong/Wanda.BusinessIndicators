If Object_Id('dbo.Pro_CompanyInfo') Is Not Null
Drop Procedure dbo.Pro_CompanyInfo
Go
Create Procedure dbo.Pro_CompanyInfo
(
	 @SystemID NVarchar(38) ='00000000-0000-0000-0000-000000000000'--角色ID
	,@KeyWord NVarchar(Max) ='' --模糊查询条件
	,@PageIndex As Int=1
    ,@PageSize  As Int= 20
)
As 
Begin 
Declare @Fileter NVarchar(Max)='';
If @SystemID!='00000000-0000-0000-0000-000000000000'
	Set @Fileter+=' And A.[SystemID]='''+@SystemID+'''';
If @KeyWord!=''
	Set @Fileter+=' And A.CompanyName like ''%'+@KeyWord+'%''';
If Object_Id('tempdb..#resultData') Is Not Null
Drop Table #resultData
Create Table #resultData
(
	RowID Int Identity(1,1) Primary Key,
	SystemID UniqueIdentifier Not Null ,
	CompanyID UniqueIdentifier Not Null,
    CompanyName NVarchar(Max)
)

Exec('
	Insert Into [#resultData]
			( [SystemID] ,
			  [CompanyID] ,
			  [CompanyName]
			)
	Select A.[SystemID],A.[ID],A.[CompanyName] From [dbo].[C_Company] As A
	Left Join [dbo].[S_Organizational] As B  On [A].[ID]=B.[ID]  And B.[IsDeleted]=0
	Where  B.[ID] Is Null  And A.[IsDeleted]=0'+@Fileter)

---第一页
	If (@PageIndex=1)
	Begin 
		Select A.[SystemID],A.[CompanyID] As ID,A.[CompanyName] From [#resultData] As A
		Where @PageIndex = 1
                      And A.[RowID] <= @PageIndex * @PageSize
                Order By [A].[CompanyName] Asc
	End 
	---其他页
	Else
    Begin 
		Select A.[SystemID],A.[CompanyID] As ID,A.[CompanyName] From [#resultData] As A
			Where @PageIndex != 1
                      And (
                            @PageSize * (@PageIndex - 1) < A.[RowID]
                            And A.[RowID] <= @PageSize * @PageIndex
                          )
					Order By [A].[CompanyName] Asc
	End 
		--一共多少记录      
    Select Count(1) As TotalCount From [#resultData];

	If Object_Id('tempdb..#resultData') Is Not Null
	Drop Table #resultData

End 
