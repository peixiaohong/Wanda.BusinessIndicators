--菜单表
Create Table S_Menu
	(
		ID			 UniqueIdentifier Primary Key Not Null --ID
	   ,CnName		 NVarchar(50)	  Not Null			   --中文名称
	   ,EnName		 NVarchar(50)	  Null				   --英文名称
	   ,ParentMenuID UniqueIdentifier Null				   --父级ID
	   ,[Level]		 Int			  Not Null			   --菜单级别
	   ,[Sequence]	 Int			  Not Null			   --序号
	   ,[Url]		 NVarchar(100)	  Not Null			   --地址
	   ,ResourceKey	 NVarchar(100)	  Not Null
	   ,IsDelete	 Bit			  Null				   --是否删除
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);
--权限表
Create Table S_RolePermissions
	(
		ID			 UniqueIdentifier Not Null Primary Key --ID
	   ,RoleID		 UniqueIdentifier Not Null			   --角色ID
	   ,MenuID		 UniqueIdentifier Not Null			   --菜单ID
	   ,IsDelete	 Bit			  Null				   --是否删除
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);
--用户角色表
Create Table S_Role_User
	(
		ID			 UniqueIdentifier Not Null Primary Key --ID
	   ,RoleID		 UniqueIdentifier Null				   --角色ID
	   ,EmployeeID	 Int			  Null				   --用户账号
	   ,IsDelete	 Bit			  Null				   --是否删除
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);

--角色表
Create Table S_Role
	(
		ID			  UniqueIdentifier Not Null Primary Key --ID
	   ,CnName		  NVarchar(50)	   Null					--中文名称
	   ,EnName		  NVarchar(50)	   Null					--英文名称
	   ,[Description] NVarchar(Max)	   Null					--备注
	   ,IsDelete	  Bit			   Null					--是否删除
	   ,CreateTime	  DateTime		   Not Null
	   ,CreatorName	  NVarchar(100)	   Null
	   ,CreateUserID  Int			   Not Null
	   ,ModifierName  NVarchar(100)	   Null
	   ,ModifyTime	  DateTime		   Null
	);
---数据权限
--组织架构
Create Table S_Organizational
	(
		ID			 UniqueIdentifier Primary Key Not Null --主键ID[板块是SystemID 项目是CompanyID
	   ,CnName		 NVarchar(60)	  Not Null			   --名称
	   ,Code		 NVarchar(60)	  Null				   --编码
	   ,ParentID	 UniqueIdentifier Not Null			   --父级ID
	   ,[Level]		 Int			  Not Null			   --等级 1板块 2大区 3区域 4项目
	   ,IsDelete	 Bit			  Null				   --是否删除
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);
--用户和组织架构的权限表
Create Table S_Org_User
	(
		ID			 UniqueIdentifier Primary Key Not Null --主键编码
	   ,SystemID	 UniqueIdentifier Not Null
	   ,EmployeeID	 Int			  Null				   --用户ID
	   ,CompanyID	 UniqueIdentifier Not Null			   --组织架构
	   ,IsDelete	 Bit			  Null				   --是否删除
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);
