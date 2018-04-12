--�˵���
Create Table S_Menu
	(
		ID			 UniqueIdentifier Primary Key Not Null --ID
	   ,CnName		 NVarchar(50)	  Not Null			   --��������
	   ,EnName		 NVarchar(50)	  Null				   --Ӣ������
	   ,ParentMenuID UniqueIdentifier Null				   --����ID
	   ,[Level]		 Int			  Not Null			   --�˵�����
	   ,[Sequence]	 Int			  Not Null			   --���
	   ,[Url]		 NVarchar(100)	  Not Null			   --��ַ
	   ,ResourceKey	 NVarchar(100)	  Not Null
	   ,IsDelete	 Bit			  Null				   --�Ƿ�ɾ��
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);
--Ȩ�ޱ�
Create Table S_RolePermissions
	(
		ID			 UniqueIdentifier Not Null Primary Key --ID
	   ,RoleID		 UniqueIdentifier Not Null			   --��ɫID
	   ,MenuID		 UniqueIdentifier Not Null			   --�˵�ID
	   ,IsDelete	 Bit			  Null				   --�Ƿ�ɾ��
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);
--�û���ɫ��
Create Table S_Role_User
	(
		ID			 UniqueIdentifier Not Null Primary Key --ID
	   ,RoleID		 UniqueIdentifier Null				   --��ɫID
	   ,EmployeeID	 Int			  Null				   --�û��˺�
	   ,IsDelete	 Bit			  Null				   --�Ƿ�ɾ��
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);

--��ɫ��
Create Table S_Role
	(
		ID			  UniqueIdentifier Not Null Primary Key --ID
	   ,CnName		  NVarchar(50)	   Null					--��������
	   ,EnName		  NVarchar(50)	   Null					--Ӣ������
	   ,[Description] NVarchar(Max)	   Null					--��ע
	   ,IsDelete	  Bit			   Null					--�Ƿ�ɾ��
	   ,CreateTime	  DateTime		   Not Null
	   ,CreatorName	  NVarchar(100)	   Null
	   ,CreateUserID  Int			   Not Null
	   ,ModifierName  NVarchar(100)	   Null
	   ,ModifyTime	  DateTime		   Null
	);
---����Ȩ��
--��֯�ܹ�
Create Table S_Organizational
	(
		ID			 UniqueIdentifier Primary Key Not Null --����ID[�����SystemID ��Ŀ��CompanyID
	   ,CnName		 NVarchar(60)	  Not Null			   --����
	   ,Code		 NVarchar(60)	  Null				   --����
	   ,ParentID	 UniqueIdentifier Not Null			   --����ID
	   ,[Level]		 Int			  Not Null			   --�ȼ� 1��� 2���� 3���� 4��Ŀ
	   ,IsDelete	 Bit			  Null				   --�Ƿ�ɾ��
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);
--�û�����֯�ܹ���Ȩ�ޱ�
Create Table S_Org_User
	(
		ID			 UniqueIdentifier Primary Key Not Null --��������
	   ,SystemID	 UniqueIdentifier Not Null
	   ,EmployeeID	 Int			  Null				   --�û�ID
	   ,CompanyID	 UniqueIdentifier Not Null			   --��֯�ܹ�
	   ,IsDelete	 Bit			  Null				   --�Ƿ�ɾ��
	   ,CreateTime	 DateTime		  Not Null
	   ,CreatorName	 NVarchar(100)	  Null
	   ,CreateUserID Int			  Not Null
	   ,ModifierName NVarchar(100)	  Null
	   ,ModifyTime	 DateTime		  Null
	);
