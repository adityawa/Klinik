CREATE PROCEDURE [dbo].[sp_GenerateTop10ReferalData]
	@month int = 1 ,
	@year int = 2019,
	@clinicId int = NULL,
	@otherinfo nvarchar(max) = NULL,
	@patientName nvarchar(100) = NULL
AS 
BEGIN 
	select 
	Letter.Id, 
	Clinic.Id,
	Clinic.Name,
	Letter.LetterType,
	Letter.Keperluan,
	Letter.AutoNumber,
	Letter.[Action],
	Patient.Name,
	Month(Letter.CreatedDate) as [Month],
	Year(Letter.CreatedDate) as [Year],
	Letter.OtherInfo,
	Letter.PatientAge,
	ICDTheme.Code
	from 
	Letter 
	inner join FormMedical on Letter.FormMedicalID = FormMedical.ID 
	inner join Patient on FormMedical.PatientID = Patient.ID 
	inner join Clinic on Clinic.ID = FormMedical.ClinicID
	inner join FormExamine on FormMedical.Id = FormExamine.FormMedicalID
	inner join FormExamineICDInfo on FormExamineICDInfo.FormExamineId = FormExamine.Id
	inner join ICDTheme on ICDTheme.Id = FormExamineICDInfo.ICDId
	where 
	Month(Letter.CreatedDate) = @month 
	and Year(Letter.CreatedDate) = @year
	and Clinic.ID = @clinicId 
	and Patient.Name = @patientName
	order by ICDTheme.Code, Letter.CreatedDate desc
END



