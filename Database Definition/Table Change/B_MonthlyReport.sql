


--区域ID
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'B_MonthlyReport'
                        AND T2.name = 'AreaID' )
    BEGIN
        ALTER TABLE B_MonthlyReport
        ADD  AreaID UNIQUEIDENTIFIER;
    END;

	
	--默认版本状态
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'B_MonthlyReport'
                        AND T2.name = 'DefaultVersionStatus' )
    BEGIN
        ALTER TABLE B_MonthlyReport
        ADD  DefaultVersionStatus INT  NOT NULL DEFAULT 1;
    END;
	
	
--分解指标ID
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'B_MonthlyReport'
                        AND T2.name = 'TargetPlanID' )
    BEGIN
        ALTER TABLE B_MonthlyReport
        ADD  TargetPlanID UNIQUEIDENTIFIER;
    END;