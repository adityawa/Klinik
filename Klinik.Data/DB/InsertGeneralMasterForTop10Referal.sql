declare @patientId int  
declare @rujukanId int 
declare @icdId int 
declare @countPatient int 
declare @countRujukan int 
declare @countIcd int 

set @patientId = (select ID from LookupCategory where TypeName = 'Patient')
set @rujukanId = (select ID from LookupCategory where TypeName = 'DoctorAndHospital')
set @icdId = (select ID from LookupCategory where TypeName = 'ICDTheme')

set @countIcd = (select count(*) from GeneralMaster where CategoryId = @icdId)
set @countRujukan = (select count(*) from GeneralMaster where CategoryId = @rujukanId)
set @countPatient = (select count(*) from GeneralMaster where CategoryId = @patientId)

-- delete if exists 
if @countIcd > 0 
	begin
		delete from GeneralMaster where CategoryId = @icdId  
	end 

if @countRujukan > 0 
	begin
		delete from GeneralMaster where CategoryId = @rujukanId  
	end 

if @countPatient > 0 
	begin
		delete from GeneralMaster where CategoryId = @patientId  
	end 

-- insert patientId 
insert into dbo.GeneralMaster(CategoryId,[Type],[Name],[Value],RowStatus, CreatedBy, CreatedDate)
select distinct @patientId, 'Patient', [Name], Convert(nvarchar(max),ID), 0, 'SYSTEM', GetDate() from Patient 

-- insert icd 
insert into dbo.GeneralMaster(CategoryId,[Type],[Name],[Value],RowStatus, CreatedBy, CreatedDate)
select distinct @icdId, 'ICDTheme', concat(Code,'-',[Name]),Id, 0, 'SYSTEM', GetDate() from ICDTheme where Id <> 0

-- insert rujukan 
insert into dbo.GeneralMaster(CategoryId,[Type],[Name],[Value],RowStatus, CreatedBy, CreatedDate)
select distinct @rujukanId,'DoctorAndHospital', OtherInfo, OtherInfo, 0, 'SYSTEM', GetDate() from Letter where OtherInfo is not null