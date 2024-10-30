use GotchaSystems
go

select m.Member_Name, m.Member_Surname, mp.Picture
from Member m join MemberPicture mp on m.Member_ID = mp.Member_ID
go

select b.Date_Booking_Made, b.Booking_Number, bs.Booking_Status_Description
from Booking b join BookingStatus bs ON b.Booking_Status_ID = bs.Booking_Status_ID
where Booking_Status_Description = 'Confirmed'
go

select b.User_Name, b.Booking_Number, b.Date_Booking_Made, bs.Booking_Session_Status
from booking b, BookingSession bs
where b.Booking_Session_ID = bs.Booking_Session_ID
AND Booking_Session_Status = 'Unavailable' 
AND User_Name like '%jacquescloete%'
go

select b.Booking_Number, b.Date_Booking_Made, rep.Booking_Rep_Name, title.Title_Name
from booking b 
inner join BookingRepresentative rep on b.Booking_Rep_ID = rep.Booking_Rep_ID
inner join Title on rep.Title_ID = Title.Title_ID
where Title_Name like '%Mr%'
go
