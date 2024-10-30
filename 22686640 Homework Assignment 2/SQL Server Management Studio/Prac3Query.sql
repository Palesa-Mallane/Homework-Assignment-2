USE master
GO

USE GotchaSystems
GO

SELECT Member_Name, Member_Surname, Picture 
FROM Member JOIN MemberPicture
ON Member.Member_ID = MemberPicture.Member_ID;
GO

SELECT Booking_Number, Date_Booking_Made, Booking_Status_Description
FROM Booking JOIN BookingStatus
ON Booking.Booking_Status_ID = BookingStatus.Booking_Status_ID
WHERE Booking_Status_Description = 'Confirmed';
GO

SELECT User_Name, Booking_Number, Booking_Session_Status, Date_Booking_Made
FROM Booking JOIN BookingSession
ON Booking.Booking_Session_ID = BookingSession.Booking_Session_ID
WHERE Booking_Session_Status = 'Unavailable'
AND User_Name LIKE '%jacquescloete%'
GO

SELECT Booking_Number, Date_Booking_Made,Booking_Rep_Name, Booking_Rep_Surname, Title_Name
FROM Booking
INNER JOIN BookingRepresentative ON BookingRepresentative.Booking_Rep_ID = Booking.Booking_Rep_ID
INNER JOIN Title ON Title.Title_ID = BookingRepresentative.Title_ID
WHERE Title.Title_Name LIKE '%Mr%'
GO