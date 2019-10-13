CREATE procedure [dbo].[sp_generateTop10ReferalReport] 
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

		if object_id('TempDB.dbo.#TmpReferal') is not null 
			begin 
				drop table tempdb.dbo.#TmpReferal
			end 
		
		create table #TempReferal(
			LetterId bigint, 
			ClinicId bigint, 
			ClinicName nvarchar(max),
			LetterType nvarchar(max),
			Keperluan nvarchar(max),
			AutoNumber bigint, 
			[Action] nvarchar(max),
			Diagnose nvarchar(max),
			PatientName nvarchar(max),
			OtherInfo nvarchar(max),
			PatientAge nvarchar(max),
			ICDCode nvarchar(max),
			ICDId bigint,
			TransDate datetime		
		);

		-- insert temp table		
		if @dateStart is not null and @dateEnd is not null 
			begin 
				insert into #TempReferal
				select Letter.Id as LetterId, Clinic.Id as ClinicId, Clinic.Name as ClinicName, Letter.LetterType as LetterType,
				Letter.Keperluan, Letter.AutoNumber,Letter.[Action],FormExamine.Diagnose,Patient.Name as PatientName, Letter.OtherInfo,	
				Letter.PatientAge,  ICDTheme.Code as ICDCode, ICDTheme.Id as ICDId, FormExamine.TransDate as TransDate
				from  Letter
				inner join FormMedical on Letter.FormMedicalID = FormMedical.ID
				inner join Patient on FormMedical.PatientID = Patient.ID
				inner join Clinic on Clinic.ID = FormMedical.ClinicID
				inner join FormExamine on FormMedical.Id = FormExamine.FormMedicalID
				inner join FormExamineICDInfo on FormExamineICDInfo.FormExamineId = FormExamine.Id
				inner join ICDTheme on ICDTheme.Id = FormExamineICDInfo.ICDId
				where FormExamine.TransDate between @dateStart and @dateEnd
				order by Letter.Id, ICDTheme.Id

			end 
		else 
			begin 
				insert into #TempReferal
				select Letter.Id as LetterId, Clinic.Id as ClinicId, Clinic.Name as ClinicName, Letter.LetterType as LetterType,
				Letter.Keperluan, Letter.AutoNumber,Letter.[Action],FormExamine.Diagnose,Patient.Name as PatientName, Letter.OtherInfo,	
				Letter.PatientAge,  ICDTheme.Code as ICDCode, ICDTheme.Id as ICDId, FormExamine.TransDate as TransDate
				from  Letter
				inner join FormMedical on Letter.FormMedicalID = FormMedical.ID
				inner join Patient on FormMedical.PatientID = Patient.ID
				inner join Clinic on Clinic.ID = FormMedical.ClinicID
				inner join FormExamine on FormMedical.Id = FormExamine.FormMedicalID
				inner join FormExamineICDInfo on FormExamineICDInfo.FormExamineId = FormExamine.Id
				inner join ICDTheme on ICDTheme.Id = FormExamineICDInfo.ICDId
				order by Letter.Id, ICDTheme.Id
			end 

		if @category = 'ICDTheme' 
			begin 
				if @categoryItem <> '0'
					begin
						select top 10 ClinicId, ClinicName, LetterType, ICDCode as Category, count(ICDCode) as Total from #TempReferal where ICDCode = @categoryItem group by ClinicId,LetterType,ClinicName,ICDCode
					end 
				else 
					begin
						select top 10 ClinicId, ClinicName, LetterType, ICDCode as Category, count(ICDCode) as Total from #TempReferal  group by ClinicId,LetterType,ClinicName,ICDCode
					end 
			end 
		else if @category = 'Patient'
			begin 
				if @categoryItem <> '0'
					begin 
						select top 10 ClinicId, ClinicName, LetterType, PatientName as Category, count(PatientName) as Total from #TempReferal where PatientName = @categoryItem group by ClinicId,LetterType,ClinicName,PatientName  
					end 
				else 
					begin 
						select top 10 ClinicId, ClinicName, LetterType, PatientName as 'Patient Name', count(PatientName) as Total from #TempReferal group by ClinicId,LetterType,ClinicName,PatientName
					end 
			end 
		else if @category = 'DoctorAndHospital'
			begin 
				if @categoryItem <> '0'
					begin 
						select top 10 ClinicId, ClinicName, LetterType, OtherInfo as 'Referal', count(OtherInfo) as Total from #TempReferal where OtherInfo = @categoryItem group by ClinicId,LetterType,ClinicName,OtherInfo
					end 
				else 
					begin 
						select top 10 ClinicId, ClinicName, LetterType, OtherInfo as 'Referal', count(OtherInfo) as Total from #TempReferal group by ClinicId,LetterType,ClinicName,OtherInfo
					end 
			end 
	end