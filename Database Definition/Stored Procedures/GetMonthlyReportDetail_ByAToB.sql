  
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
    @AreaID UNIQUEIDENTIFIER ,
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
                        A.[SystemID] ,
                        A.[FinYear] ,
                        A.[FinMonth] ,
                        A.[TargetID] ,
                        A.[CompanyID] ,
                        A.[TargetPlanID] ,
                        A.[OPlanAmmount] ,
                        A.[OActualAmmount] ,
                        A.[OActualRate] ,
                        A.[ODisplayRate] ,
                        A.[NPlanAmmount] ,
                        A.[NActualAmmount] ,
                        A.[NActualRate] ,
                        A.[NDisplayRate] ,
                        A.[OAccumulativePlanAmmount] ,
                        A.[OAccumulativeActualAmmount] ,
                        A.[OAccumulativeActualRate] ,
                        A.[OAcccumulativeDisplayRate] ,
                        A.[NAccumulativePlanAmmount] ,
                        A.[NAccumulativeActualAmmount] ,
                        A.[NAccumulativeActualRate] ,
                        A.[NAccumulativeDisplayRate] ,
                        A.[IsMissTarget] ,
                        A.[Counter] ,
                        A.[FirstMissTargetDate] ,
                        A.[PromissDate] ,
                        A.[CommitDate] ,
                        A.[MIssTargetReason] ,
                        A.[MIssTargetDescription] ,
                        A.[ReturnType] ,
                        A.[ModifierID] ,
                        A.[ModifierName] ,
                        A.[ModifyTime] ,
                        A.[CreatorID] ,
                        A.[CreateTime] ,
                        A.[CreatorName] ,
                        A.[IsDeleted] ,
                        A.[IsMissTargetCurrent] ,
                        A.[NDifference] ,
                        A.[NAccumulativeDifference] ,
                        A.[ODifference] ,
                        A.[OAccumulativeDifference] ,
                        @MonthlyReportID AS [MonthlyReportID] ,
                        A.[MeasureRate] ,
                        A.[ReturnDescription] ,
                        A.[Display] ,
                        A.[CompanyProperty1] ,
                        A.[IsCommitDate] ,
                        A.[CommitReason] ,
                        A.[CurrentMonthCommitDate] ,
                        A.[CurrentMonthCommitReason] ,
                        A.[NewCounter] ,
                        A.[ReturnType_Sub] ,
                        A.[CompanyProperty] ,
                        A.[CurrentMIssTargetReason] ,
                        A.[CurrentMIssTargetDescription]
                FROM    dbo.A_MonthlyReportDetail AS A
                        LEFT JOIN dbo.A_MonthlyReport AS B ON B.ID = A.MonthlyReportID
                WHERE   A.FinYear = @FinYear
                        AND A.FinMonth = @FinMonth
                        AND A.SystemID = @SystemID
                        AND A.TargetPlanID = @TargetPlanID
                        AND ISNULL(B.AreaID,'00000000-0000-0000-0000-000000000000') = @AreaID;  
    
    
    
        SELECT  *
        FROM    [B_MonthlyReportDetail]
        WHERE   MonthlyReportID = @MonthlyReportID;    
    
    END;    
    
    
    
  
  
  
  