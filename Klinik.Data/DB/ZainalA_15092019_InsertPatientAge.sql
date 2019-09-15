declare @count int 

set @count = (select count(*) from dbo.PatientAge)

if @count = 0 
	begin
	  -- insert records 
	  insert into dbo.PatientAge(PatientId,Age, AgeCode, CreatedBy, CreatedDate)
	  select  ID, dbo.GetAge(BirthDate) as age, dbo.GetAgeCategory(dbo.GetAge(BirthDate)) as category,'SYSTEM',GetDate()from Patient 

	end 

