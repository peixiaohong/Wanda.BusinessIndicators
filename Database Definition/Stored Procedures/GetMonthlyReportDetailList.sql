If Object_Id('dbo.GetMonthlyReportDetailList') Is Not Null
Drop Procedure dbo.GetMonthlyReportDetailList
Go

Create   PROCEDURE [dbo].[GetMonthlyReportDetailList] 
@SystemID UNIQUEIDENTIFIER,
@FinYear INT,
@FinMonth INT,
@TargetPlanID UNIQUEIDENTIFIER,
@SystemBatchID UNIQUEIDENTIFIER

AS
BEGIN

DECLARE @IsHaveArea INT--0无区域 大于0有区域（表示月报可分区域分批上传）
Select @IsHaveArea=count(1) From [dbo].[S_Organizational] Where [ParentID]=@SystemID  And [SystemID]=@SystemID And [IsCompany]=0

IF @IsHaveArea>0
--获取最新批次ID,根据最新批次ID查找MonthlyReportID,然后关联出所有的月报明细
	BEGIN 
	IF @SystemBatchID IS NULL OR @SystemBatchID='00000000-0000-0000-0000-000000000000'
		SET  @SystemBatchID=(SELECT TOP 1 SystemBatchID FROM B_MonthlyReport WHERE SystemID=@SystemID AND FinYear=@FinYear AND FinMonth=@FinMonth AND TargetPlanID=@TargetPlanID ORDER BY CreateTime DESC);
	  
	  SELECT a.[ID]
      , a.[SystemID]
      ,a.[FinYear]
      ,a.[FinMonth]
      ,a.[TargetID]
      ,a.[CompanyID]
      ,a.[TargetPlanID]
      ,a.[OPlanAmmount]
      ,a.[OActualAmmount]
      ,a.[OActualRate]
      ,a.[ODisplayRate]
      ,a.[NPlanAmmount]
      ,a.[NActualAmmount]
      ,a.[NActualRate]
      ,a.[NDisplayRate]
      ,a.[OAccumulativePlanAmmount]
      ,a.[OAccumulativeActualAmmount]
      ,a.[OAccumulativeActualRate]
      ,a.[OAcccumulativeDisplayRate]
      ,a.[NAccumulativePlanAmmount]
      ,a.[NAccumulativeActualAmmount]
      ,a.[NAccumulativeActualRate]
      ,a.[NAccumulativeDisplayRate]
      ,a.[IsMissTarget]
      ,a.[Counter]
      ,a.[FirstMissTargetDate]
      ,a.[PromissDate]
      ,(SELECT TOP 1 CASE WHEN PromissDate IS NOT NULL THEN PromissDate ELSE
	  CurrentMonthCommitDate END FROM  dbo.A_MonthlyReportDetail WHERE SystemID=a.[SystemID] AND TargetID=a.[TargetID]
					AND FinMonth=a.[FinMonth] AND FinYear=a.[FinYear] AND  CompanyID=a.CompanyID AND (PromissDate IS NOT NULL OR CurrentMonthCommitDate IS NOT NULL))  [CommitDate]
      ,a.[MIssTargetReason]
      ,a.[MIssTargetDescription]
	  ,a.CurrentMIssTargetReason
	  ,a.CurrentMIssTargetDescription
      ,a.[ReturnType]
      ,a.[ModifierID]
      ,a.[ModifierName]
      ,a.[ModifyTime]
      ,a.[CreatorID]
      ,a.[CreateTime]
      ,a.[CreatorName]
      ,a.[IsDeleted]
      ,a.[IsMissTargetCurrent]
      ,a.[NDifference]
      ,a.[NAccumulativeDifference]
      ,a.[ODifference]
      ,a.[OAccumulativeDifference]
      ,a.[MonthlyReportID]
      ,a.[MeasureRate]
      ,a.[ReturnDescription]
      ,a.[Display]
      ,a.[CompanyProperty1]
      ,a.[IsCommitDate]
      ,a.[CommitReason]
      ,a.[CurrentMonthCommitDate]
      ,a.[CurrentMonthCommitReason]
      ,a.[NewCounter]
      ,a.[ReturnType_Sub]
      ,a.[CompanyProperty]
	   ,( SELECT  SystemName  FROM  dbo.C_System WHERE id = a.SystemID  AND  GETDATE()  BETWEEN VersionStart AND VersionEnd AND IsDeleted = 0)  AS SystemName
	   ,( SELECT CompanyName FROM dbo.C_Company WHERE id = a.CompanyID   ) AS CompanyName
	   ,(SELECT TargetName FROM  dbo.C_Target WHERE id= a.TargetID AND SystemID =a.SystemID  AND  VersionStart <= GETDATE() AND GETDATE() < VersionEnd   AND IsDeleted =0   )AS   TargetName
	   ,(SELECT TargetType FROM  dbo.C_Target WHERE id= a.TargetID AND SystemID =a.SystemID AND  VersionStart <= GETDATE() AND GETDATE() < VersionEnd    AND IsDeleted =0 ) AS TargetType
	   ,(SELECT sum(Target) FROM A_TargetPlanDetail WHERE SystemID=a.SystemID AND CompanyID=a.CompanyID AND FinYear=a.FinYear AND TargetID=a.TargetID AND TargetPlanID=a.TargetPlanID  AND IsDeleted=0 GROUP BY SystemID,CompanyID,FinYear,TargetID,TargetPlanID ) AS NPlanAmmountByYear--全年指标	   
	  FROM [dbo].[B_MonthlyReportDetail] a INNER JOIN B_MonthlyReport b ON a.MonthlyReportID=b.ID  
	  WHERE b.SystemBatchID=@SystemBatchID AND B.FinYear=@FinYear AND B.FinMonth=@FinMonth   AND B.TargetPlanID=@TargetPlanID
	END 
ELSE 
	BEGIN 
	DECLARE @lastMonthlyReportID UNIQUEIDENTIFIER --最后一版月报ID
	SET @lastMonthlyReportID=(SELECT TOP 1 ID FROM B_MonthlyReport WHERE  SystemID=@SystemID AND FinYear=@FinYear AND FinMonth=@FinMonth AND TargetPlanID=@TargetPlanID ORDER BY CreateTime DESC)
		SELECT [ID]
      ,[SystemID]
      ,[FinYear]
      ,[FinMonth]
      ,[TargetID]
      ,[CompanyID]
      ,[TargetPlanID]
      ,[OPlanAmmount]
      ,[OActualAmmount]
      ,[OActualRate]
      ,[ODisplayRate]
      ,[NPlanAmmount]
      ,[NActualAmmount]
      ,[NActualRate]
      ,[NDisplayRate]
      ,[OAccumulativePlanAmmount]
      ,[OAccumulativeActualAmmount]
      ,[OAccumulativeActualRate]
      ,[OAcccumulativeDisplayRate]
      ,[NAccumulativePlanAmmount]
      ,[NAccumulativeActualAmmount]
      ,[NAccumulativeActualRate]
      ,[NAccumulativeDisplayRate]
      ,[IsMissTarget]
      ,[Counter]
      ,[FirstMissTargetDate]
      ,[PromissDate]
      ,(SELECT TOP 1 CASE WHEN PromissDate IS NOT NULL THEN PromissDate ELSE
	  CurrentMonthCommitDate END FROM  dbo.A_MonthlyReportDetail WHERE SystemID=b.SystemID AND TargetID=b.TargetID
					AND FinMonth=b.FinMonth AND FinYear=b.FinYear AND  CompanyID=b.CompanyID AND (PromissDate IS NOT NULL OR CurrentMonthCommitDate IS NOT NULL))  [CommitDate]
      ,[MIssTargetReason]
      ,[MIssTargetDescription]
	  ,CurrentMIssTargetReason
	  ,CurrentMIssTargetDescription
      ,[ReturnType]
      ,[ModifierID]
      ,[ModifierName]
      ,[ModifyTime]
      ,[CreatorID]
      ,[CreateTime]
      ,[CreatorName]
      ,[IsDeleted]
      ,[IsMissTargetCurrent]
      ,[NDifference]
      ,[NAccumulativeDifference]
      ,[ODifference]
      ,[OAccumulativeDifference]
      ,[MonthlyReportID]
      ,[MeasureRate]
      ,[ReturnDescription]
      ,[Display]
      ,[CompanyProperty1]
      ,[IsCommitDate]
      ,[CommitReason]
      ,[CurrentMonthCommitDate]
      ,[CurrentMonthCommitReason]
      ,[NewCounter]
      ,[ReturnType_Sub]
      ,[CompanyProperty]
	   ,( SELECT  SystemName  FROM  dbo.C_System WHERE id = SystemID  AND  GETDATE()  BETWEEN VersionStart AND VersionEnd AND IsDeleted = 0)  AS SystemName
	   ,( SELECT CompanyName FROM dbo.C_Company WHERE id = CompanyID   ) AS CompanyName
	   ,(SELECT TargetName FROM  dbo.C_Target WHERE id= TargetID AND SystemID =SystemID  AND  VersionStart <= GETDATE() AND GETDATE() < VersionEnd   AND IsDeleted =0   )AS   TargetName
	   ,(SELECT TargetType FROM  dbo.C_Target WHERE id= TargetID AND SystemID =SystemID AND  VersionStart <= GETDATE() AND GETDATE() < VersionEnd    AND IsDeleted =0 ) AS TargetType
	   ,(SELECT sum(Target) FROM A_TargetPlanDetail p WHERE p.SystemID=b.SystemID AND p.CompanyID=b.CompanyID AND p.FinYear=b.FinYear AND p.TargetID=b.TargetID AND p.TargetPlanID=b.TargetPlanID  AND p.IsDeleted=0 GROUP BY SystemID,CompanyID,FinYear,TargetID,TargetPlanID ) AS NPlanAmmountByYear--全年指标	   
	  FROM B_MonthlyReportDetail b
	  WHERE MonthlyReportID=@lastMonthlyReportID
	END 
END

GO

