




--区域ID
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'A_MonthlyReport'
                        AND T2.name = 'AreaID' )
    BEGIN
        ALTER TABLE A_MonthlyReport
        ADD  AreaID UNIQUEIDENTIFIER;
    END;

--分解指标ID
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'A_MonthlyReport'
                        AND T2.name = 'TargetPlanID' )
    BEGIN
        ALTER TABLE A_MonthlyReport
        ADD  TargetPlanID UNIQUEIDENTIFIER;
    END;	
	