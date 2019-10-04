USE [KlinikDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_generateTop10DiseaseReport]    Script Date: 10/4/2019 5:03:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[sp_generateTop10DiseaseReport] 
				@monthStart int,
				@yearStart int,
				@monthEnd int,
				@yearEnd int,
				@clinicId int,
				@category nvarchar(100),
				@categoryItem nvarchar(200)
as 
	begin 
		
		declare @dateStart datetime 
		declare @dateEnd datetime

		if @monthStart <> 0 and @yearStart <> 0 
			begin 
				set @dateStart = (select DATEFROMPARTS(@yearStart,@monthStart,1))
			end 
		if @monthEnd <> 0 and @yearEnd <> 0 
			begin
				set @dateEnd = (select DATEFROMPARTS(@yearEnd, @monthEnd, 31))
			end

		if object_id('TempDB.dbo.#TmpDisease') is not null 
			begin 
				drop table tempdb.dbo.#TmpDisease
			end 

		-- create temp table 
		create table #TmpDisease (
				FormExamineId bigint,
				IcdId bigint,
				ClinicId bigint,
				ClinicName nvarchar(max),
				PatientName nvarchar(max),
				EmpName nvarchar(max),
				Department nvarchar(max),
				BusinessUnit nvarchar(max),
				Region nvarchar(max),
				StatusName nvarchar(max),
				BirthDate datetime, 
				BPJSNumber nvarchar(max),
				Gender nvarchar(max),
				Age decimal(18,4),
				AgeCode nvarchar(max),
				FamCode nvarchar(max),
				FamName nvarchar(max),
				TransDate datetime,
				NeedRest nvarchar(max),
				ExamineType nvarchar(max),
				Diagnose nvarchar(max),
				Necessity nvarchar(max),
				PaymentType nvarchar(max),
				ICDCode nvarchar(max),
				ICDName nvarchar(max)
				);
		
		-- insert temp table		
		insert into #TmpDisease
			select FormExamine.Id as FormExamineId,  ICDTheme.Id as ICDId, Clinic.Id as ClinicId,
			Clinic.Name as ClinicName, Patient.Name as PatientName, Employee.EmpName as EmpName, EmployeeAssignment.Department ,
			EmployeeAssignment.BusinessUnit, EmployeeAssignment.Region, EmployeeStatus.Name as StatusName,
			Patient.BirthDate, Patient.BPJSNumber, Patient.Gender as Gender, PatientAge.Age, PatientAge.AgeCode, FamilyRelationship.Code as FamCode, 
			FamilyRelationship.Name as FamName,  FormExamine.TransDate, FormExamine.NeedRest,
			FormExamine.IsAccident as ExamineType,  FormExamine.Diagnose, FormMedical.Necessity,
			FormMedical.PaymentType, ICDTheme.Code as ICDCode, ICDTheme.Name as ICDName
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
			order by FormExamine.Id, ICDTheme.Id

		
		if @dateStart is not null and @dateEnd is not null and @clinicId is not null
			begin 				
				if @category = 'Department' 
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId, ICDCode, ICDName, Department as Category, count(Department) as Total from #TmpDisease where Department = @categoryItem and ClinicId = @clinicId group by ICDId, ICDCode, ICDName, Department
							end 
						else 
							begin 
								select top 10 ICDId, ICDCode, ICDName, Department as Category, count(Department) as Total from #TmpDisease where ClinicId = @clinicId group by ICDId, ICDCode,  ICDName, Department
							end 
					end 
				else if @category = 'BusinessUnit'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, BusinessUnit as Category, count(BusinessUnit) as Total from #TmpDisease where BusinessUnit = @categoryItem and ClinicId = @clinicId  group by ICDId, ICDCode,  ICDName,  BusinessUnit
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, BusinessUnit as Category, count(BusinessUnit) as Total from #TmpDisease where ClinicId = @clinicId group by ICDId, ICDCode,  ICDName,  BusinessUnit
							end 
					end 
				else if @category = 'Gender'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, Gender as Category, count(Gender) as Total from #TmpDisease where Gender = @categoryItem and ClinicId = @clinicId  group by ICDId, ICDCode, ICDName,  Gender
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, Gender as Category, count(Gender) as Total from #TmpDisease where ClinicId = @clinicId group by ICDId, ICDCode, ICDName,  Gender
							end 
					end 
				else if @category = 'Age'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, Age as Category, count(Age) as Total from #TmpDisease where Age = @categoryItem and ClinicId = @clinicId  group by ICDId,ICDCode, ICDName,  ICDCode,Age
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, Age as Category, count(Age) as Total from #TmpDisease where ClinicId = @clinicId  group by ICDId, ICDCode,  ICDName, Age
							end 
					end 
				else if @category = 'EmploymentType'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, StatusName as Category, count(StatusName) as Total from #TmpDisease where StatusName = @categoryItem and ClinicId = @clinicId  group by ICDId, ICDCode, ICDName, StatusName
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, StatusName as Category, count(StatusName) as Total from #TmpDisease where ClinicId = @clinicId group by ICDId, ICDCode, ICDName, StatusName
							end 
					end 
				else if @category = 'FamilyStatus'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, FamName as Category, count(FamName) as Total from #TmpDisease where FamCode = @categoryItem and ClinicId = @clinicId  group by ICDId, ICDCode,  ICDName, FamName 
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, FamName as Category, count(FamName) as Total from #TmpDisease where ClinicId = @clinicId  group by ICDId, ICDCode,  ICDName, FamName 
							end 
					end 
				else if @category = 'PaymentType'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, PaymentType as Category, count(PaymentType) as Total from #TmpDisease where PaymentType = @categoryItem and ClinicId = @clinicId  group by ICDId, ICDCode,  ICDName, PaymentType 
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, PaymentType as Category, count(PaymentType) as Total from #TmpDisease where ClinicId = @clinicId  group by ICDId, ICDCode, ICDName,  PaymentType 
							end 
					end 
				else if @category = 'NeedRest'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, NeedRest as Category, count(NeedRest) as Total from #TmpDisease where NeedRest = @categoryItem and ClinicId = @clinicId  group by ICDId, ICDCode, ICDName,  NeedRest 
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, NeedRest as Category, count(NeedRest) as Total from #TmpDisease where ClinicId = @clinicId  group by ICDId, ICDCode,  ICDName, NeedRest
							end 
					end 
				else if @category = 'ExamineType'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, ExamineType as Category, count(ExamineType) as Total from #TmpDisease where ExamineType = @categoryItem and ClinicId = @clinicId  group by ICDId, ICDCode, ICDName,  ExamineType 
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, ExamineType as Category, count(ExamineType) as Total from #TmpDisease where ClinicId = @clinicId  group by ICDId, ICDCode,  ICDName, ExamineType
							end 
					end 
				else 
					begin 
						select * from #TmpDisease
					end
			end 
		else if @dateStart is not null and @dateEnd is not null and @clinicId is null
			
					if @category = 'Department' 
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId, ICDCode,ICDName, Department as Category, count(Department) as Total from #TmpDisease where Department = @categoryItem  group by ICDId, ICDCode, ICDName, Department
							end 
						else 
							begin 
								select top 10 ICDId, ICDCode,ICDName, Department as Category, count(Department) as Total from #TmpDisease  group by ICDId, ICDCode, ICDName,  Department
							end 
					end 
				else if @category = 'BusinessUnit'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, BusinessUnit as Category, count(BusinessUnit) as Total from #TmpDisease where BusinessUnit = @categoryItem  group by ICDId, ICDCode, ICDName,  BusinessUnit
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, BusinessUnit as Category, count(BusinessUnit) as Total from #TmpDisease  group by ICDId, ICDCode, ICDName,  BusinessUnit
							end 
					end 
				else if @category = 'Gender'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, Gender as Category, count(Gender) as Total from #TmpDisease where Gender = @categoryItem   group by ICDId, ICDCode, ICDName,  Gender
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, Gender as Category, count(Gender) as Total from #TmpDisease group by ICDId, ICDCode, ICDName,  Gender
							end 
					end 
				else if @category = 'Age'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, Age as Category, count(Age) as Total from #TmpDisease where Age = @categoryItem  group by ICDId, ICDCode, ICDName, Age
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, Age as Category, count(Age) as Total from #TmpDisease  group by ICDId, ICDCode, ICDName, Age
							end 
					end 
				else if @category = 'EmploymentType'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, StatusName as Category, count(StatusName) as Total from #TmpDisease where StatusName = @categoryItem group by ICDId, ICDCode, ICDName, StatusName
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, StatusName as Category, count(StatusName) as Total from #TmpDisease  group by ICDId, ICDCode, ICDName, StatusName
							end 
					end 
				else if @category = 'FamilyStatus'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, FamName as Category, count(FamName) as Total from #TmpDisease where FamCode = @categoryItem  group by ICDId, ICDCode, ICDName,  FamName 
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, FamName as Category, count(FamName) as Total from #TmpDisease  group by ICDId, ICDCode,  ICDName, FamName 
							end 
					end 
				else if @category = 'PaymentType'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, PaymentType as Category, count(PaymentType) as Total from #TmpDisease where PaymentType = @categoryItem  group by ICDId, ICDCode,  ICDName, PaymentType
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, PaymentType as Category, count(PaymentType) as Total from #TmpDisease group by ICDId, ICDCode,  ICDName, PaymentType 
							end 
					end 
				else if @category = 'NeedRest'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, NeedRest as Category, count(NeedRest) as Total from #TmpDisease where NeedRest = @categoryItem  group by ICDId, ICDCode, ICDName,  NeedRest 
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, NeedRest as Category, count(NeedRest) as Total from #TmpDisease where ClinicId = @clinicId  group by ICDId, ICDCode, ICDName,  NeedRest
							end 
					end 
				else if @category = 'ExamineType'
					begin 
						if @categoryItem <> '0'
							begin 
								select top 10 ICDId,ICDCode,ICDName, ExamineType as Category, count(ExamineType) as Total from #TmpDisease where ExamineType = @categoryItem  group by ICDId, ICDCode, ICDName,  ExamineType 
							end 
						else 
							begin 
								select top 10 ICDId,ICDCode,ICDName, ExamineType as Category, count(ExamineType) as Total from #TmpDisease  group by ICDId, ICDCode,  ICDName, ExamineType
							end 
					end 
				else 
					begin 
						select * from #TmpDisease
					end
 
	end