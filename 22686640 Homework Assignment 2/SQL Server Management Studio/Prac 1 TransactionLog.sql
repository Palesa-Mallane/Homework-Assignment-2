IF OBJECT_ID('fn_dblog') IS NOT NULL
BEGIN
	SELECT 
	[Current LSN],
	[Operation],
	[Transaction ID],
	[Transaction Name],
	[Begin Time],
	[End Time],
	[AllocUnitName],
	[Page ID],
	[Slot ID],
	[Previous LSN],
	[RowLog Contents 0],
	[RowLog Contents 1]
    FROM fn_dblog(NULL, NULL)
	WHERE [Operation] IN ('LOP_INSERT_ROWS', 'L+OP_MODIFY_ROW', 'LOP_DELETE_ROWS')
		AND ([AllocUnitName] like '%owner%' OR [AllocUnitName] like '%employer%');
END
ELSE
PRINT 'The fn_dblog function is not available in your version of SQL Server';