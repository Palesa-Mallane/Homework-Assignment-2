use Daspoort_Clinic
go

select * 
from Referral_Doctor
go

select patient.Name, patient.Surname, patient.ID_Number, patient.Patient_Address, patient.TelePhone, patient.Gender
from Patient
go

select * 
from Patient
where patient.CitizenShip <> 'South African'
go

select * from Referral_Doctor RD
where RD.Doc_ID = 1 OR RD.Doc_ID = 2 OR RD.Doc_ID = 7
go

select rc.Clinic_Name, rc.Clinic_Phone, rc.Clinic_Address
from Referral_Clinic rc
where rc.Clinic_Address like '%Mashishing%'
go

select * from Consultation
where consultation.Con_Hist_BloodPressure < 65
order by Consultation.Con_Hist_BloodPressure desc
go

select avg(Consultation.Con_Hist_BMI) as 'Average BMI'
from Consultation
GO
