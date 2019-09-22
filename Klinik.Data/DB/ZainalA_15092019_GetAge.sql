create function dbo.GetAge(@dob datetime)
returns decimal
as 
	begin 
		declare @age decimal
		set @age = (SELECT DATEDIFF(hour,@dob,GETDATE())/8766.0 AS AgeYearsDecimal)
		return @age
	end 

