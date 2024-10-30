use Daspoort_Clinic
go


--Question 1.1
SELECT p.Patient_No, p.Name, p.Surname, p.CitizenShip ,  c.Consult_No, c.Con_Weight
FROM Patient p
LEFT OUTER JOIN Consultation c ON c.Patient_No = p.Patient_No
WHERE c.Con_Weight < (SELECT avg(C2.con_weight) FROM Consultation C2)
GO


--Question 1.2
SELECT rh.Hosp_Name, rh.Hosp_Address
FROM Referral_Hospital rh
WHERE rh.Hosp_Province IN (
    SELECT pc.Clinic_Province
    FROM Referral_Clinic pc
    WHERE pc.Clinic_Province = (
        SELECT Province_ID
        FROM Province
        WHERE Province_Name LIKE 'Gauteng'
    )
);

--Question 1.3

SELECT p.Patient_No, p.Name, p.Surname, p.Province_ID,c.Con_Hist_BMI
FROM Patient p
INNER JOIN Consultation c ON p.Patient_No = c.Patient_No
WHERE c.Con_Hist_BMI = (
        SELECT MAX(Con_Hist_BMI)
        FROM Consultation
        WHERE Patient_No IN (
                SELECT Patient_No
                FROM Patient
                WHERE Province_ID = p.Province_ID))
ORDER BY p.Province_ID
GO


