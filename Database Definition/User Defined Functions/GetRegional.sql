Create Function GetRegional(@OrgParentID UniqueIdentifier)
returns @LoginNames Table
(
	LoginName nvarchar(Max)
)
As
Begin

	Declare  @SubID UniqueIdentifier;
	Set @SubID=(Select Top 1 ID From [dbo].[S_Organizational] Where [ParentID]=@OrgParentID And [IsDeleted]=0)

	 Insert Into @LoginNames
	 Select [LoginName] From [dbo].[S_Org_User] Where [CompanyID]=@SubID
	 Return 
End 