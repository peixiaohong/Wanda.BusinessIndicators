  
  IF OBJECT_ID('GetMonthlyReportDetail_ByAToB') IS NOT NULL
    DROP PROC GetMonthlyReportDetail_ByAToB;
  GO

-- =============================================  
-- Author:  <Author,,dubiao>  
-- Create date: <Create Date,2017/11/14,>  
-- Description: <Description,从A表中获取数据，插入B表中,>  
-- =============================================  
  CREATE PROCEDURE [dbo].[GetMonthlyReportDetail_ByAToB]
    @FinYear INT ,
    @FinMonth INT ,
    @SystemID UNIQUEIDENTIFIER ,
    @MonthlyReportID UNIQUEIDENTIFIER ,
    @TargetPlanID UNIQUEIDENTIFIER
  AS
    BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
        SET NOCOUNT ON;  
  
--DECLARE @MonthlyReportID  UNIQUEIDENTIFIER ;   
--SET @MonthlyReportID ='807D2F7C-FC62-45F4-8CCF-156FCD575A31';  
  
        INSERT  INTO [dbo].[B_MonthlyReportDetail]
                ( [ID] ,
                  [SystemID] ,
                  [FinYear] ,
                  [FinMonth] ,
                  [TargetID] ,
                  [CompanyID] ,
                  [TargetPlanID] ,
                  [OPlanAmmount] ,
                  [OActualAmmount] ,
                  [OActualRate] ,
                  [ODisplayRate] ,
                  [NPlanAmmount] ,
                  [NActualAmmount] ,
                  [NActualRate] ,
                  [NDisplayRate] ,
                  [OAccumulativePlanAmmount] ,
                  [OAccumulativeActualAmmount] ,
                  [OAccumulativeActualRate] ,
                  [OAcccumulativeDisplayRate] ,
                  [NAccumulativePlanAmmount] ,
                  [NAccumulativeActualAmmount] ,
                  [NAccumulativeActualRate] ,
                  [NAccumulativeDisplayRate] ,
                  [IsMissTarget] ,
                  [Counter] ,
                  [FirstMissTargetDate] ,
                  [PromissDate] ,
                  [CommitDate] ,
                  [MIssTargetReason] ,
                  [MIssTargetDescription] ,
                  [ReturnType] ,
                  [ModifierID] ,
                  [ModifierName] ,
                  [ModifyTime] ,
                  [CreatorID] ,
                  [CreateTime] ,
                  [CreatorName] ,
                  [IsDeleted] ,
                  [IsMissTargetCurrent] ,
                  [NDifference] ,
                  [NAccumulativeDifference] ,
                  [ODifference] ,
                  [OAccumulativeDifference] ,
                  [MonthlyReportID] ,
                  [MeasureRate] ,
                  [ReturnDescription] ,
                  [Display] ,
                  [CompanyProperty1] ,
                  [IsCommitDate] ,
                  [CommitReason] ,
                  [CurrentMonthCommitDate] ,
                  [CurrentMonthCommitReason] ,
                  [NewCounter] ,
                  [ReturnType_Sub] ,
                  [CompanyProperty] ,
                  [CurrentMIssTargetReason] ,
                  [CurrentMIssTargetDescription]
                )
                SELECT  NEWID() AS [ID] ,
                        [SystemID] ,
                        [FinYear] ,
                        [FinMonth] ,
                        [TargetID] ,
                        [CompanyID] ,
                        [TargetPlanID] ,
                        [OPlanAmmount] ,
                        [OActualAmmount] ,
                        [OActualRate] ,
                        [ODisplayRate] ,
                        [NPlanAmmount] ,
                        [NActualAmmount] ,
                        [NActualRate] ,
                        [NDisplayRate] ,
                        [OAccumulativePlanAmmount] ,
                        [OAccumulativeActualAmmount] ,
                        [OAccumulativeActualRate] ,
                        [OAcccumulativeDisplayRate] ,
                        [NAccumulativePlanAmmount] ,
                        [NAccumulativeActualAmmount] ,
                        [NAccumulativeActualRate] ,
                        [NAccumulativeDisplayRate] ,
                        [IsMissTarget] ,
                        [Counter] ,
                        [FirstMissTargetDate] ,
                        [PromissDate] ,
                        [CommitDate] ,
                        [MIssTargetReason] ,
                        [MIssTargetDescription] ,
                        [ReturnType] ,
                        [ModifierID] ,
                        [ModifierName] ,
                        [ModifyTime] ,
                        [CreatorID] ,
                        [CreateTime] ,
                        [CreatorName] ,
                        [IsDeleted] ,
                        [IsMissTargetCurrent] ,
                        [NDifference] ,
                        [NAccumulativeDifference] ,
                        [ODifference] ,
                        [OAccumulativeDifference] ,
                        @MonthlyReportID AS [MonthlyReportID] ,
                        [MeasureRate] ,
                        [ReturnDescription] ,
                        [Display] ,
                        [CompanyProperty1] ,
                        [IsCommitDate] ,
                        [CommitReason] ,
                        [CurrentMonthCommitDate] ,
                        [CurrentMonthCommitReason] ,
                        [NewCounter] ,
                        [ReturnType_Sub] ,
                        [CompanyProperty] ,
                        [CurrentMIssTargetReason] ,
                        [CurrentMIssTargetDescription]
                FROM    dbo.A_MonthlyReportDetail
                WHERE   FinYear = @FinYear
                        AND FinMonth = @FinMonth
                        AND SystemID = @SystemID
                        AND TargetPlanID = @TargetPlanID;
  
  
  
        SELECT  *
        FROM    [B_MonthlyReportDetail]
        WHERE   MonthlyReportID = @MonthlyReportID;  
  
    END;  
  
  
  
  