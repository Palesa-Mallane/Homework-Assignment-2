use Daspoort_Clinic
go

--------------------------------------------------------------------
--PRACTICAL 2
--------------------------------------------------------------------

--QUESTION 1
SELECT * 
FROM Referral_Doctor
GO

--QUESTION 2
SELECT p.Name, p.Surname, p.ID_Number, p.Patient_Address, p.TelePhone, p.Gender
FROM Patient p
GO

--QUESTION 3
SELECT *
FROM Patient p
WHERE p.CitizenShip <> 'South African'
GO

--QUESTION 4
SELECT * 
FROM Referral_Doctor
WHERE Referral_Doctor.Doc_Surname LIKE '%Crane%' OR 
Referral_Doctor.Doc_Surname LIKE '%Beatty%' OR
Referral_Doctor.Doc_Surname LIKE '%Joe%'
GO

--QUESTION 5
SELECT rc.Clinic_Name, rc.Clinic_Phone, rc.Clinic_Address
FROM Referral_Clinic rc
WHERE rc.Clinic_Address LIKE '%Mashishing%'
GO

--QUESTION 6
SELECT *
FROM Consultation c
WHERE c.Con_Hist_BloodPressure < 65
ORDER BY c.Con_Hist_BloodPressure DESC
GO

--QUESTION 7
SELECT AVG(Consultation.Con_Hist_BMI) AS 'Average BMI'
FROM Consultation
GO

--------------------------------------------------------------------
--PRACTICAL 3
--------------------------------------------------------------------

--QUESTION 1

use GotchaSystems
GO

--QUESTION 1
SELECT m.Member_Name, m.Member_Surname, MP.Picture
FROM [Member] m, MemberPicture MP
GO

--QUESTION 2
SELECT b.Date_Booking_Made, b.Booking_Number, bs.Booking_Status_Description
FROM Booking b, BookingStatus bs
WHERE bs.Booking_Status_Description = 'Confirmed'
GO

--QUESTION 3
SELECT b.User_Name, b.Booking_Number, b.Date_Booking_Made, BookingSession.Booking_Session_Status
FROM  Booking b, BookingSession
WHERE b.Booking_Session_ID = BookingSession.Booking_Session_ID
AND BookingSession.Booking_Session_Status = 'Unavailable'
AND b.User_Name LIKE '%jacquescloete%'
GO

--QUESTION 4
SELECT b.Booking_Number,b.Date_Booking_Made, br.Booking_Rep_Name, br.Booking_Rep_Surname, t.Title_Name
FROM Booking b
INNER JOIN BookingRepresentative br on br.Booking_Rep_ID = b.Booking_Rep_ID
INNER JOIN Title t on t.Title_ID = br.Title_ID
WHERE t.Title_Name LIKE '%Mr%'
GO

--------------------------------------------------------------------
--PRACTICAL 4
--------------------------------------------------------------------

use DBSocialHire
go

--QUESTION 1
SELECT Item.Item_ID, Item.Item_Specification, IQ.Item_Question_Description, IQ.Item_Question_Answer
FROM Item
INNER JOIN Item_Questions IQ ON IQ.Item_ID = Item.Item_ID
WHERE Item.Item_Specification LIKE 'Black %'
GO

--QUESTION 2
SELECT DISTINCT i.Item_Name, ri.Rented_Start_Date, ri.Rented_End_Date
FROM  Rented_Item ri 
LEFT OUTER JOIN Item i ON i.Item_ID = ri.Item_ID 
GO

--QUESTION 3
SELECT r.Rental_ID,r.Rental_Date,C.First_Name + '.' + C.Last_Name AS 'Clients'
FROM Client c
RIGHT OUTER JOIN Rentals r ON r.Client_ID = c.Client_ID
ORDER BY R.Rental_ID

--QUESTION 4
SELECT i.Item_ID, i.Item_Name, i.Item_Specification, ri.Rent_Return_Date, c.Complaint_Nature 
FROM Item i
INNER JOIN Rented_Item ri ON i.Item_ID = ri.Item_ID
INNER JOIN Complaint c ON c.Rented_ID = ri.Rented_ID 
WHERE ri.Rent_Return_Date > 2011
ORDER BY C.Complaint_Nature
GO

--------------------------------------------------------------------
--PRACTICAL 5
--------------------------------------------------------------------

use School
go

--QUESTION 1


SELECT p.FirstName, p.EnrollmentDate, sg.CourseID, sg.Grade 
FROM Person p
RIGHT OUTER JOIN StudentGrade sg ON sg.StudentID = p.PersonID
WHERE sg.Grade IS NOT NULL
AND p.EnrollmentDate > '2004-07-31'
ORDER BY sg.CourseID
GO

--QUESTION 2
SELECT c.CourseID, c.Title, c.DepartmentID, ci.PersonID
FROM Course c
LEFT OUTER JOIN CourseInstructor ci ON ci.CourseID = c.CourseID
WHERE ci.PersonID IS NULL
GO

--QUESTION 3
SELECT c.CourseID, OC.URL, C.Title, C.Credits
FROM Course c
RIGHT OUTER JOIN OnlineCourse oc ON OC.CourseID = C.CourseID
WHERE C.Credits BETWEEN 3 AND 5
AND C.Title <> 'Algebra'
GO

--------------------------------------------------------------------
--PRACTICAL 6
--------------------------------------------------------------------
USE Ultimate_DataBase
GO

--QUESTION 3
SELECT b.Bld_Name, b.Bld_Address, DATENAME(MONTH,i.Ins_Date) AS 'InspectionMonth', DATENAME(DAY, i.Ins_Date) AS 'InspectionDay'
FROM Building b
INNER JOIN Inspection i ON b.Bld_ID = i.Bld_ID
GROUP BY Bld_Name, Bld_Address, Ins_Date
ORDER BY  DATENAME(DAY, i.Ins_Date)  DESC
GO

--QUESTION 2
SELECT CONCAT(a.App_Names,' ',A.App_Surname,'; ',a.App_Cellphone) AS 'Applicant Info', c.Country_Name
FROM Applicant a
INNER JOIN Country c ON c.Country_ID = a.Country_ID
GO


--QUESTION 3
SELECT CONCAT(t.Ten_Name,' ', T.Ten_Surname) AS 'Tenant', DATEDIFF(YYYY,t.Ten_Date_OF_Birth,GETDATE()) AS 'TenantAge'
FROM Tenant t
GROUP BY t.Ten_Name, t.Ten_Surname, t.Ten_Date_OF_Birth
ORDER BY  DATEDIFF(YYYY,t.Ten_Date_OF_Birth,GETDATE())
GO

--------------------------------------------------------------------
--PRACTICAL 7 - MOCK TEST
-------------------------------------------------------------------

use Daspoort_Clinic
go 

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

--QUESTION 2
SELECT e.Employee_Name, e.Employee_Surname, e.Commencement_date, t.Name
FROM Employee e
INNER JOIN Title t ON t.Title_ID = e.Title_ID
WHERE e.Commencement_date > '2009-11-30'
ORDER BY e.Employee_Name
GO

--QUESTION 3
SELECT c.Patient_No, c.Con_Consult_Date, CONCAT(Users.Name, ' ', Users.Surname) as 'User'
FROM Consultation c
INNER JOIN Users ON Users.User_ID = c.User_ID
GO

--QUESTION 4
SELECT e.Employee_Name, e.Employee_Surname, DATEDIFF(YYYY,CAST(SUBSTRING( e.Employee_Idnumber, 1, 6) AS DATE) ,GETDATE()) AS 'Age', e.Commencement_date, Users.UserName
FROM Employee e
LEFT OUTER JOIN Users ON e.User_ID = Users.User_ID
GROUP BY e.Employee_Name, e.Employee_Surname, e.Employee_Idnumber,  e.Commencement_date, Users.UserName
GO

--QUESTION 5
SELECT ms.Med_Sup_Name, ms.Med_Sup_Quantity, COUNT(ims.Quantity) as 'No of times product issued',SUM(ims.Quantity) as 'Qty of grouped products issued', ims.IssueDate
FROM Medical_Supply ms
INNER JOIN IssueMedicalSupply ims ON ms.Med_Sup_ID = ims.Med_Sup_ID
GROUP BY ms.Med_Sup_Name, ms.Med_Sup_Quantity, ims.IssueDate
HAVING COUNT(*) > 1
GO

--------------------------------------------------------------------
--PRACTICAL 8
-------------------------------------------------------------------
use GotchaSystems
go


--QUESTION 1
SELECT s.Stock_Item_Name, s.Stock_Item_Description, sr.Stock_Returned_Date
FROM Stock s INNER JOIN StockReturned sr ON s.Stock_Item_ID = sr.Stock_Item_ID
WHERE Stock_Returned_Date = 
								(SELECT MAX(Stock_Returned_Date)
								FROM StockReturned
								)
GO

--QUESTION 2
SELECT Employee.Employee_ID, Employee.Employee_Name, Employee.Employee_Surname
FROM Employee 
WHERE Employee_ID NOT IN (SELECT Employee_ID FROM CheckIn)
GO

--QUESTION 3
SELECT Stock.Stock_Item_Name, Stock_Item_Description
FROM Stock
WHERE Stock_Item_ID IN 
				(SELECT Stock_Item_ID FROM StockWriteOff WHERE Stock_Item_ID IN (
					SELECT Stock_Item_ID FROM StockReturned WHERE Stock_Returned_Reason LIKE '%incorrect%'))
GO

--QUESTION 4
SELECT p.Payment_ID, p.Payment_Amount, p.Payment_Date
FROM Payment p
WHERE Member_ID IN 
	(SELECT Member_ID FROM [Member])
GO

--------------------------------------------------------------------
--PRACTICAL 9
-------------------------------------------------------------------
use Daspoort_Clinic
GO

--QUESTION 1.1
SELECT p.Patient_No, p.Name, p.Surname, p.CitizenShip, c.Consult_No, c.Con_Weight
FROM Patient p LEFT OUTER JOIN Consultation c ON c.Patient_No = p.Patient_No
WHERE Con_Weight < 
			(SELECT AVG(Con_Weight) FROM Consultation)
GO

--QUESTION 1.2
SELECT rh.Hosp_name, rh.Hosp_Address
FROM Referral_Hospital rh 
WHERE Hosp_Province IN 
				(SELECT Clinic_Province FROM Referral_Clinic WHERE Clinic_Province IN
				(SELECT Province_ID FROM Province WHERE Province_Name ='Gauteng' ))
GO

--QUESTION 1.3
SELECT p.Name, p.Surname, p.Province_ID, c.Con_Hist_BMI
FROM Patient p INNER JOIN Consultation c ON p.Patient_No = c.Patient_No
WHERE Con_Hist_BMI = 
					(SELECT MAX(Con_Hist_BMI) FROM Consultation WHERE Patient_No IN 
					(SELECT Patient_No FROM Patient WHERE Patient.Province_ID = p.Province_ID))

--------------------------------------------------------------------
--PRACTICAL 10 - SICK TEST 2023?
-------------------------------------------------------------------
use GotchaSystems
go


--QUESTION 1
SELECT si.Supplier_Invoice_Number, si.Supplier_Invoice_Date, sil.Supplier_Invoice_Line_Quantity
FROM SupplierInvoice as SI 
INNER JOIN SupplierInvoiceLine as SIL ON SI.Supplier_Invoice_Number = SIL.Supplier_Invoice_Number
WHERE SIL.Supplier_Invoice_Line_Quantity > 
					(SELECT AVG(Supplier_Invoice_Line_Quantity) FROM SupplierInvoiceLine)
ORDER BY SIL.Supplier_Invoice_Line_Quantity

--QUESTION 2
SELECT s.Stock_Item_ID, s.Stock_Item_Name, s.Stock_Item_Quantity, sr.Stock_Received_Date, sr.Stock_Received_Quantity, sw.Stock_Write_Off_Date, sw.Stock_Write_Off_Quantity, sw.Stock_Write_Off_Reason
FROM Stock s
FULL OUTER JOIN StockReceived sr ON sr.Stock_Item_ID = s.Stock_Item_ID
FULL OUTER JOIN StockWriteOff sw ON sw.Stock_Item_ID = sr.Stock_Item_ID
WHERE Stock_Write_Off_Date >= '2011-10-01'

--QUESTION 3

SELECT [Order].Order_Number, DATENAME(Day,Order_Date)AS 'Day', DATENAME(Month,Order_Date) AS 'Month', DATENAME(Year, Order_Date) AS 'Year', [Order].Supplier_ID
FROM [Order] 
WHERE EXISTS 
		(SELECT * 
		FROM Supplier
		WHERE [Order].Supplier_ID = Supplier.Supplier_ID 
		AND Supplier.Supplier_Name LIKE '%CC%')


--QUESTION 4
SELECT s.Stock_Item_ID, s.Stock_Item_Name, COUNT(DISTINCT O.Supplier_ID) as 'Supplier Count'
FROM Stock AS s
INNER JOIN StockReceived AS SR ON SR.Stock_Item_ID = s.Stock_Item_ID
INNER JOIN SupplierInvoice as SI ON SI.Supplier_Invoice_Number= SR.Supplier_Invoice_Number
INNER JOIN [Order] O ON O.Order_Number = SI.Order_Number
GROUP BY s.Stock_Item_ID, s.Stock_Item_Name
HAVING COUNT (DISTINCT O.Supplier_ID) >=3


--------------------------------------------------------------------
--PRACTICAL SEM TEST 2024
-------------------------------------------------------------------

--QUESTION 1

IF EXISTS (SELECT * FROM SYS.DATABASES WHERE NAME = 'UniversityDB')

DROP DATABASE UniversityDB
GO


CREATE DATABASE UniversityDB
GO

USE UniversityDB
GO

CREATE TABLE Student (
Student_ID INT  IDENTITY(1,1)  PRIMARY KEY NOT NULL,
Student_Name VARCHAR (255),
Student_Cell VARCHAR(255),
Date_Enrolled DATE,
Street_address VARCHAR (255),
Final_Mark DECIMAL (6,4)
)

INSERT INTO Student VALUES 
('Frank', '012 345 6789', '01-02-2024', '11, Blouberg Street, Pretoria', 68.5),
('Gloria', '012 345 6722', '01-02-2019', '67, Poker Street, Johannesburg', 75.0),
('Peter', '065 987 6543', '06-02-2020', '23, William Road, Centurion', 49.6)

SELECT * FROM Student

--QUESTION 2
use GotchaSystems
GO

SELECT S.Supplier_Name, OL.Order_Line_Item_Name, OL.Order_Line_Description, Sum(DISTINCT OL.Order_Line_Quantity) AS 'Quantity'
FROM [Order] o
INNER JOIN SupplierInvoice SI on SI.Order_Number = O.Order_Number
INNER JOIN OrderLine OL ON OL.Order_Number = SI.Order_Number
INNER JOIN Supplier S ON S.Supplier_ID = O.Supplier_ID
WHERE OL.Order_Line_Item_Name LIKE '%Helmet%'
GROUP BY S.Supplier_Name, OL.Order_Line_Item_Name, OL.Order_Line_Description
ORDER BY Sum( DISTINCT OL.Order_Line_Quantity)


--QUESTION 3
SELECT b.Booking_Number, DATEDIFF(DAY,b.Date_Booking_Made, BS.Booking_Session_Time) as 'Early Booking',  SUM(CAST(b.Total_Guests_Attending AS INT) - CAST(b.Number_Of_Members_Attending AS INT)) as 'Non-Member Attendees',bs.Booking_Session_Status
FROM Booking b
INNER JOIN BookingSession bs ON bs.Booking_Session_ID = b.Booking_Session_ID
GROUP BY b.Booking_Number, bs.Booking_Session_Status, B.Date_Booking_Made, BS.Booking_Session_Time
HAVING SUM(CAST(b.Total_Guests_Attending AS INT) - CAST(b.Number_Of_Members_Attending AS INT)) BETWEEN 50 AND 120
AND DATEDIFF(DAY,b.Date_Booking_Made, BS.Booking_Session_Time) > 1

--QUESTION 4
SELECT S.Stock_Item_ID, S.Stock_Item_Name, S.Stock_Item_Quantity, SRET.Stock_Returned_Date, SREC.Stock_Received_Date, SRET.Stock_Returned_Quantity, SRET.Stock_Returned_Reason, SUM(CAST(S.Stock_Item_Quantity AS INT)  - CAST(SRET.Stock_Returned_Quantity AS INT) + CAST(SREC.Stock_Received_Quantity AS INT)), DATEDIFF(DAY, SRET.Stock_Returned_Date, SREC.Stock_Received_Date) AS 'Days_Difference'
FROM Stock S
LEFT OUTER JOIN StockReceived SREC ON SREC.Stock_Item_ID = S.Stock_Item_ID
LEFT OUTER JOIN StockReturned SRET ON SRET.Stock_Item_ID = S.Stock_Item_ID
WHERE SRET.Stock_Returned_Date IS NOT NULL
GROUP BY S.Stock_Item_ID, S.Stock_Item_Name, S.Stock_Item_Quantity, SRET.Stock_Returned_Date, SREC.Stock_Received_Date, SRET.Stock_Returned_Quantity, SRET.Stock_Returned_Reason
ORDER BY S.Stock_Item_ID

--QUESTION 5
SELECT o.Order_Number, s.Supplier_Name, CONCAT(s.Supplier_Rep_Name, ' ',S.Supplier_Rep_Surname,' - ', S.Supplier_Rep_Tel_Num) as 'Supplier Representative', CONCAT(DATENAME(Day,o.Order_Date),' ',DATENAME(MONTH,o.Order_Date),' ',DATENAME(YEAR,o.Order_Date)) AS 'Order_Date', FORMAT(CAST(o.Order_Date as datetime), 'HH:mm:ss') AS 'Time_Order_Was_Made', SUM(OL.Order_Line_Quantity) AS 'Number of products ordered'
FROM [Order] o
INNER JOIN Supplier s ON s.Supplier_ID = o.Supplier_ID
INNER JOIN OrderLine OL ON OL.Order_Number = O.Order_Number
GROUP BY o.Order_Number, s.Supplier_Name,s.Supplier_Rep_Name,S.Supplier_Rep_Surname, S.Supplier_Rep_Tel_Num,o.Order_Date
