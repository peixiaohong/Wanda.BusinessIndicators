
--�汾����
IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'B_TargetPlan'
                        AND T2.name = 'VersionName' )
    BEGIN
        ALTER TABLE B_TargetPlan
        ADD  VersionName nvarchar(50);
    END;
	--Ĭ�ϰ汾
	IF NOT EXISTS ( SELECT  1
                FROM    sysobjects T1
                        INNER JOIN syscolumns T2 ON T1.id = T2.id
                WHERE   T1.name = 'B_TargetPlan'
                        AND T2.name = 'VersionDefault' )
    BEGIN
        ALTER TABLE B_TargetPlan
        ADD  VersionDefault int NOT NULL DEFAULT 0;
    END;