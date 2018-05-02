


--ÇøÓòID
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'B_MonthlyReport'
                        AND T2.name = 'AreaID' )
    BEGIN
        ALTER TABLE B_MonthlyReport
        ADD  AreaID UNIQUEIDENTIFIER;
    END;

	
	--Ä¬ÈÏ°æ±¾×´Ì¬
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'B_MonthlyReport'
                        AND T2.name = 'DefaultVersionStatus' )
    BEGIN
        ALTER TABLE B_MonthlyReport
        ADD  DefaultVersionStatus INT  NOT NULL DEFAULT 1;
    END;