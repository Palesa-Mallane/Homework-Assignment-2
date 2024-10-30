USE Ultimate_DataBase
GO

BEGIN TRY
BEGIN TRANSACTION;

insert into Employer values('Teen Titans','125 South Street Jump City','0113697895','0126674545','admin@titanstower.co.za','2013/08/10','20050.00','Mr B Wayne')


COMMIT TRANSACTION;

PRINT 'Transaction Successful. The employee has been inserted.'

END TRY

BEGIN CATCH 
	ROLLBACK TRANSACTION;
	SELECT
		ERROR_NUMBER() AS ErrorNumber,
		ERROR_SEVERITY() AS ErrorSeverity, 
		ERROR_STATE() as ErrorState, 
		ERROR_PROCEDURE() as ErrorProcedure, 
		ERROR_LINE() as ErrorLine, 
		ERROR_MESSAGE() as ErrorMessage; 
		PRINT 'Transaction Failed'; 
END CATCH