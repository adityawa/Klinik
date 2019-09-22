CREATE  PROCEDURE [dbo].[sp_GenerateTop10DiseaseReportData]
	@month int = 1 ,
	@year int = 2019,
	@clinicId int = NULL,
	@department nvarchar(100) = NULL,
	@businessunit nvarchar(100) =  NULL,
	@age decimal(18,4) = NULL, 
	@gender nvarchar(2) = NULL,
	@famStatus nvarchar(50) = NULL,
	@patientCategory nvarchar(50) = NULL,
	@necessityType nvarchar(10) = NULL,
	@examineType nvarchar(10) =  NULL,
	@needrest nvarchar(2) = NULL

AS
select FormExamine.Id as FormExamineId, 
	   ICDTheme.Id as ICDId,
	   Clinic.Id as ClinicId, 
	   Clinic.Name as ClinicName,
	   Patient.Name as PatientName, 
	   Employee.EmpName as EmpName,
	   EmployeeAssignment.Department as Department,
	   EmployeeAssignment.BusinessUnit as BusinessUnit,
	   EmployeeAssignment.Region as Region,
	   EmployeeStatus.Name as EmpStatus,
	   Patient.BirthDate as BirthDate,
	   Patient.BPJSNumber as BPJSNumber,
	   Patient.Gender as Gender,
	   PatientAge.Age as Age,
	   FamilyRelationship.Code as FamCode, 
	   FamilyRelationship.Name as FamName, 
	   FormExamine.TransDate,
	   FormExamine.NeedRest,
	   FormExamine.IsAccident,
	   FormExamine.Diagnose,
	   FormMedical.Necessity,
	   FormMedical.PaymentType,
	   ICDTheme.Code
from FormExamine 
left join FormMedical on FormExamine.FormMedicalID = FormMedical.Id 
inner join FormExamineICDInfo on FormExamine.ID = FormExamineICDInfo.FormExamineId
inner join ICDTheme on ICDTheme.Id = FormExamineICDInfo.ICDId
inner join Patient on Patient.Id = FormMedical.PatientID
inner join FamilyRelationship on Patient.FamilyRelationshipID = FamilyRelationship.Id  
inner join PatientAge on Patient.Id = PatientAge.PatientId
inner join Clinic on Clinic.Id = FormMedical.ClinicID
inner join Employee on Patient.EmployeeID = Employee.Id
left join EmployeeAssignment on Employee.Id = EmployeeAssignment.EmployeeID
left join EmployeeStatus on Employee.EmpType = EmployeeStatus.ID
where 
month(FormExamine.TransDate) = @month
and year(FormExamine.TransDate) = @year
and Clinic.ID = @clinicId
and EmployeeAssignment.Department = @department
and EmployeeAssignment.BusinessUnit = @businessunit
and PatientAge.Age = @age
and Patient.Gender = @gender
and FamilyRelationship.Code = @famStatus
and EmployeeStatus.Code = @patientCategory
and FormMedical.Necessity = @necessityType
and FormExamine.NeedRest = @needrest
and FormExamine.IsAccident = @examineType
order by Patient.Name, ICDTheme.Id, TransDate Desc