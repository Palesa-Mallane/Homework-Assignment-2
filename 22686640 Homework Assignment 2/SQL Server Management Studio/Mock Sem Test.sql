USE Daspoort_Clinic
GO

Create table Employee (
Employee_ID INT IDENTITY (1,1) PRIMARY KEY NOT NULL,
Employee_Name VARCHAR(50),
Employee_Surname VARCHAR(50),
Employee_Idnumber VARCHAR (13),
Employee_Address VARCHAR(50),
Cell_number VARCHAR(50),
Title_ID INT REFERENCES Title(Title_ID),
User_ID INT REFERENCES Users(User_ID),
Commencement_date date 
)
GO 

INSERT INTO Employee VALUES ('John', 'Doe', '8601015800086', 'Cape Town, 35, Sneeuberg Street', '0760338332', 4, NULL, '2011-02-01')
INSERT INTO Employee VALUES ('Mikhail', 'Davids', '7901014800082','Klerksdorp ,54, Walmer Road',0823973546,4, NULL,'2003-02-04')
INSERT INTO Employee VALUES ('Chelsea', 'Adams', '9701065800082','Pretoria ,84, Poker Road',07277664893,4, NULL,'2019-05-06')
INSERT INTO Employee VALUES ('Jessica', 'Koshby', '9710114800084','Pretoria ,34, Lame Road',0843388432,4, NULL,'2012-03-06')
INSERT INTO Employee VALUES ('Kagisho', 'Shai', '8609115800088','Pretoria ,22, Verwagt Road',0725678935,4,2,'2020-06-06')
GO

SELECT Employee.Employee_Name, Employee.Employee_Surname, Employee.Commencement_Date, Title.Name
FROM Employee
INNER JOIN Title
ON Employee.Title_ID = Title.Title_ID
WHERE Employee.Commencement_date BETWEEN '2010-01-01' AND '2020-01-01' AND Employee.Employee_Address LIKE '%Pretoria%'
ORDER BY Employee.Employee_Name
GO

SELECT Patient.Patient_No, Consultation.Con_Consult_Date, CONCAT(Patient.Name, ' ', Patient.Surname) AS 'User'
FROM Patient
INNER JOIN Consultation ON Patient.Patient_No = Consultation.Patient_No
WHERE Consultation.Con_Consult_Date > '2011-04-10'
GO

SELECT Employee.Employee_Name, Employee.Employee_Surname, DATEDIFF(year,CAST(SUBSTRING(Employee.Employee_Idnumber, 1, 6) AS DATE),GETDATE()) AS 'Age' , CONCAT(DATENAME(DAY,Employee.Commencement_date),' ', DATENAME(MONTH,Employee.Commencement_date),' ',DATENAME(YEAR,Employee.Commencement_date)) as 'Commencement Date' , Users.UserName
FROM EMPLOYEE
LEFT OUTER JOIN Users ON Employee.User_ID = Users.User_ID
ORDER BY Age DESC 
GO

SELECT m.Med_Sup_Name, m.Med_Sup_Quantity, COUNT(*) AS 'No. of times product issued',SUM(ims.Quantity) AS 'Qty of grouped products issued',ims.IssueDate
FROM Medical_Supply m
INNER JOIN IssueMedicalSupply ims ON m.Med_Sup_ID = ims.Med_Sup_ID

GROUP BY m.Med_Sup_Name, m.Med_Sup_Quantity, ims.IssueDate
HAVING COUNT(*) > 1
GO