USE Ultimate_Database
GO

BEGIN TRY 
BEGIN TRANSACTION;

INSERT INTO Owner_Table VALUES 
    (
        'John', 
        'Doe', 
        '1234567890123', 
        '1973-09-01', 
        '0123456789', 
        '0712345678', 
        'john.doe@gmail.com', 
        1, 
        '123 Postal Street', 
        '01234', 
        '123 Street Name', 
        '56789', 
        9999,  -- Invalid Title_ID 
		1,  
        1 
    );
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
		PRINT 'Transaction Failed. Insertion of Owner Rolled Back'; 
END CATCH


select * from Owner_Table

