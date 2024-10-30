USE master 
GO

USE Ultimate_DataBase
GO

SELECT Building.Bld_Name, Building.Bld_Address, DATENAME(month, Inspection.Ins_Date) AS 'Inspection Month', DATENAME(day, Inspection.Ins_Date) AS 'Inspection Day'
FROM Building
INNER JOIN  Inspection ON Building.Bld_ID = Inspection.Bld_ID
ORDER BY DATENAME(month, Inspection.Ins_Date), CAST(DATENAME(day,Inspection.Ins_Date) AS int)
GO

SELECT CONCAT(Applicant.App_Names , Applicant.App_Surname) AS 'Applicant Info', Country.Country_Name
FROM Applicant
INNER JOIN Country ON Applicant.Country_ID = Country.Country_ID
GO

SELECT CONCAT(Tenant.Ten_Name, Tenant.Ten_Surname) AS 'Tenant', DATEDIFF(year,Tenant.Ten_Date_of_Birth, GETDATE()) AS 'Age'
FROM Tenant
ORDER BY DATEDIFF(year,Tenant.Ten_Date_OF_Birth,GETDATE())
GO