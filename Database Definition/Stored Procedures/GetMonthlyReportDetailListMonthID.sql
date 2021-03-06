If Object_Id('dbo.GetMonthlyReportDetailListMonthID') Is Not Null
Drop Procedure dbo.GetMonthlyReportDetailList
Go


Create  PROCEDURE [dbo].[GetMonthlyReportDetailListMonthID] 
@MonthlyReportID UNIQUEIDENTIFIER
AS
BEGIN
	
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
	  CurrentMonthCommitDate END FROM  dbo.A_MonthlyReportDetail WHERE SystemID=a.SystemID AND TargetID=a.TargetID
					AND FinMonth=a.FinMonth AND FinYear=a.FinYear AND  CompanyID=a.CompanyID AND (PromissDate IS NOT NULL OR CurrentMonthCommitDate IS NOT NULL))  [CommitDate]
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
	   ,(SELECT sum(Target) FROM B_TargetPlanDetail WHERE SystemID=a.SystemID AND CompanyID=a.CompanyID AND FinYear=a.FinYear AND TargetID=a.TargetID AND TargetPlanID=a.TargetPlanID  AND IsDeleted=0 GROUP BY SystemID,CompanyID,FinYear,TargetID,TargetPlanID ) AS NPlanAmmountByYear--全年指标	   
  FROM [dbo].[B_MonthlyReportDetail] a  
  WHERE a.MonthlyReportID=@MonthlyReportID

END
Go