-- ============================================================================
-- Schema Query Script for SocialMotive
-- Run this in SSMS or sqlcmd and paste the output for AI analysis
-- ============================================================================

USE [SocialMotive]
GO

-- ============================================================================
-- 1. TABLES & COLUMNS
-- ============================================================================
PRINT '=== TABLES & COLUMNS ===';
SELECT 
    t.TABLE_NAME,
    c.COLUMN_NAME,
    c.DATA_TYPE + 
        CASE 
            WHEN c.CHARACTER_MAXIMUM_LENGTH IS NOT NULL AND c.CHARACTER_MAXIMUM_LENGTH <> -1
                THEN '(' + CAST(c.CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
            WHEN c.CHARACTER_MAXIMUM_LENGTH = -1
                THEN '(max)'
            WHEN c.DATA_TYPE IN ('decimal','numeric')
                THEN '(' + CAST(c.NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(c.NUMERIC_SCALE AS VARCHAR) + ')'
            ELSE ''
        END AS [Type],
    CASE WHEN c.IS_NULLABLE = 'YES' THEN 'NULL' ELSE 'NOT NULL' END AS [Nullable],
    ISNULL(
        (SELECT 'IDENTITY' FROM sys.identity_columns ic
         JOIN sys.objects o ON ic.object_id = o.object_id
         WHERE o.name = t.TABLE_NAME AND ic.name = c.COLUMN_NAME), 
        '') AS [Identity],
    ISNULL(cc.definition, '') AS [Default]
FROM INFORMATION_SCHEMA.TABLES t
JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME AND t.TABLE_SCHEMA = c.TABLE_SCHEMA
LEFT JOIN sys.default_constraints cc 
    ON cc.parent_object_id = OBJECT_ID(t.TABLE_SCHEMA + '.' + t.TABLE_NAME)
    AND cc.parent_column_id = (
        SELECT column_id FROM sys.columns sc 
        WHERE sc.object_id = OBJECT_ID(t.TABLE_SCHEMA + '.' + t.TABLE_NAME) 
        AND sc.name = c.COLUMN_NAME
    )
WHERE t.TABLE_TYPE = 'BASE TABLE'
ORDER BY t.TABLE_NAME, c.ORDINAL_POSITION;

-- ============================================================================
-- 2. PRIMARY KEYS
-- ============================================================================
PRINT '';
PRINT '=== PRIMARY KEYS ===';
SELECT 
    tc.TABLE_NAME,
    STRING_AGG(kcu.COLUMN_NAME, ', ') WITHIN GROUP (ORDER BY kcu.ORDINAL_POSITION) AS [PK_Columns],
    tc.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu 
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
GROUP BY tc.TABLE_NAME, tc.CONSTRAINT_NAME
ORDER BY tc.TABLE_NAME;

-- ============================================================================
-- 3. FOREIGN KEYS
-- ============================================================================
PRINT '';
PRINT '=== FOREIGN KEYS ===';
SELECT 
    fk.name AS [FK_Name],
    tp.name AS [Parent_Table],
    cp.name AS [Parent_Column],
    tr.name AS [Referenced_Table],
    cr.name AS [Referenced_Column],
    CASE fk.delete_referential_action
        WHEN 0 THEN 'NO ACTION'
        WHEN 1 THEN 'CASCADE'
        WHEN 2 THEN 'SET NULL'
        WHEN 3 THEN 'SET DEFAULT'
    END AS [On_Delete]
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
JOIN sys.tables tp ON fkc.parent_object_id = tp.object_id
JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
JOIN sys.tables tr ON fkc.referenced_object_id = tr.object_id
JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
ORDER BY tp.name, fk.name;

-- ============================================================================
-- 4. INDEXES (non-PK)
-- ============================================================================
PRINT '';
PRINT '=== INDEXES ===';
SELECT 
    t.name AS [Table],
    i.name AS [Index_Name],
    CASE WHEN i.is_unique = 1 THEN 'UNIQUE' ELSE '' END AS [Unique],
    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS [Columns]
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.is_primary_key = 0 AND i.type > 0
GROUP BY t.name, i.name, i.is_unique
ORDER BY t.name, i.name;

-- ============================================================================
-- 5. ROW COUNTS
-- ============================================================================
PRINT '';
PRINT '=== ROW COUNTS ===';
SELECT 
    t.name AS [Table],
    p.rows AS [Row_Count]
FROM sys.tables t
JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0, 1)
ORDER BY t.name;

PRINT '';
PRINT '=== DONE ===';
GO
