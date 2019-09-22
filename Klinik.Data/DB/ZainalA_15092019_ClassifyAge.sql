create function dbo.GetAgeCategory(@age decimal)
returns nvarchar(20)
as 
	begin 
		declare @category nvarchar(20)

		select @category = case 
								when @age < 18 then '1'
								when @age = 18 then '2'
								when @age > 18 and @age <= 25 then '3'
								when @age > 25 and @age <= 34 then '4'
								when @age > 35 and @age <= 44 then '5'
								when @age > 45 and @age <= 55 then '6'
								when @age > 55 then '7'
								else '0'									
						   end 


		return @category
	end