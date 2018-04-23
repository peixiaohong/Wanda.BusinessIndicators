




--«¯”ÚID
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'A_MonthlyReport'
                        AND T2.name = 'AreaID' )
    BEGIN
        ALTER TABLE A_MonthlyReport
        ADD  AreaID UNIQUEIDENTIFIER;
    END;

	
	