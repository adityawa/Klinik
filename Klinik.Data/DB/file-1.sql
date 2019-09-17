USE [master]
GO
/****** Object:  Database [KlinikDBUAT]    Script Date: 17/09/2019 05:46:47 ******/
CREATE DATABASE [KlinikDBUAT] ON  PRIMARY 
( NAME = N'KlinikDBUAT', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\KlinikDBUAT.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'KlinikDBUAT_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\KlinikDBUAT_log.ldf' , SIZE = 1280KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [KlinikDBUAT] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [KlinikDBUAT].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [KlinikDBUAT] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET ARITHABORT OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [KlinikDBUAT] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [KlinikDBUAT] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET  DISABLE_BROKER 
GO
ALTER DATABASE [KlinikDBUAT] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [KlinikDBUAT] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [KlinikDBUAT] SET RECOVERY FULL 
GO
ALTER DATABASE [KlinikDBUAT] SET  MULTI_USER 
GO
ALTER DATABASE [KlinikDBUAT] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [KlinikDBUAT] SET DB_CHAINING OFF 
GO
USE [KlinikDBUAT]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GenerateMRNumber]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fn_GenerateMRNumber] 
(
	@CreatedTime DATETIME = '2019-08-01 00:00:00',
	@seq INT =0
)
RETURNS varchar(50)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @MRNumber varchar(50) = '', @Month int, @year int,@CountMR INT = 0, @yearmonth VARCHAR(4)

	SET @Month = CONVERT(int,MONTH(@CreatedTime))
	SET @year = CONVERT(int,YEAR(@CreatedTime))
	SET @yearmonth = CONVERT(varchar(2),@Month) + RIGHT(CONVERT(varchar(4),@year),2)
	-- Add the T-SQL statements to compute the return value here
	SELECT TOP 1 @CountMR = CONVERT(int,RIGHT(MRNumber,3))
	FROM Patient WITH (nolock) 
	WHERE MONTH(CreatedDate) = @Month AND YEAR(CreatedDate) = @year AND MRNumber LIKE @yearmonth+'%'
	ORDER BY ID Desc

	SET @CountMR = @CountMR+@seq
	SET @MRNumber = CONVERT(varchar(4),@CountMR)
	SET @MRNumber = CONVERT(varchar(2),@Month) + RIGHT(CONVERT(varchar(4),@year),2) + replicate('0',3-Len(@MRNumber))+@MRNumber

	-- Return the result of the function
	RETURN @MRNumber

END
GO
/****** Object:  UserDefinedFunction [dbo].[GetAge]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[GetAge](@dob datetime)
returns decimal
as 
	begin 
		declare @age decimal
		set @age = (SELECT DATEDIFF(hour,@dob,GETDATE())/8766.0 AS AgeYearsDecimal)
		return @age
	end 

GO
/****** Object:  UserDefinedFunction [dbo].[GetAgeCategory]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[GetAgeCategory](@age decimal)
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
GO
/****** Object:  Table [dbo].[AppConfig]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppConfig](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Value] [varchar](200) NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_AppConfig] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Appointment]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointment](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [bigint] NULL,
	[ClinicID] [bigint] NOT NULL,
	[DoctorID] [int] NULL,
	[RequirementID] [int] NULL,
	[AppointmentDate] [datetime] NOT NULL,
	[MCUPackageID] [int] NULL,
	[PoliID] [int] NULL,
	[Status] [smallint] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [nvarchar](50) NOT NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Appointment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[City]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[City](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Province] [varchar](30) NOT NULL,
	[City] [varchar](50) NOT NULL,
	[Kelurahan] [nvarchar](50) NULL,
	[Kecamatan] [nvarchar](50) NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clinic]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clinic](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NULL,
	[Name] [varchar](200) NULL,
	[Address] [varchar](500) NULL,
	[LegalNumber] [varchar](50) NULL,
	[LegalDate] [date] NULL,
	[ContactNumber] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Lat] [float] NULL,
	[Long] [float] NULL,
	[CityID] [int] NULL,
	[ClinicType] [smallint] NULL,
	[ReffID] [int] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_clinic] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliveryOrder]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryOrder](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[poid] [int] NOT NULL,
	[donumber] [varchar](20) NULL,
	[dodate] [date] NULL,
	[dodest] [varchar](30) NULL,
	[approveby] [varchar](100) NULL,
	[approve] [int] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[Recived] [int] NULL,
	[GudangId] [int] NULL,
	[SendBy] [varchar](100) NULL,
	[SourceId] [int] NULL,
 CONSTRAINT [PK_DeliveryOrder] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliveryOrderDetail]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryOrderDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DeliveryOderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[namabarang] [varchar](30) NOT NULL,
	[qty_request] [float] NULL,
	[nama_by_ho] [varchar](50) NULL,
	[qty_by_HP] [float] NULL,
	[remark_by_ho] [varchar](50) NULL,
	[qty_adj] [float] NULL,
	[remark_adj] [varchar](30) NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[RowStatus] [smallint] NULL,
	[Recived] [bit] NULL,
 CONSTRAINT [PK_DeliveryOrderDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliveryOrderPusat]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryOrderPusat](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[poid] [int] NOT NULL,
	[donumber] [varchar](20) NULL,
	[dodate] [date] NULL,
	[dodest] [varchar](30) NULL,
	[approve_by] [varchar](100) NULL,
	[approve] [int] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[StatusSop] [int] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[Recived] [int] NULL,
	[GudangId] [int] NULL,
	[SendBy] [varchar](100) NULL,
	[Validasi] [int] NULL,
 CONSTRAINT [PK_DeliveryOrderPusat] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliveryOrderPusatDetail]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryOrderPusatDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DeliveryOrderPusatId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[namabarang] [varchar](30) NOT NULL,
	[VendorId] [int] NOT NULL,
	[namavendor] [varchar](30) NOT NULL,
	[satuan] [float] NULL,
	[harga] [float] NULL,
	[stok_prev] [float] NULL,
	[total_req] [float] NULL,
	[total_dist] [float] NULL,
	[sisa_stok] [float] NULL,
	[qty] [float] NULL,
	[qty_add] [float] NULL,
	[reason_add] [varchar](50) NULL,
	[qty_final] [float] NULL,
	[remark] [varchar](100) NULL,
	[total] [float] NULL,
	[qty_unit] [float] NULL,
	[qty_box] [float] NULL,
	[Recived] [bit] NULL,
	[No_Do_Vendor] [varchar](100) NULL,
	[Tgl_Do_Vendor] [datetime] NULL,
	[SendBy] [varchar](100) NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[statusop] [int] NULL,
 CONSTRAINT [PK__PurchasePusatD__3213E83FAC292246] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Doctor]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Doctor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [char](10) NULL,
	[Name] [varchar](100) NULL,
	[SpecialistID] [int] NULL,
	[TypeID] [int] NULL,
	[KTPNumber] [varchar](50) NULL,
	[STRNumber] [varchar](50) NULL,
	[STRValidFrom] [datetime] NULL,
	[STRValidTo] [datetime] NULL,
	[Address] [varchar](200) NULL,
	[HPNumber] [varchar](20) NULL,
	[Email] [varchar](50) NULL,
	[Remark] [varchar](300) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[EmployeeID] [bigint] NOT NULL,
 CONSTRAINT [PK_Doctor] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DoctorClinic]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DoctorClinic](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [char](10) NULL,
	[ClinicID] [bigint] NOT NULL,
	[DoctorID] [int] NOT NULL,
	[PhotoID] [bigint] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_DoctorClinic] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EmpID] [varchar](50) NOT NULL,
	[EmpName] [varchar](100) NOT NULL,
	[BirthDate] [date] NULL,
	[ReffEmpID] [varchar](50) NULL,
	[Gender] [char](1) NULL,
	[EmpType] [smallint] NULL,
	[KTPNumber] [varchar](max) NULL,
	[HPNumber] [varchar](max) NULL,
	[Email] [varchar](50) NULL,
	[LastEmpID] [varchar](50) NULL,
	[Status] [smallint] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeAssignment]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeAssignment](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [bigint] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Department] [varchar](100) NULL,
	[BusinessUnit] [varchar](100) NULL,
	[Region] [varchar](100) NULL,
	[Grade] [nvarchar](50) NULL,
	[EmpStatus] [smallint] NULL,
	[LastEmpID] [varchar](50) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_EmployeeAssignment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeStatus]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeStatus](
	[ID] [smallint] IDENTITY(1,1) NOT NULL,
	[Code] [char](10) NULL,
	[Name] [varchar](50) NULL,
	[Status] [char](1) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_EmployeeStatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FamilyRelationship]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FamilyRelationship](
	[ID] [smallint] NOT NULL,
	[Code] [char](3) NULL,
	[Name] [varchar](10) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FamilyRelationship] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FileArchieve]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileArchieve](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SourceTable] [varchar](50) NULL,
	[ActualPath] [varchar](300) NULL,
	[ActualName] [varchar](50) NULL,
	[TypeDoc] [varchar](10) NULL,
	[FileDoc] [varbinary](1024) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FileArchieve] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormExamine]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormExamine](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FormMedicalID] [bigint] NULL,
	[PoliID] [int] NULL,
	[TransDate] [datetime] NULL,
	[DoctorID] [int] NULL,
	[Anamnesa] [text] NULL,
	[Diagnose] [text] NULL,
	[Therapy] [text] NULL,
	[Remark] [text] NULL,
	[ICDInformation] [varchar](250) NULL,
	[Result] [nvarchar](250) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FormExamine1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormExamineAttachment]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormExamineAttachment](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FormExamineID] [bigint] NULL,
	[FileAttach] [bigint] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FormExamineAttachment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormExamineLab]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormExamineLab](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FormMedicalID] [bigint] NULL,
	[LabType] [nvarchar](250) NULL,
	[LabItemID] [int] NULL,
	[Result] [varchar](50) NULL,
	[ResultIndicator] [char](1) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FormExamineLab] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormExamineMedicine]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormExamineMedicine](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FormExamineID] [bigint] NULL,
	[TypeID] [nvarchar](250) NULL,
	[ProductID] [int] NULL,
	[Qty] [float] NULL,
	[Dose] [nvarchar](50) NULL,
	[ConcoctionMedicine] [text] NULL,
	[RemarkUse] [varchar](50) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[JenisObat] [varchar](20) NULL,
 CONSTRAINT [PK_FormExamineMedicine] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormExamineMedicineDetail]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormExamineMedicineDetail](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FormExamineMedicineID] [bigint] NULL,
	[ProductID] [int] NULL,
	[ProductName] [nvarchar](150) NULL,
	[Qty] [float] NULL,
	[Note] [nvarchar](100) NULL,
	[ProcessType] [nvarchar](50) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[Status] [varchar](5) NULL,
	[TanggalAmbilObat] [datetime] NULL,
	[TanggalReadyObat] [datetime] NULL,
 CONSTRAINT [PK_FormExamineMedicineDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormExamineService]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormExamineService](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FormExamineID] [bigint] NULL,
	[ServiceID] [int] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FormExamineService] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormMedical]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormMedical](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ClinicID] [bigint] NULL,
	[PatientID] [bigint] NULL,
	[Necessity] [nvarchar](250) NULL,
	[PaymentType] [nvarchar](250) NULL,
	[Number] [varchar](50) NULL,
	[ClaimNumber] [varchar](50) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[TotalPrice] [money] NULL,
	[DiscountPercent] [float] NULL,
	[DiscountAmount] [money] NULL,
	[BenefitPaid] [money] NULL,
	[BenefitPlan] [nchar](10) NULL,
	[Remark] [varchar](255) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FormMedical] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormPreExamine]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormPreExamine](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FormMedicalID] [bigint] NOT NULL,
	[TransDate] [datetime] NOT NULL,
	[DoctorID] [int] NULL,
	[Temperature] [float] NULL,
	[Weight] [float] NULL,
	[Height] [float] NULL,
	[Respiratory] [float] NULL,
	[Pulse] [int] NULL,
	[Systolic] [int] NULL,
	[Diastolic] [int] NULL,
	[Others] [text] NULL,
	[RightEye] [varchar](50) NULL,
	[LeftEye] [varchar](50) NULL,
	[ColorBlind] [nvarchar](250) NULL,
	[MenstrualDate] [datetime] NULL,
	[KBDate] [datetime] NULL,
	[DailyGlasses] [nvarchar](250) NULL,
	[ExamineGlasses] [nvarchar](250) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FormExamine] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GeneralMaster]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeneralMaster](
	[ID] [smallint] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Value] [nvarchar](250) NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_GeneralMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gudang]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Gudang](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[ClinicId] [bigint] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[RowStatus] [smallint] NULL,
 CONSTRAINT [PK_Warehouses] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HistoryProductInGudang]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistoryProductInGudang](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[GudangId] [int] NOT NULL,
	[value] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_HistoryProductInGudang] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ICDTheme]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ICDTheme](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Name] [nvarchar](50) NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_ICDTheme] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LabItem]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LabItem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Name] [varchar](200) NULL,
	[LabItemCategoryID] [int] NULL,
	[Normal] [varchar](50) NULL,
	[Price] [money] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_LabItem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LabItemCategory]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LabItemCategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LabType] [nvarchar](250) NULL,
	[PoliID] [int] NULL,
	[Name] [varchar](50) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_LabCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Letter]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Letter](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LetterType] [varchar](50) NOT NULL,
	[AutoNumber] [bigint] NOT NULL,
	[Year] [int] NOT NULL,
	[Keperluan] [text] NULL,
	[Decision] [varchar](50) NULL,
	[Treatment] [varchar](50) NULL,
	[Action] [nvarchar](50) NULL,
	[ReferenceTo] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[ForPatient] [bigint] NULL,
	[ResponsiblePerson] [nvarchar](max) NULL,
	[PatientAge] [varchar](50) NULL,
	[FormMedicalID] [bigint] NULL,
	[Cekdate] [datetime] NULL,
	[Pekerjaan] [nvarchar](50) NULL,
	[OtherInfo] [nvarchar](max) NULL,
 CONSTRAINT [PK_Letter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Start] [datetime] NOT NULL,
	[Module] [varchar](50) NOT NULL,
	[Account] [bigint] NOT NULL,
	[Command] [text] NULL,
	[OldValue] [nvarchar](max) NULL,
	[NewValue] [nvarchar](max) NULL,
	[Status] [varchar](50) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LookupCategory]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LookupCategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LookUpCode] [varchar](10) NOT NULL,
	[LookUpName] [varchar](50) NOT NULL,
	[LookupContent] [varchar](max) NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_LookupCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MCUPackage]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MCUPackage](
	[ID] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[Gender] [char](2) NULL,
	[AgeStart] [int] NULL,
	[AgeEnd] [int] NULL,
	[GradeID] [int] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[NodifiedDate] [datetime] NULL,
 CONSTRAINT [PK_MCUPackage] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MCURegistrationInterface]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MCURegistrationInterface](
	[REG_ID] [int] IDENTITY(1,1) NOT NULL,
	[REG_NUMBER] [varchar](20) NULL,
	[RESERVE_DATE] [date] NULL,
	[EMPL_ID] [int] NULL,
	[EMPL_NAME] [varchar](50) NULL,
	[SCHEDULE_ID] [int] NULL,
	[SCHEDULE_CODE] [varchar](10) NULL,
	[DOCUMENT_STATUS] [varchar](50) NULL,
	[IS_TRANSFERRED] [char](1) NULL,
	[CREATED_DATE] [datetime] NULL,
	[CREATED_BY_NAME] [varchar](50) NULL,
	[MODIFIED_DATE] [datetime] NULL,
	[MODIFIED_BY_NAME] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Medicine]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Medicine](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Medicine] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[ParentMenuId] [bigint] NULL,
	[PageLink] [nvarchar](max) NULL,
	[SortIndex] [int] NOT NULL,
	[HasChild] [bit] NULL,
	[IsMenu] [bit] NULL,
	[Name] [varchar](50) NULL,
	[Controller] [varchar](50) NULL,
	[Action] [varchar](50) NULL,
	[Level] [int] NULL,
	[Icon] [nvarchar](50) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organization]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organization](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrgCode] [nvarchar](30) NOT NULL,
	[OrgName] [nvarchar](50) NOT NULL,
	[KlinikID] [bigint] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganizationPrivilege]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganizationPrivilege](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrgID] [bigint] NOT NULL,
	[PrivilegeID] [bigint] NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_OrganizationPrivilege] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganizationRole]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganizationRole](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrgID] [bigint] NOT NULL,
	[RoleName] [varchar](30) NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_OrganizationRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PanggilanPoli]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PanggilanPoli](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[QueueCode] [nvarchar](50) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[PoliID] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[SortNumber] [int] NOT NULL,
 CONSTRAINT [PK_PanggilanPoli] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PasswordHistory]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrganizationID] [bigint] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PasswordHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Patient]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Patient](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [bigint] NULL,
	[FamilyRelationshipID] [smallint] NULL,
	[MRNumber] [varchar](50) NULL,
	[Name] [varchar](100) NOT NULL,
	[Gender] [char](1) NOT NULL,
	[MaritalStatus] [char](1) NULL,
	[BirthDate] [datetime] NOT NULL,
	[KTPNumber] [varchar](20) NULL,
	[Address] [varchar](100) NOT NULL,
	[CityID] [int] NULL,
	[HPNumber] [nvarchar](50) NULL,
	[Type] [smallint] NULL,
	[BPJSNumber] [varchar](50) NULL,
	[BloodType] [char](2) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[PatientKey] [nvarchar](100) NULL,
 CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PatientAge]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PatientAge](
	[Id] [int] NOT NULL,
	[PatientId] [int] NULL,
	[Age] [int] NOT NULL,
	[AgeCode] [nvarchar](50) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreateDate] [datetime] NULL,
	[ModifiedBy] [varchar](100) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PatientAge] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PatientClinic]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PatientClinic](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PatientID] [bigint] NOT NULL,
	[ClinicID] [bigint] NOT NULL,
	[TempAddress] [varchar](100) NULL,
	[TempCityID] [int] NULL,
	[RefferencePerson] [varchar](50) NULL,
	[RefferenceNumber] [varchar](20) NULL,
	[RefferenceRelation] [int] NULL,
	[PhotoID] [bigint] NULL,
	[OldMRNumber] [varchar](50) NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PatientClinic] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Poli]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Poli](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [char](2) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Type] [smallint] NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Poli] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PoliClinic]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoliClinic](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ClinicID] [bigint] NOT NULL,
	[PoliID] [int] NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[Createddate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PoliClinic] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PoliFlowTemplate]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoliFlowTemplate](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[PoliTypeID] [smallint] NOT NULL,
	[PoliTypeIDTo] [smallint] NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PoliFlowTemplate] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PoliSchedule]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoliSchedule](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ClinicID] [bigint] NOT NULL,
	[DoctorID] [int] NOT NULL,
	[PoliID] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[ReffID] [bigint] NULL,
	[Remark] [varchar](150) NULL,
	[Status] [int] NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PoliSchedule] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PoliScheduleMaster]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoliScheduleMaster](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ClinicID] [bigint] NOT NULL,
	[DoctorID] [int] NOT NULL,
	[PoliID] [int] NOT NULL,
	[Day] [int] NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PoliScheduleMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PoliServices]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoliServices](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ServicesID] [int] NOT NULL,
	[ClinicID] [bigint] NOT NULL,
	[PoliID] [int] NOT NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_PoliServices] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Privilege]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Privilege](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Privilege_Name] [nvarchar](150) NOT NULL,
	[Privilege_Desc] [nvarchar](500) NULL,
	[MenuID] [bigint] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Privilege] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NULL,
	[Name] [varchar](200) NULL,
	[ClinicID] [bigint] NULL,
	[Vendor] [varchar](100) NULL,
	[ProductCategoryID] [int] NOT NULL,
	[ProductUnitID] [int] NOT NULL,
	[RetailPrice] [decimal](19, 4) NOT NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductInGudang]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductInGudang](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[GudangId] [int] NULL,
	[ProductId] [int] NULL,
	[stock] [int] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[RowStatus] [int] NULL,
 CONSTRAINT [PK_ProductInGudang] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductMedicine]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductMedicine](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[MedicineID] [int] NOT NULL,
	[Amount] [float] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_ProductMedicine] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductUnit]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductUnit](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NULL,
	[Name] [varchar](200) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_ProductUnit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrder](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseRequestId] [int] NULL,
	[ponumber] [varchar](20) NULL,
	[podate] [date] NULL,
	[request_by] [varchar](30) NULL,
	[approve_by] [varchar](30) NULL,
	[approve] [int] NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[statusop] [int] NULL,
	[Validasi] [int] NULL,
	[GudangId] [int] NULL,
	[SourceId] [int] NULL,
 CONSTRAINT [PK__Purchase__3213E83FCF741E2D] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrderDetail]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[namabarang] [varchar](30) NULL,
	[tot_pemakaian] [float] NULL,
	[sisa_stok] [float] NULL,
	[qty] [float] NULL,
	[qty_add] [float] NULL,
	[reason_add] [varchar](50) NULL,
	[total] [float] NOT NULL,
	[nama_by_ho] [varchar](30) NULL,
	[qty_by_ho] [float] NULL,
	[remark_by_ho] [varchar](30) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](100) NULL,
	[RowStatus] [int] NULL,
	[statusop] [int] NULL,
	[OrderNumber] [int] NULL,
	[Verified] [bit] NULL,
 CONSTRAINT [PK__Purchase__3213E83FAC292246] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrderPusat]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderPusat](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseRequestId] [int] NULL,
	[ponumber] [varchar](20) NULL,
	[podate] [date] NULL,
	[request_by] [varchar](30) NULL,
	[approve_by] [varchar](30) NULL,
	[approve] [int] NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[statusop] [int] NULL,
	[Validasi] [int] NULL,
	[GudangId] [int] NULL,
 CONSTRAINT [PK__PurchasePusat__3213E83FCF741E2D] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrderPusatDetail]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderPusatDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderPusatId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[namabarang] [varchar](30) NOT NULL,
	[VendorId] [int] NOT NULL,
	[namavendor] [varchar](30) NOT NULL,
	[satuan] [float] NULL,
	[harga] [float] NULL,
	[stok_prev] [float] NULL,
	[total_req] [float] NULL,
	[total_dist] [float] NULL,
	[sisa_stok] [float] NULL,
	[qty] [float] NULL,
	[qty_add] [float] NULL,
	[reason_add] [varchar](50) NULL,
	[qty_final] [float] NULL,
	[remark] [varchar](100) NULL,
	[total] [float] NULL,
	[qty_unit] [float] NULL,
	[qty_box] [float] NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[statusop] [int] NULL,
	[OrderNumber] [int] NULL,
	[Verified] [bit] NULL,
 CONSTRAINT [PK__PurchasePusat__3213E83FAC292246] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseRequest]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseRequest](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[prnumber] [varchar](20) NULL,
	[prdate] [date] NULL,
	[request_by] [varchar](30) NULL,
	[approve_by] [varchar](30) NULL,
	[approve] [int] NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[statusop] [int] NULL,
	[Validasi] [int] NULL,
	[GudangId] [int] NULL,
 CONSTRAINT [PK__PurchaseRequest__3213E83FCF741E2D] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseRequestConfig]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseRequestConfig](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[GudangId] [int] NULL,
	[StartDate] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](30) NULL,
	[RowStatus] [int] NULL,
 CONSTRAINT [PK__PurchaseRC__3213E83FAC292246] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseRequestDetail]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseRequestDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseRequestId] [int] NOT NULL,
	[ProductId] [int] NULL,
	[namabarang] [varchar](30) NULL,
	[tot_pemakaian] [float] NULL,
	[sisa_stok] [float] NULL,
	[qty] [float] NULL,
	[qty_add] [float] NULL,
	[reason_add] [varchar](50) NULL,
	[total] [float] NOT NULL,
	[nama_by_ho] [varchar](30) NULL,
	[qty_by_ho] [float] NULL,
	[remark_by_ho] [varchar](30) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](30) NULL,
	[RowStatus] [int] NULL,
	[statusop] [int] NULL,
 CONSTRAINT [PK__PurchaseRD__3213E83FAC292246] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseRequestPusat]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseRequestPusat](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[prnumber] [varchar](20) NULL,
	[prdate] [date] NULL,
	[request_by] [varchar](30) NULL,
	[approve_by] [varchar](30) NULL,
	[approve] [int] NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[statusop] [int] NULL,
	[Validasi] [int] NULL,
	[GudangId] [int] NULL,
 CONSTRAINT [PK__PurchaseRequestP__3213E83FCF741E2D] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseRequestPusatDetail]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseRequestPusatDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseRequestPusatId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[namabarang] [varchar](30) NOT NULL,
	[VendorId] [int] NOT NULL,
	[namavendor] [varchar](30) NOT NULL,
	[satuan] [float] NULL,
	[harga] [float] NULL,
	[stok_prev] [float] NULL,
	[total_req] [float] NULL,
	[total_dist] [float] NULL,
	[sisa_stok] [float] NULL,
	[qty] [float] NULL,
	[qty_add] [float] NULL,
	[reason_add] [varchar](50) NULL,
	[qty_final] [float] NULL,
	[remark] [varchar](100) NULL,
	[total] [float] NULL,
	[qty_unit] [float] NULL,
	[qty_box] [float] NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[statusop] [int] NULL,
 CONSTRAINT [PK__PurchasePusatR__3213E83FAC292246] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QueuePoli]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QueuePoli](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ClinicID] [bigint] NOT NULL,
	[PatientID] [bigint] NOT NULL,
	[DoctorID] [int] NULL,
	[FormMedicalID] [bigint] NULL,
	[TransactionDate] [datetime] NOT NULL,
	[Type] [smallint] NOT NULL,
	[AppointmentID] [bigint] NULL,
	[SortNumber] [int] NOT NULL,
	[PoliFrom] [int] NOT NULL,
	[PoliTo] [int] NOT NULL,
	[Remark] [varchar](100) NULL,
	[ReffID] [bigint] NULL,
	[Status] [int] NULL,
	[IsPreExamine] [bit] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_QueuePoli] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePrivilege]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePrivilege](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleID] [bigint] NOT NULL,
	[PrivilegeID] [bigint] NOT NULL,
	[MenuID] [bigint] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_RolePrivilege] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Services]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Services](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NULL,
	[Name] [varchar](100) NULL,
	[Price] [money] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Services] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[stok]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[stok](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[ClinicId] [bigint] NOT NULL,
	[stok_real] [float] NULL,
	[stok_prev] [float] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[RowStatus] [int] NULL,
	[statusop] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[ClinicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[stok_bulanan]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[stok_bulanan](
	[ProductId] [int] NOT NULL,
	[ClinicId] [bigint] NOT NULL,
	[thnbln] [datetime] NOT NULL,
	[stok] [float] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[RowStatus] [int] NULL,
	[statusop] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[ClinicId] ASC,
	[thnbln] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[stoks]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[stoks](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[ClinicId] [bigint] NOT NULL,
	[stok_real] [float] NULL,
	[stok_prev] [float] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[RowStatus] [int] NULL,
	[statusop] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[ClinicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[substitute]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[substitute](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[namabarang] [varchar](50) NOT NULL,
	[qty] [float] NULL,
	[PurchaseOrderDetailId] [int] NULL,
 CONSTRAINT [PK_do_substitute] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SuratRujukanLabKeluar]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuratRujukanLabKeluar](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoSurat] [nvarchar](50) NULL,
	[FormMedicalID] [bigint] NULL,
	[DokterPengirim] [varchar](50) NULL,
	[LabItemId] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_SuratRujukanLabKeluar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrganizationID] [bigint] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
	[EmployeeID] [bigint] NULL,
	[ExpiredDate] [datetime] NULL,
	[ResetPasswordCode] [nvarchar](50) NULL,
	[Status] [bit] NULL,
	[RowStatus] [smallint] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NOT NULL,
	[RoleID] [bigint] NOT NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendor]    Script Date: 17/09/2019 05:46:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendor](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[namavendor] [varchar](30) NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_m_vendor] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[City] ON 
GO
INSERT [dbo].[City] ([Id], [Province], [City], [Kelurahan], [Kecamatan], [CreatedBy], [CreatedDate]) VALUES (1, N'Riau', N'Kerinci', N'-', N'-', N'SYSTEM', CAST(N'2019-09-14T10:25:56.680' AS DateTime))
GO
INSERT [dbo].[City] ([Id], [Province], [City], [Kelurahan], [Kecamatan], [CreatedBy], [CreatedDate]) VALUES (2, N'Riau', N'Pekanbaru', N'-', N'-', N'SYSTEM', CAST(N'2019-09-14T10:25:56.680' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[City] OFF
GO
SET IDENTITY_INSERT [dbo].[Clinic] ON 
GO
INSERT [dbo].[Clinic] ([ID], [Code], [Name], [Address], [LegalNumber], [LegalDate], [ContactNumber], [Email], [Lat], [Long], [CityID], [ClinicType], [ReffID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'K0001', N'Klinik Town Site', N'jalan kerinci 1', N'1234', CAST(N'2019-01-10' AS Date), N'123/PKS/I/2019', N'townsite1@gmail.com', 0, 0, 30, 0, 0, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:45:55.650' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Clinic] OFF
GO
SET IDENTITY_INSERT [dbo].[Doctor] ON 
GO
INSERT [dbo].[Doctor] ([ID], [Code], [Name], [SpecialistID], [TypeID], [KTPNumber], [STRNumber], [STRValidFrom], [STRValidTo], [Address], [HPNumber], [Email], [Remark], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [EmployeeID]) VALUES (6, N'D1648     ', N'dr bambang', 1, 0, N'645645654654654', N'53544', CAST(N'2018-07-12T00:00:00.000' AS DateTime), CAST(N'2020-12-24T00:00:00.000' AS DateTime), N'RUKO MANGGA DUA A9 NO 7 JL JAGIR WONOKROMO', N'3312424', N'BENICHISTER@GMAIL.COM', NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T17:55:47.877' AS DateTime), NULL, NULL, 3)
GO
INSERT [dbo].[Doctor] ([ID], [Code], [Name], [SpecialistID], [TypeID], [KTPNumber], [STRNumber], [STRValidFrom], [STRValidTo], [Address], [HPNumber], [Email], [Remark], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [EmployeeID]) VALUES (7, N'D7533     ', N'nina', 4, 1, N'1234567834561234', N'4423423', CAST(N'2014-10-28T00:00:00.000' AS DateTime), CAST(N'2020-11-26T00:00:00.000' AS DateTime), N'RUKO MANGGA DUA A9 NO 7 JL JAGIR WONOKROMO', N'543543', N'BENICHISTER@GMAIL.COM', NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:16:58.450' AS DateTime), NULL, NULL, 4)
GO
SET IDENTITY_INSERT [dbo].[Doctor] OFF
GO
SET IDENTITY_INSERT [dbo].[Employee] ON 
GO
INSERT [dbo].[Employee] ([ID], [EmpID], [EmpName], [BirthDate], [ReffEmpID], [Gender], [EmpType], [KTPNumber], [HPNumber], [Email], [LastEmpID], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'D1648', N'dr bambang', CAST(N'1971-02-10' AS Date), NULL, N'M', NULL, N'645645654654654', N'3312424', N'BENICHISTER@GMAIL.COM', NULL, 0, 0, N'M008-adminsuper', CAST(N'2019-09-14T17:55:47.827' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Employee] ([ID], [EmpID], [EmpName], [BirthDate], [ReffEmpID], [Gender], [EmpType], [KTPNumber], [HPNumber], [Email], [LastEmpID], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'D7533', N'nina', CAST(N'1990-02-06' AS Date), NULL, N'F', NULL, N'1234567834561234', N'543543', N'BENICHISTER@GMAIL.COM', NULL, 0, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:16:58.367' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Employee] OFF
GO
SET IDENTITY_INSERT [dbo].[EmployeeStatus] ON 
GO
INSERT [dbo].[EmployeeStatus] ([ID], [Code], [Name], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (0, N'NS        ', N'Not Specified', N'F', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[EmployeeStatus] ([ID], [Code], [Name], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'C         ', N'Contract', N'A', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[EmployeeStatus] ([ID], [Code], [Name], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'P         ', N'Permanent', N'A', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[EmployeeStatus] ([ID], [Code], [Name], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'PKWT      ', N'Pre Permanent', N'A', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[EmployeeStatus] ([ID], [Code], [Name], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'R         ', N'Resign', N'F', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[EmployeeStatus] ([ID], [Code], [Name], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'Ps        ', N'Pension', N'F', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[EmployeeStatus] ([ID], [Code], [Name], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'Na        ', N'Not Active', N'F', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[EmployeeStatus] OFF
GO
INSERT [dbo].[FamilyRelationship] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'E  ', N'Employee', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[FamilyRelationship] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'S  ', N'Spouse', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[FamilyRelationship] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'C1 ', N'Child 1', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[FamilyRelationship] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'C2 ', N'Child 2', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[FamilyRelationship] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'C3 ', N'Child 3', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[FamilyRelationship] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'C4 ', N'Child 4', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[FamilyRelationship] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'C5 ', N'Child 5', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[GeneralMaster] ON 
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'EmploymentType', N'Permanent', N'1', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'EmploymentType', N'Outsource', N'2', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'EmploymentType', N'PKWT', N'3', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'Department', N'ISA', N'1', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'Department', N'Sales', N'2', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'Department', N'ERP', N'3', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'Department', N'HRD', N'4', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'DoctorType', N'Umum', N'1', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, N'DoctorType', N'Gigi', N'2', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, N'DoctorType', N'PenyakitDalam', N'3', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, N'DoctorType', N'Kulit', N'4', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, N'DoctorType', N'Mata', N'5', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, N'DoctorType', N'THT', N'6', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, N'DoctorType', N'Anak', N'7', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (15, N'DoctorType', N'Syaraf', N'8', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (16, N'PoliScheduleStatus', N'Aktif', N'1', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (17, N'PoliScheduleStatus', N'Tidak Aktif', N'0', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (18, N'ParamedicType', N'Farmasi', N'0', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (19, N'ParamedicType', N'Laboratorium', N'1', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20, N'ParamedicType', N'Radiologi', N'2', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (21, N'ParamedicType', N'MedicalRecord', N'3', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (22, N'ParamedicType', N'Cashier', N'4', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (23, N'Day', N'Monday', N'1', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (24, N'Day', N'Tuesday', N'2', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (25, N'Day', N'Wednesday', N'3', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (26, N'Day', N'Thursday', N'4', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (27, N'Day', N'Friday', N'5', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (28, N'Day', N'Saturday', N'6', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (29, N'Day', N'Sunday', N'7', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30, N'City', N'Kerinci', N'1', 0, N'SYSTEM', CAST(N'2019-03-23T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (31, N'City', N'Pekanbaru', N'2', 0, N'SYSTEM', CAST(N'2019-03-23T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (32, N'City', N'Lainnya', N'3', 0, N'SYSTEM', CAST(N'2019-03-23T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (33, N'CIty', N'Solo', N'4', -1, N'SYSTEM', CAST(N'2019-03-23T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (34, N'PatientType', N'General', N'1', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (35, N'PatientType', N'Company', N'2', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (36, N'Marital', N'Single', N'S', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (37, N'Marital', N'Married', N'M', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (38, N'Marital', N'Divorced', N'D', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (39, N'Marital', N'Widowed', N'W', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40, N'Relation', N'Adik', N'1', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (41, N'Relation', N'Kakak', N'2', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (42, N'Relation', N'Istri', N'3', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (43, N'Relation', N'Orang Tua', N'4', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (44, N'NecessityType', N'Berobat', N'1', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (45, N'NecessityType', N'Check up', N'2', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (46, N'NecessityType', N'Konsultasi', N'3', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (47, N'NecessityType', N'Periksa Kehamilan', N'4', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (48, N'PaymentType', N'Umum', N'1', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (49, N'PaymentType', N'BPJS', N'2', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50, N'PaymentType', N'Asuransi', N'3', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (51, N'PaymentType', N'Company', N'4', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (52, N'NecessityType', N'Imunisasi', N'5', 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (55, N'PoliType', N'Loket', N'Group 0', 0, N'SYSTEM', CAST(N'2019-01-01T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (56, N'PoliType', N'Poli Umum', N'Group 1', 0, N'SYSTEM', CAST(N'2019-01-01T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (57, N'PoliType', N'Poli Gigi', N'Group 2', 0, N'SYSTEM', CAST(N'2019-01-01T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (58, N'PoliType', N'Poli Spesialis', N'Group 3', 0, N'SYSTEM', CAST(N'2019-01-01T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (59, N'PoliType', N'Lainnya', N'Group 4', 0, N'SYSTEM', CAST(N'2019-01-01T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[GeneralMaster] ([ID], [Type], [Name], [Value], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (60, N'PoliType', N'Kasir', N'Group 5', 0, N'SYSTEM', CAST(N'2019-01-01T00:00:00.000' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[GeneralMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[Gudang] ON 
GO
INSERT [dbo].[Gudang] ([id], [name], [ClinicId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [RowStatus]) VALUES (1, N'Gudang Town Site 1', 1, N'M008-adminsuper', CAST(N'2019-09-14T12:19:06.983' AS DateTime), NULL, NULL, 0)
GO
SET IDENTITY_INSERT [dbo].[Gudang] OFF
GO
SET IDENTITY_INSERT [dbo].[LabItemCategory] ON 
GO
INSERT [dbo].[LabItemCategory] ([ID], [LabType], [PoliID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Lab', 12, N'Faal Ginjal', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:22:04.050' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LabItemCategory] ([ID], [LabType], [PoliID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'Lab', 12, N'Urine Rutin', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:22:16.440' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LabItemCategory] ([ID], [LabType], [PoliID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'Lab', 12, N'Darah Lengkap', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:22:50.490' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LabItemCategory] ([ID], [LabType], [PoliID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'Lab', 12, N'PROFIL LIPID', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:23:00.653' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LabItemCategory] ([ID], [LabType], [PoliID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'Lab', 12, N'GULA DARAH', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:23:11.803' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LabItemCategory] ([ID], [LabType], [PoliID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'Lab', 12, N'TES ARTHRITIS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:23:34.963' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LabItemCategory] ([ID], [LabType], [PoliID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'Spirometri', 11, N'SPIROMETRI', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:24:37.997' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LabItemCategory] ([ID], [LabType], [PoliID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'Audiometri', 11, N'AUDIOMETRI', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:25:19.643' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[LabItemCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[Log] ON 
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, CAST(N'2019-09-14T10:09:04.810' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:09:04.810' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, CAST(N'2019-09-14T10:12:56.437' AS DateTime), N'MASTER_ROLE', 0, N'Add New Role', NULL, N'{"OrgID":1,"OrganizationName":null,"RoleName":"IT Administrator","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:12:56.477' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, CAST(N'2019-09-14T10:14:12.357' AS DateTime), N'MASTER_ROLE', 0, N'Add New Role', NULL, N'{"OrgID":1,"OrganizationName":null,"RoleName":"Human Resources","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:14:12.357' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, CAST(N'2019-09-14T10:14:27.293' AS DateTime), N'MASTER_ROLE', 0, N'Add New Role', NULL, N'{"OrgID":1,"OrganizationName":null,"RoleName":"Warehouse","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:14:27.293' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, CAST(N'2019-09-14T10:14:35.483' AS DateTime), N'MASTER_ROLE', 0, N'Add New Role', NULL, N'{"OrgID":1,"OrganizationName":null,"RoleName":"Employee","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:14:35.483' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, CAST(N'2019-09-14T10:34:13.307' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:34:13.307' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, CAST(N'2019-09-14T10:45:55.720' AS DateTime), N'MASTER_CLINIC', 0, N'Add New Clinic', NULL, N'{"Code":"K0001","Name":"Klinik Town Site","Address":"jalan kerinci 1","LegalNumber":"1234","LegalDate":"2019-01-10T00:00:00","LegalDateDesc":null,"Email":"townsite1@gmail.com","ContactNumber":"123/PKS/I/2019","Long":0.0,"Lat":0.0,"CityId":30,"CityDesc":null,"ClinicType":0,"ClinicTypeDesc":null,"ReffID":0,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:45:55.780' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, CAST(N'2019-09-14T11:32:34.950' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T11:32:34.950' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, CAST(N'2019-09-14T11:43:38.270' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T11:43:38.273' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, CAST(N'2019-09-14T12:17:28.773' AS DateTime), N'MASTER_SERVICE', 0, N'Add New Service', NULL, N'{"Code":"S001","Name":"Chest X-Ray","Price":18050.0,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:17:28.833' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, CAST(N'2019-09-14T12:17:45.310' AS DateTime), N'MASTER_SERVICE', 0, N'Add New Service', NULL, N'{"Code":"S002","Name":"EKG","Price":13236.0,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:17:45.310' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, CAST(N'2019-09-14T12:18:02.220' AS DateTime), N'MASTER_SERVICE', 0, N'Add New Service', NULL, N'{"Code":"S003","Name":"Audiometri","Price":25872.0,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:18:02.220' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, CAST(N'2019-09-14T12:18:23.190' AS DateTime), N'MASTER_SERVICE', 0, N'Add New Service', NULL, N'{"Code":"S004","Name":"Spirometri","Price":28279.0,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:18:23.190' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, CAST(N'2019-09-14T12:19:07.050' AS DateTime), N'MASTER_GUDANG', 0, N'Add New Gudang', NULL, N'{"name":"Gudang Town Site 1","ClinicId":1,"ClinicName":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:19:07.057' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (15, CAST(N'2019-09-14T12:22:04.083' AS DateTime), N'MASTER_LAB_ITEM_CATEGORY', 0, N'Add New Lab Item Category', NULL, N'{"LabType":"Lab","PoliID":12,"PoliName":null,"Name":"Faal Ginjal","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:22:04.090' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (16, CAST(N'2019-09-14T12:22:16.453' AS DateTime), N'MASTER_LAB_ITEM_CATEGORY', 0, N'Add New Lab Item Category', NULL, N'{"LabType":"Lab","PoliID":12,"PoliName":null,"Name":"Urine Rutin","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:22:16.457' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (17, CAST(N'2019-09-14T12:22:50.513' AS DateTime), N'MASTER_LAB_ITEM_CATEGORY', 0, N'Add New Lab Item Category', NULL, N'{"LabType":"Lab","PoliID":12,"PoliName":null,"Name":"Darah Lengkap","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:22:50.513' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (18, CAST(N'2019-09-14T12:23:00.670' AS DateTime), N'MASTER_LAB_ITEM_CATEGORY', 0, N'Add New Lab Item Category', NULL, N'{"LabType":"Lab","PoliID":12,"PoliName":null,"Name":"PROFIL LIPID","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:23:00.670' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (19, CAST(N'2019-09-14T12:23:11.817' AS DateTime), N'MASTER_LAB_ITEM_CATEGORY', 0, N'Add New Lab Item Category', NULL, N'{"LabType":"Lab","PoliID":12,"PoliName":null,"Name":"GULA DARAH","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:23:11.817' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20, CAST(N'2019-09-14T12:23:34.983' AS DateTime), N'MASTER_LAB_ITEM_CATEGORY', 0, N'Add New Lab Item Category', NULL, N'{"LabType":"Lab","PoliID":12,"PoliName":null,"Name":"TES ARTHRITIS","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:23:34.983' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (21, CAST(N'2019-09-14T12:24:38.010' AS DateTime), N'MASTER_LAB_ITEM_CATEGORY', 0, N'Add New Lab Item Category', NULL, N'{"LabType":"Spirometri","PoliID":11,"PoliName":null,"Name":"SPIROMETRI","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:24:38.010' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (22, CAST(N'2019-09-14T12:25:19.660' AS DateTime), N'MASTER_LAB_ITEM_CATEGORY', 0, N'Add New Lab Item Category', NULL, N'{"LabType":"Audiometri","PoliID":11,"PoliName":null,"Name":"AUDIOMETRI","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:25:19.660' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (23, CAST(N'2019-09-14T12:27:39.440' AS DateTime), N'MASTER_PRODUCT_CATEGORY', 0, N'Add New Product Category', NULL, N'{"Name":"Obat Oral","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:27:39.443' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (24, CAST(N'2019-09-14T12:38:48.250' AS DateTime), N'MASTER_PRODUCT_CATEGORY', 0, N'Add New Product Category', NULL, N'{"Name":"Injection","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:38:48.253' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (25, CAST(N'2019-09-14T12:38:59.263' AS DateTime), N'MASTER_PRODUCT_CATEGORY', 0, N'Add New Product Category', NULL, N'{"Name":"Racikan","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:38:59.267' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (26, CAST(N'2019-09-14T12:39:06.180' AS DateTime), N'MASTER_PRODUCT_CATEGORY', 0, N'Add New Product Category', NULL, N'{"Name":"Request","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:39:06.180' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (27, CAST(N'2019-09-14T12:39:36.547' AS DateTime), N'MASTER_PRODUCT_UNIT', 0, N'Add New Product Unit', NULL, N'{"Code":null,"Name":"tub","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:39:36.553' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (28, CAST(N'2019-09-14T12:39:40.470' AS DateTime), N'MASTER_PRODUCT_UNIT', 0, N'Add New Product Unit', NULL, N'{"Code":null,"Name":"pcs","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:39:40.473' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (29, CAST(N'2019-09-14T12:39:44.557' AS DateTime), N'MASTER_PRODUCT_UNIT', 0, N'Add New Product Unit', NULL, N'{"Code":null,"Name":"bot","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:39:44.557' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30, CAST(N'2019-09-14T12:40:06.817' AS DateTime), N'MASTER_PRODUCT_UNIT', 0, N'Add New Product Unit', NULL, N'{"Code":null,"Name":"pack","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:40:06.817' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (31, CAST(N'2019-09-14T12:40:16.037' AS DateTime), N'MASTER_PRODUCT_UNIT', 0, N'Edit Product Unit', N'{"Code":null,"Name":"tub","Id":1,"RowStatus":0,"CreatedDate":"2019-09-14T12:39:36.49","ModifiedDate":null,"CreatedBy":"M008-adminsuper","ModifiedBy":null,"Account":null}', N'{"Code":null,"Name":"tube","Id":1,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:40:16.037' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (32, CAST(N'2019-09-14T12:40:35.003' AS DateTime), N'MASTER_PRODUCT_UNIT', 0, N'Add New Product Unit', NULL, N'{"Code":null,"Name":"vial","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:40:35.003' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (33, CAST(N'2019-09-14T12:40:52.880' AS DateTime), N'MASTER_PRODUCT_UNIT', 0, N'Add New Product Unit', NULL, N'{"Code":null,"Name":"tab","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:40:52.880' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (34, CAST(N'2019-09-14T13:50:47.657' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T13:50:47.660' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (35, CAST(N'2019-09-14T14:19:54.960' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T14:19:54.960' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (36, CAST(N'2019-09-14T14:33:52.347' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T14:33:52.353' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (37, CAST(N'2019-09-14T14:35:07.727' AS DateTime), N'MASTER_PRODUCT', 0, N'Add New Product', NULL, N'{"Code":"1-001","Name":"Novax tab","Vendor":"Gracia Pharmindo","ProductCategoryID":1,"ProductUnitID":6,"ProductCategoryName":null,"ProductUnitName":null,"RetailPrice":5000.0,"stock":0.0,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T14:35:07.820' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (38, CAST(N'2019-09-14T15:02:13.450' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T15:02:13.450' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (39, CAST(N'2019-09-14T16:17:33.030' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:17:33.030' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40, CAST(N'2019-09-14T16:18:46.247' AS DateTime), N'MASTER_ORGANIZATION', 0, N'Add New Organization', NULL, N'{"ID":2,"OrgCode":"K0001","OrgName":"Klinik Town Site1","KlinikID":1,"RowStatus":0,"CreatedBy":"adminsuper","CreatedDate":"2019-09-14T16:18:46.2029977+07:00","ModifiedBy":null,"ModifiedDate":null,"Clinic":null,"OrganizationPrivileges":[],"OrganizationRoles":[],"PasswordHistories":[],"Users":[]}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:18:46.307' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (41, CAST(N'2019-09-14T16:21:55.127' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"Validation failed for one or more entities. See ''EntityValidationErrors'' property for more details."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:21:55.127' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (42, CAST(N'2019-09-14T16:23:24.837' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"Validation failed for one or more entities. See ''EntityValidationErrors'' property for more details."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:23:24.837' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (43, CAST(N'2019-09-14T16:35:58.747' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"The underlying provider failed on Rollback.\r\nInnerException: Value cannot be null.\r\nParameter name: connection"', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:35:58.747' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (44, CAST(N'2019-09-14T16:37:59.163' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Employee_FamilyRelationship\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.FamilyRelationship\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:37:59.163' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (45, CAST(N'2019-09-14T16:40:53.870' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Employee_FamilyRelationship\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.FamilyRelationship\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:40:53.870' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (46, CAST(N'2019-09-14T16:41:58.783' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Doctor_Employee\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.Employee\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:41:58.783' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (47, CAST(N'2019-09-14T16:42:34.713' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Doctor_Employee\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.Employee\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:42:34.713' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (48, CAST(N'2019-09-14T16:47:22.927' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Employee_FamilyRelationship\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.FamilyRelationship\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:47:22.927' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (49, CAST(N'2019-09-14T16:49:41.577' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Employee_FamilyRelationship\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.FamilyRelationship\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:49:41.577' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50, CAST(N'2019-09-14T16:56:22.157' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Employee_EmployeeStatus\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.EmployeeStatus\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T16:56:22.157' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (51, CAST(N'2019-09-14T17:49:02.050' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T17:49:02.050' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (52, CAST(N'2019-09-14T17:52:23.500' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_DoctorClinic_Clinic\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.Clinic\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T17:52:23.500' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (53, CAST(N'2019-09-14T17:55:37.577' AS DateTime), N'MASTER_DOCTOR', 0, N'Add New Doctor', NULL, N'"An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: An error occurred while updating the entries. See the inner exception for details.\r\nInnerException: The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_DoctorClinic_Clinic\". The conflict occurred in database \"KlinikDBUAT\", table \"dbo.Clinic\", column ''ID''.\r\nThe statement has been terminated."', N'ERROR', 0, N'M008-adminsuper', CAST(N'2019-09-14T17:55:37.577' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (54, CAST(N'2019-09-14T17:56:53.833' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T17:56:53.833' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (55, CAST(N'2019-09-14T18:14:04.827' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T18:14:04.830' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (56, CAST(N'2019-09-14T18:24:08.497' AS DateTime), N'MASTER_ROLE', 0, N'Add New Role', NULL, N'{"OrgID":2,"OrganizationName":null,"RoleName":"Doctor","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T18:24:08.527' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (57, CAST(N'2019-09-14T18:24:14.987' AS DateTime), N'MASTER_ROLE', 0, N'Add New Role', NULL, N'{"OrgID":2,"OrganizationName":null,"RoleName":"Admin","Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":1,"EmployeeID":0,"Roles":[1,2],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40001,40002,40003,40004,40006,40005,40007,40008,40009,40011,40012,40013,40014,40015,40016,40017,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40036,40037,40038,40039,40040,40041,40047,40046,40044,40045,40043,40042,40049,40048,40050,40053,40051,40052,40059,40058,40057,40056,40055,40054,40060,40061,50062,50061,50060,50063,50064,50065,50066,50067,50068,50070,50069,50072,50071,40018,50080,50079,50078,50081,50077,50076,50075,50074,50073,50082,50083,50086,50085,50084,50087,50092,50091,50089,50088,50093,50097,50096,50095,50094,50101,50100,50099,50098,50105,50104,50103,50102,50110,50109,50108,50107,50114,50113,50112,50111,50118,50117,50116,50115,50106,50123,50122,50121,50120,50133,50123,50122,50121,50120,50119,50118,50117,50116,50115,50114,50113,50112,50111,50110,50109,50108,50107,50106,50105,50104,50103,50102,50101,50100,50099,50098,50097,50096,50095,50094,50093,50092,50091,50089,50088,50087,50086,50085,50084,50083,50082,50077,50076,50075,50074,50073,50063,50062,50061,50060,40061,50081,50080,50079,50078,40018,50072,50071,50065,50066,50067,50068,50069,50070,50064,40060,40059,40058,40057,40056,40055,40054,40053,40052,40051,40048,40049,40050,40047,40046,40045,40044,40043,40042,40041,40040,40039,40038,40037,40036,40035,40033,40031,40030,50137,50138],"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Polis":{"ClinicID":0,"PoliID":0,"PoliIDs":[],"ClinicName":null,"PoliName":null,"PoliCode":null,"Id":0,"RowStatus":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":"M008","UserCode":"M008-adminsuper","ClinicID":0,"GudangID":0}}', N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T18:24:14.987' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (58, CAST(N'2019-09-14T18:28:34.863' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'K0001-admin', CAST(N'2019-09-14T18:28:34.863' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (59, CAST(N'2019-09-14T18:29:06.683' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T18:29:06.683' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (60, CAST(N'2019-09-14T18:29:57.333' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'K0001-admin', CAST(N'2019-09-14T18:29:57.333' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (61, CAST(N'2019-09-14T19:28:57.897' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-14T19:28:57.897' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Log] ([ID], [Start], [Module], [Account], [Command], [OldValue], [NewValue], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (62, CAST(N'2019-09-17T05:34:51.840' AS DateTime), N'LOGIN', 0, N'Login To System', NULL, NULL, N'SUCCESS', 0, N'M008-adminsuper', CAST(N'2019-09-17T05:34:51.840' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Log] OFF
GO
SET IDENTITY_INSERT [dbo].[LookupCategory] ON 
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'0', N'Umur', N'18', 0, N'M008-adminsuper', CAST(N'2019-09-15T05:49:24.703' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'1', N'Umur', N'18-25', 0, N'M008-adminsuper', CAST(N'2019-09-15T05:51:17.253' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'2', N'Umur', N'25-34', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:09:53.547' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'3', N'Umur', N'35-44', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:11:07.893' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'4', N'Umur', N'45-44', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:11:20.243' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'M', N'Gender', N'Male', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:11:30.903' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T06:13:10.450' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'F', N'Gender', N'Female', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:11:39.670' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T06:13:16.337' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'E', N'Family Status', N'Employee', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:13:57.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, N'S', N'Family Status', N'Employee Spouse', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:14:19.890' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T08:00:35.273' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, N'C1', N'Family Status', N'Employee Children 1', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:14:33.183' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T08:00:40.007' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, N'C2', N'Family Status', N'Employee Children 2', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:14:44.240' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T08:00:46.257' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, N'C3', N'Family Status', N'Employee Children 3', 0, N'M008-adminsuper', CAST(N'2019-09-15T06:15:03.317' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T08:00:53.733' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, N'VN', N'Family Status', N'Vendor', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:26:06.097' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, N'VS', N'Family Status', N'Vendor Spouse', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:26:31.880' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (15, N'VC', N'Family Status', N'Vendor Children', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:27:09.893' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (16, N'2100', N'Business Unit', N' APRIL Riaupulp', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:30:20.517' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (17, N'0024', N'Department', N'AAA R&D', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:31:08.007' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (18, N'3100', N'Department', N'BB H&R', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:31:33.803' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (19, N'EN', N'Clinic Status', N'Employee - New', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:32:29.643' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20, N'EC', N'Clinic Status', N'Employee - Control', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:32:46.320' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (21, N'VN', N'Clinic Status', N'Vendor New', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:33:23.127' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (22, N'VC', N'Clinic Status', N'Vendor - Control', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:33:41.267' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (23, N'2', N'Payment Type', N'BPJS', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:34:09.230' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T09:46:36.983' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (24, N'3', N'Payment Type', N'Insurance', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:34:20.970' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T09:46:56.020' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (25, N'5', N'Umur', N'>55', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:35:14.173' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (26, N'Y', N'Need Rest', N'Yes', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:37:27.737' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (27, N'N', N'Need Rest', N'No', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:37:44.540' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (28, N'AC', N'Examine Type', N'Accident', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:38:17.560' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (29, N'NA', N'Examine Type', N'Not Accident', 0, N'M008-adminsuper', CAST(N'2019-09-15T07:38:32.790' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30, N'PR', N'Request Type', N'Purchase Request', 0, N'M008-adminsuper', CAST(N'2019-09-15T08:15:24.877' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (31, N'PO', N'Request Type', N'Purchase Order', 0, N'M008-adminsuper', CAST(N'2019-09-15T08:15:47.540' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (32, N'DO', N'Request Type', N'Delivery Order', 0, N'M008-adminsuper', CAST(N'2019-09-15T08:16:01.760' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (33, N'1', N'Necessity Type', N'Berobat', 0, N'M008-adminsuper', CAST(N'2019-09-15T09:43:41.740' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (34, N'6', N'Necessity Type', N'Control', 0, N'M008-adminsuper', CAST(N'2019-09-15T09:43:58.003' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T09:51:05.573' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (35, N'2', N'Necessity Type', N'Check up', 0, N'M008-adminsuper', CAST(N'2019-09-15T09:44:14.207' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T09:49:59.543' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (36, N'3', N'Necessity Type', N'Konsultasi', 0, N'M008-adminsuper', CAST(N'2019-09-15T09:44:44.263' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (37, N'4', N'Necessity Type', N'Periksa Kehamilan', 0, N'M008-adminsuper', CAST(N'2019-09-15T09:44:58.330' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (38, N'5', N'Necessity Type', N'Imunisasi', 0, N'M008-adminsuper', CAST(N'2019-09-15T09:45:09.880' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-15T09:50:50.407' AS DateTime))
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (39, N'1', N'Payment Type', N'Umum', 0, N'M008-adminsuper', CAST(N'2019-09-15T09:47:09.843' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[LookupCategory] ([ID], [LookUpCode], [LookUpName], [LookupContent], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40, N'4', N'Payment Type', N'Company', 0, N'M008-adminsuper', CAST(N'2019-09-15T09:48:22.950' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[LookupCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[Menu] ON 
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Master Data', 0, N'#', 2, 1, 1, NULL, NULL, NULL, 0, N'fa fa-database', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'Master Organization', 30051, N'~/MasterData/OrganizationList', 1, 0, 1, N'VIEW_M_ORG', N'MasterData', N'OrganizationList', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:31:00.773' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'ADD Master Organization', 2, NULL, 2, 0, 0, N'ADD_M_ORG', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'Edit Master Organization', 2, NULL, 3, 0, 0, N'EDIT_M_ORG', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'Delete Master Organization', 2, NULL, 4, 0, 0, N'DELETE_M_ORG', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'Master Privilege', 30051, NULL, 2, 0, 1, N'VIEW_M_PRIV', N'MasterData', N'PrivilegeList', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:29:04.200' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'ADD Master Privilege', 6, NULL, 1, 0, 0, N'ADD_M_PRIV', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'Edit Master Privilege', 6, NULL, 2, 0, 0, N'EDIT_M_PRIV', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, N'Delete Master Privilege', 6, NULL, 3, 0, 0, N'DELETE_M_PRIV', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, N'Master Role', 30051, NULL, 3, 0, 1, N'VIEW_M_ROLE', N'MasterData', N'RoleList', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:29:29.553' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, N'ADD Master Role', 10, NULL, 1, 0, 0, N'ADD_M_ROLE', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, N'Edit Master Role', 10, NULL, 2, 0, 0, N'EDIT_M_ROLE', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, N'Delete Master Role', 10, NULL, 3, 0, 0, N'DELETE_M_ROLE', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, N'Master User', 30051, NULL, 4, 0, 1, N'VIEW_M_USER', N'MasterData', N'UserList', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:29:51.760' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (16, N'Add Master User', 14, NULL, 1, 0, 0, N'ADD_M_USER', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (17, N'Edit Master User', 14, NULL, 2, 0, 0, N'EDIT_M_USER', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (18, N'Delete Master User', 14, NULL, 3, 0, 0, N'DELETE_M_USER', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (19, N'Master Employee', 1, NULL, 5, 0, 1, N'VIEW_M_EMPLOYEE', N'MasterData', N'EmployeeList', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20, N'Add Master Employee', 19, NULL, 1, 0, 0, N'ADD_M_EMPLOYEE', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (21, N'Edit Master Employee', 19, NULL, 2, 0, 0, N'EDIT_M_EMPLOYEE', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (22, N'Delete MasterEmployee', 19, NULL, 3, 0, 0, N'DELETE_M_EMPLOYEE', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (23, N'Master Log', 0, N'#', 3, 1, 1, N'Master Log', NULL, NULL, 0, N'fa fa-file-text-o', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:57:54.297' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (24, N'Logging', 23, NULL, 1, 0, 1, N'CommandLogging', N'Administration', N'Logging', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10011, N'Master Clinic', 1, NULL, 6, 0, 1, N'VIEW_M_CLINIC', N'MasterData', N'ClinicList', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10012, N'Add Master Clinic', 10011, NULL, 1, 0, 0, N'ADD_M_CLINIC', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10013, N'Edit Master Clinic', 10011, NULL, 2, 0, 0, N'EDIT_M_CLINIC', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10014, N'Delete Master Clinic', 10011, NULL, 3, 0, 0, N'DELETE_M_CLINIC', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20011, N'Home', 0, NULL, 0, 0, 1, N'Home', N'Home', N'Index', 0, N'fa fa-home', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20012, N'Reset Password', 23, NULL, 2, 0, 1, N'RESET_PASSWORD', N'Administration', N'ResetPassword', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20013, N'Registration', 0, NULL, 1, 0, 1, N'VIEW_REGISTRATION', N'Loket', N'Index', 0, N'fa fa-registered', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20015, N'Master Doctor', 1, NULL, 7, 0, 1, N'VIEW_M_DOCTOR', N'Doctor', N'Index', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20017, N'Add Master Doctor', 20015, NULL, 1, 0, 0, N'ADD_M_DOCTOR', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20018, N'Edit Master Doctor', 20015, NULL, 2, 0, 0, N'EDIT_M_DOCTOR', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20019, N'Delete Master Docor', 20015, NULL, 3, 0, 0, N'DELETE_M_DOCTOR', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20020, N'Poli Schedule', 20026, NULL, 1, 0, 1, N'VIEW_M_POLISCHEDULE', N'PoliSchedule', N'Index', 1, N'', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20021, N'Antrian Poli Umum', 20040, NULL, 1, 0, 1, N'VIEW_REGISTRATION_UMUM', N'Loket', N'PoliUmum', 0, N'fa fa-registered', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:46:59.543' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20022, N'Master Paramedic', 1, NULL, 8, 0, 1, N'VIEW_M_PARAMEDIC', N'Paramedic', N'Index', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20023, N'Add Master Paramedic', 20022, NULL, 1, 0, 0, N'ADD_M_PARAMEDIC', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20024, N'Edit Master Paramedic', 20022, NULL, 2, 0, 0, N'EDIT_M_PARAMEDIC', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20025, N'Delete Master Paramedic', 20022, NULL, 3, 0, 0, N'DELETE_M_PARAMEDIC', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20026, N'Master Schedule', 0, N'#', 4, 1, 1, N'Master Schedule', NULL, NULL, 0, N'fa fa-th-list', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:59:36.063' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20027, N'Poli Schedule Master', 20026, NULL, 2, 0, 1, N'VIEW_M_POLISCHEDULE_M', N'PoliSchedule', N'Master', 1, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20029, N'Master Patient', 1, NULL, 6, 0, 1, N'VIEW_M_PATIENT', N'Patient', N'PasienList', 0, NULL, 0, N'SYSTEM', CAST(N'2019-03-27T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:49:20.287' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20031, N'View Doctor Patient List', 0, NULL, 7, 0, 1, N'VIEW_POLI_PATIENT_LIST', N'Poli', N'PatientList', 0, N'fa fa-registered', 0, N'SYSTEM', CAST(N'2019-04-11T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20032, N'Master Menu', 30051, NULL, 1, 0, 1, N'VIEW_M_MENU', N'MasterData', N'MenuList', 1, NULL, 0, N'SYSTEM', CAST(N'2019-04-11T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:30:22.900' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20033, N'Master Product', 1, NULL, 1, 0, 1, N'VIEW_M_PRODUCT', N'MasterData', N'ProductList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:19:41.170' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20034, N'Master Product Category', 1, NULL, 1, 0, 1, N'VIEW_M_PRODUCT_CATEGORY', N'MasterData', N'ProductCategoryList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:26:58.090' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20035, N'Master Product Unit', 1, NULL, 1, 0, 1, N'VIEW_M_PRODUCT_UNIT', N'MasterData', N'ProductUnitList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:28:05.187' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20036, N'Master Product Medicine', 1, NULL, 1, 0, 1, N'VIEW_M_PRODUCT_MEDICINE', N'MasterData', N'ProductMedicineList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:34:40.397' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20037, N'Master Medicine', 1, NULL, 1, 0, 1, N'VIEW_M_MEDICINE', N'MasterData', N'MedicineList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:43:37.233' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20038, N'Master Lab ', 1, NULL, 1, 0, 1, N'VIEW_M_LAB_ITEM', N'MasterData', N'LabItemList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-04-18T14:00:48.543' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20039, N'Master Lab Category', 1, NULL, 1, 0, 1, N'VIEW_M_LAB_ITEM_CATEGORY', N'MasterData', N'LabItemCategoryList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-04-18T14:01:08.257' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20040, N'Queue List', 0, NULL, 7, 1, 1, N'Queue List', NULL, NULL, 0, N'fa fa-list-ol', 0, N'SYSTEM', CAST(N'2019-04-20T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T02:44:45.787' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30041, N'PreExamine', 0, NULL, 10, 0, 1, N'VIEW_PREEXAMINE', N'PreExamine', N'ListQueue', 0, N'fa fa-stethoscope', 0, N'SYSTEM', CAST(N'2019-04-27T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30042, N'Antrian Lab', 20040, NULL, 1, 0, 1, N'VIEW_QUEUE_LAB', N'Lab', N'ListQueueLaboratorium', 1, NULL, 0, N'SYSTEM', CAST(N'2019-05-01T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30043, N'Antrian Radiologi', 20040, NULL, 2, 0, 1, N'VIEW_QUEUE_RADIOLOGI', N'Radiologi', N'ListQueueRadiologi', 2, NULL, 0, N'SYSTEM', CAST(N'2019-05-01T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30044, N'Master Service', 1, NULL, 1, 0, 1, N'VIEW_M_SERVICE', N'MasterData', N'ServiceList', 0, NULL, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:13:08.320' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30045, N'Master Poli Service', 1, NULL, 1, 0, 1, N'VIEW_M_POLI_SERVICE', N'MasterData', N'PoliServiceList', 0, NULL, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:13:26.140' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30046, N'Master Poli', 1, NULL, 19, 0, 1, N'VIEW_M_POLI', N'MasterData', N'PoliList', 1, NULL, 0, N'M2020-mande', CAST(N'2019-05-06T08:49:26.937' AS DateTime), N'M008-adminsuper', CAST(N'2019-08-17T16:02:26.377' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30047, N'ADD Master Poli', 30046, NULL, 1, 0, 0, N'ADD_M_POLI', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30048, N'Edit Master Poli', 30046, NULL, 2, 0, 0, N'EDIT_M_POLI', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30049, N'Delete Master Poli', 30046, NULL, 3, 0, 0, N'DELETE_M_POLI', NULL, NULL, NULL, NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30050, N'Cashier Menu', 0, NULL, 11, 0, 1, N'VIEW_M_CASHIER', N'Cashier', N'ListPatien', 0, N'fa fa-user-md', 0, N'M2020-mande', CAST(N'2019-05-06T09:23:41.407' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30051, N'Master ACL', 0, N'#', 2, 1, 1, N'Master Access Control List', NULL, NULL, 0, N'fa fa-users', 0, N'M008-adminsuper', CAST(N'2019-05-08T02:18:13.193' AS DateTime), N'M008-adminsuper', CAST(N'2019-05-08T03:02:14.443' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30052, N'Surat', 0, N'#', 15, 0, 1, N'VIEW_SURAT', N'Letters', N'SuratRujukan', 0, N'fa fa-envelope', 0, N'SYSTEM', CAST(N'2019-07-18T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30053, N'Master Gudang', 1, N'#', 15, 0, 1, N'VIEW_M_GUDANG', N'MasterData', N'GudangList', 0, NULL, 0, N'M008-adminsuper', CAST(N'2019-08-12T20:19:06.130' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30056, N'Stock Module', 0, N'#', 25, 1, 1, N'VIEW_M_REEDOO', NULL, NULL, 0, N'fa fa-database', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), N'M008-adminsuper', CAST(N'2019-08-17T17:11:07.850' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30059, N'Purchase Order', 30056, N'#', 3, 0, 1, N'VIEW_M_PURCHASEORDER', N'PurchaseOrder', N'PurchaseOrderList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-08-16T22:00:24.583' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30060, N'Master Vendor', 1, N'#', 16, 0, 1, N'VIEW_M_VENDOR', N'MasterData', N'VendorList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-08-17T15:56:41.783' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30061, N'Delivery Order', 30056, N'#', 1, 0, 1, N'VIEW_M_DELIVERYORDER', N'DeliveryOrder', N'DeliveryOrderList', 1, NULL, 0, N'M008-adminsuper', CAST(N'2019-08-17T17:07:54.860' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30062, N'Pharmacy', 0, NULL, 0, 0, 1, N'VIEW_FARMASI', N'Pharmacy', N'PatientList', 0, N'fa fa-stethoscope', 0, N'M008-adminsuper', CAST(N'2019-08-17T17:51:47.340' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30064, N'Purchase Request', 30056, N'#', 5, 0, 1, N'VIEW_M_PURCHASEREQUEST', N'PurchaseRequest', N'PurchaseRequestList', 0, NULL, 0, N'M008-adminsuper', CAST(N'2019-08-18T13:11:16.897' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30066, N'Master Product In Gudang', 1, N'#', 100, 0, 1, N'VIEW_M_PRODUCTINGUDANG', N'MasterData', N'ProductInGudangList', 0, NULL, 0, N'M008-adminsuper', CAST(N'2019-08-23T10:30:28.157' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30067, N'Master Tes', 1, N'http//;google.com', 0, 1, 0, N'Master Tes', N'Aji', N'Sembarang', 4, N'fa-fa-bars', 0, N'M008-adminsuper', CAST(N'2019-08-25T13:11:16.280' AS DateTime), N'M008-adminsuper', CAST(N'2019-08-25T13:18:06.970' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30068, N'Master Tes', 14, N'http//;google.com', 0, 1, 0, N'Master Tes', N'Aji', N'Sembarang', 4, N'fa-fa-bars', 0, N'M008-adminsuper', CAST(N'2019-08-25T13:11:35.733' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30069, N'PR Config', 30056, NULL, 9, 0, 1, N'VIEW_M_PURCHASEREQUESTCONFIG', N'PurchaseRequestConfig', N'PurchaseRequestConfigList', 1, NULL, 0, N'0721-mustarifin', CAST(N'2019-09-02T20:03:02.570' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30070, N'Pengambilan Obat', 0, N'#', 50, 0, 1, N'VIEW_PENGAMBILAN_OBAT', N'Pharmacy', N'PengambilanObat', 0, N'fa fa-medkit', 0, N'SYSTEM', CAST(N'2019-09-03T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30071, N'Reports', 0, NULL, 2, 1, 1, N'Reports', N'Reports', N'Index', 0, N'fa fa-database', 0, N'M008-adminsuper', CAST(N'2019-09-06T05:35:38.933' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-06T06:38:22.440' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30072, N'Gudang Pusat', 0, NULL, 0, 1, 1, N'VIEW_GUDANG_PUSAT', NULL, NULL, 0, NULL, 0, N'0721-mustarifin', CAST(N'2019-09-06T10:04:29.620' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30073, N'Purchase Request Pusat', 30072, N'#', 1, 0, 1, N'VIEW_M_PURCHASEREQUESTPUSAT', N'GudangPusat', N'PurchaseRequestList', 1, NULL, 0, N'0721-mustarifin', CAST(N'2019-09-06T10:06:49.897' AS DateTime), N'0721-mustarifin', CAST(N'2019-09-06T10:10:16.740' AS DateTime))
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30076, N'Top 10 Diseases', 30071, NULL, 0, 0, 1, N'Top 10 Diseases', N'Reports', N'Top10Dieases', 0, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-09T06:02:22.613' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30077, N'Top 10 Referals', 30071, NULL, 0, 0, 1, N'Top 10 Referals', N'Reports', N'Top10Referals', 0, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-09T06:19:00.937' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Menu] ([ID], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [Icon], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30078, N'Medical History', 0, N'#', 27, 0, 1, N'VIEW_MEDICAL_HISTORY', N'MedicalHistory', N'ViewEmployeeData', 0, N'fa fa-list-ul', 0, N'SYSTEM', CAST(N'2019-09-06T00:00:00.000' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Menu] OFF
GO
SET IDENTITY_INSERT [dbo].[Organization] ON 
GO
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'M008', N'PT RAPP', 0, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, CAST(N'2019-09-02T05:35:14.047' AS DateTime))
GO
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'K0001', N'Klinik Town Site1', 1, 0, N'adminsuper', CAST(N'2019-09-14T16:18:46.203' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Organization] OFF
GO
SET IDENTITY_INSERT [dbo].[OrganizationPrivilege] ON 
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, 10002, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, 10003, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 1, 10006, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, 1, 10007, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, 1, 10004, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, 1, 10008, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, 1, 10013, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, 1, 20014, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, 1, 30013, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, 1, 30014, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, 1, 10016, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, 1, 10017, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, 1, 10015, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, 1, 10018, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (15, 1, 20013, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (16, 1, 20015, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (17, 1, 20016, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (18, 1, 20017, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (19, 1, 10010, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20, 1, 10011, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (21, 1, 10005, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (22, 1, 10009, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (23, 1, 10012, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (24, 1, 10014, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (25, 1, 40001, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (26, 1, 40002, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (27, 1, 40003, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (28, 1, 40004, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (29, 1, 40005, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30, 1, 40006, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (31, 1, 40007, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (32, 1, 40008, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (33, 1, 40009, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (34, 1, 40011, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (35, 1, 40012, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (36, 1, 40013, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (37, 1, 40014, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (38, 1, 40015, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (39, 1, 40016, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40, 1, 40017, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (41, 1, 40019, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (42, 1, 40020, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (43, 1, 40021, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (44, 1, 40022, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (45, 1, 40023, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (46, 1, 40024, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (47, 1, 40025, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (48, 1, 40026, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (49, 1, 40027, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50, 1, 40028, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (51, 1, 40029, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (52, 1, 40030, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (53, 1, 40031, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (54, 1, 40033, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (55, 1, 40035, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (56, 1, 40036, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (57, 1, 40037, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (58, 1, 40038, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (59, 1, 40039, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (60, 1, 40040, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (61, 1, 40041, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (62, 1, 40042, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (63, 1, 40043, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (64, 1, 40044, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (65, 1, 40045, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (66, 1, 40046, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (67, 1, 40047, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (68, 1, 40050, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (69, 1, 40049, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (70, 1, 40048, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (71, 1, 40051, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (72, 1, 40052, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (73, 1, 40053, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (74, 1, 40054, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (75, 1, 40055, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (76, 1, 40056, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (77, 1, 40057, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (78, 1, 40058, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (79, 1, 40059, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (80, 1, 40060, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (81, 1, 40061, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (82, 1, 50060, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (83, 1, 50061, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (84, 1, 50062, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (85, 1, 50063, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (86, 1, 50064, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (87, 1, 50070, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (88, 1, 50069, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (89, 1, 50068, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (90, 1, 50067, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (91, 1, 50066, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (92, 1, 50065, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (93, 1, 50071, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (94, 1, 50072, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (95, 1, 40018, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (96, 1, 50078, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (97, 1, 50079, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (98, 1, 50080, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (99, 1, 50081, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (100, 1, 40061, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (101, 1, 50060, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (102, 1, 50061, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (103, 1, 50062, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (104, 1, 50063, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (105, 1, 50073, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (106, 1, 50074, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (107, 1, 50075, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (108, 1, 50076, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (109, 1, 50077, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (110, 1, 50082, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (111, 1, 50083, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (112, 1, 50084, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (113, 1, 50085, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (114, 1, 50086, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (115, 1, 50087, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (116, 1, 50088, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (117, 1, 50089, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (118, 1, 50091, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (119, 1, 50092, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (120, 1, 50093, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (121, 1, 50094, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (122, 1, 50095, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (123, 1, 50096, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (124, 1, 50097, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (125, 1, 50098, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (126, 1, 50099, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (127, 1, 50100, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (128, 1, 50101, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (129, 1, 50102, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (130, 1, 50103, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (131, 1, 50104, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (132, 1, 50105, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (133, 1, 50106, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (134, 1, 50107, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (135, 1, 50108, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (136, 1, 50109, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (137, 1, 50110, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (138, 1, 50111, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (139, 1, 50112, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (140, 1, 50113, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (141, 1, 50114, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (142, 1, 50115, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (143, 1, 50116, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (144, 1, 50117, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (145, 1, 50118, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (146, 1, 50119, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (147, 1, 10002, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (148, 1, 10003, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (149, 1, 10006, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (150, 1, 10007, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (151, 1, 10004, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (152, 1, 10008, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (153, 1, 10013, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (154, 1, 20014, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (155, 1, 30013, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (156, 1, 30014, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (157, 1, 10016, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (158, 1, 10017, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (159, 1, 10015, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (160, 1, 10018, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (161, 1, 20013, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (162, 1, 20015, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (163, 1, 20016, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (164, 1, 20017, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (165, 1, 10010, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (166, 1, 10011, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (167, 1, 10005, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (168, 1, 10009, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (169, 1, 10012, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (170, 1, 10014, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (171, 1, 40001, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (172, 1, 40002, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (173, 1, 40003, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (174, 1, 40004, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (175, 1, 40005, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (176, 1, 40006, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (177, 1, 40007, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (178, 1, 40008, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (179, 1, 40009, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (180, 1, 40011, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (181, 1, 40012, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (182, 1, 40013, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (183, 1, 40014, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (184, 1, 40015, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (185, 1, 40016, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (186, 1, 40017, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (187, 1, 40019, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (188, 1, 40020, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (189, 1, 40021, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (190, 1, 40022, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (191, 1, 40023, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (192, 1, 40024, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (193, 1, 40025, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (194, 1, 40026, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (195, 1, 40027, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (196, 1, 40028, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (197, 1, 40029, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (198, 1, 40030, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (199, 1, 40031, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (200, 1, 40033, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (201, 1, 40035, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (202, 1, 40036, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (203, 1, 40037, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (204, 1, 40038, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (205, 1, 40039, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (206, 1, 40040, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (207, 1, 40041, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (208, 1, 40042, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (209, 1, 40043, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (210, 1, 40044, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (211, 1, 40045, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (212, 1, 40046, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (213, 1, 40047, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (214, 1, 40050, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (215, 1, 40049, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (216, 1, 40048, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (217, 1, 40051, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (218, 1, 40052, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (219, 1, 40053, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (220, 1, 40054, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (221, 1, 40055, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (222, 1, 40056, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (223, 1, 40057, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (224, 1, 40058, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (225, 1, 40059, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (226, 1, 40060, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (227, 1, 40061, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (228, 1, 50060, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (229, 1, 50061, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (230, 1, 50062, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (231, 1, 50063, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (232, 1, 50064, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (233, 1, 50070, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (234, 1, 50069, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (235, 1, 50068, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (236, 1, 50067, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (237, 1, 50066, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (238, 1, 50065, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (239, 1, 50071, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (240, 1, 50072, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (241, 1, 40018, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (242, 1, 50078, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (243, 1, 50079, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (244, 1, 50080, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (245, 1, 50081, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (246, 1, 40061, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (247, 1, 50060, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (248, 1, 50061, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (249, 1, 50062, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (250, 1, 50063, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (251, 1, 50073, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (252, 1, 50074, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (253, 1, 50075, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (254, 1, 50076, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (255, 1, 50077, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (256, 1, 50082, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (257, 1, 50083, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (258, 1, 50084, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (259, 1, 50085, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (260, 1, 50086, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (261, 1, 50087, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (262, 1, 50088, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (263, 1, 50089, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (264, 1, 50091, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (265, 1, 50092, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (266, 1, 50093, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (267, 1, 50094, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (268, 1, 50095, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (269, 1, 50096, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (270, 1, 50097, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (271, 1, 50098, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (272, 1, 50099, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (273, 1, 50100, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (274, 1, 50101, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (275, 1, 50102, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (276, 1, 50103, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (277, 1, 50104, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (278, 1, 50105, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (279, 1, 50106, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (280, 1, 50107, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (281, 1, 50108, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (282, 1, 50109, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (283, 1, 50110, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (284, 1, 50111, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (285, 1, 50112, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (286, 1, 50113, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (287, 1, 50114, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (288, 1, 50115, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (289, 1, 50116, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (290, 1, 50117, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (291, 1, 50118, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (292, 1, 50119, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (293, 1, 50120, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (294, 1, 50121, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (295, 1, 50122, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (296, 1, 50123, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (297, 1, 50133, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (298, 1, 50137, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (299, 1, 50138, 0, N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:43:26.710' AS DateTime))
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (300, 2, 10013, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.103' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (301, 2, 40001, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.107' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (302, 2, 40002, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.107' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (303, 2, 40003, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.107' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (304, 2, 40014, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.107' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (305, 2, 40015, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.107' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (306, 2, 40016, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.110' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (307, 2, 40017, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.110' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (308, 2, 40018, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.110' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (309, 2, 40023, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.110' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (310, 2, 40024, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.110' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (311, 2, 40025, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.110' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (312, 2, 40026, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.113' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (313, 2, 40027, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.113' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (314, 2, 40028, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.113' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (315, 2, 40029, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.113' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (316, 2, 40030, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.113' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (317, 2, 40031, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.117' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (318, 2, 40033, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.117' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (319, 2, 40035, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.117' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (320, 2, 40039, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.117' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (321, 2, 40040, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.117' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (322, 2, 40041, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.120' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (323, 2, 40048, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.120' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (324, 2, 40051, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.120' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (325, 2, 40049, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.120' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (326, 2, 40050, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.123' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (327, 2, 40054, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.123' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (328, 2, 40057, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.123' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (329, 2, 40060, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.127' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (330, 2, 40055, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.127' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (331, 2, 40056, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.127' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (332, 2, 40061, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.130' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (333, 2, 50060, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.130' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (334, 2, 50061, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.130' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (335, 2, 50062, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.133' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (336, 2, 50063, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.137' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (337, 2, 50064, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.137' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (338, 2, 50065, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.197' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (339, 2, 50068, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.197' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (340, 2, 50073, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.200' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (341, 2, 50077, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.200' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (342, 2, 50078, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.203' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (343, 2, 50080, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.203' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (344, 2, 50082, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.207' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (345, 2, 50083, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.207' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (346, 2, 50084, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.207' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (347, 2, 50091, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (348, 2, 50092, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (349, 2, 50093, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (350, 2, 50098, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.213' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (351, 2, 50099, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.213' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (352, 2, 50100, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.213' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (353, 2, 50101, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.217' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (354, 2, 50102, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.217' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (355, 2, 50106, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.217' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (356, 2, 50111, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.220' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (357, 2, 50112, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.220' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (358, 2, 50113, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.223' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (359, 2, 50114, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.227' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (360, 2, 50119, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.227' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (361, 2, 50120, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.230' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (362, 2, 50124, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.230' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (363, 2, 50125, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.233' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (364, 2, 50126, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.237' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (365, 2, 50127, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.237' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (366, 2, 50132, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:23:37.240' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[OrganizationPrivilege] OFF
GO
SET IDENTITY_INSERT [dbo].[OrganizationRole] ON 
GO
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, N'Super Admin', 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, N'Administrator', 0, N'M008-adminsuper', CAST(N'2019-09-06T05:44:07.013' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 1, N'IT Administrator', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:12:56.410' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, 1, N'Human Resources', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:14:12.350' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, 1, N'Warehouse', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:14:27.283' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, 1, N'Employee', 0, N'M008-adminsuper', CAST(N'2019-09-14T10:14:35.477' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, 2, N'Doctor', 0, N'M008-adminsuper', CAST(N'2019-09-14T18:24:08.480' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, 2, N'Admin', 0, N'M008-adminsuper', CAST(N'2019-09-14T18:24:14.980' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[OrganizationRole] OFF
GO
SET IDENTITY_INSERT [dbo].[Poli] ON 
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'A ', N'Loket', 0, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'B ', N'Poli Umum', 1, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'C ', N'Poli Gigi', 2, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'D ', N'Poli Penyakit Dalam', 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'E ', N'Poli Kulit', 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'F ', N'Poli Paru', 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'G ', N'Poli THT', 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'H ', N'Poli Anak', 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, N'I ', N'Poli Syaraf', 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, N'J ', N'Poli Obgyn', 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, N'K ', N'Radiologi', 4, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, N'L ', N'Laboratorium', 4, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, N'M ', N'Farmasi', 4, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, N'O ', N'Administrasi', 4, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Poli] ([ID], [Code], [Name], [Type], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (15, N'N ', N'Kasir', 5, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Poli] OFF
GO
SET IDENTITY_INSERT [dbo].[PoliClinic] ON 
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, 15, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.093' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, 14, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.097' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (3, 1, 13, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.097' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (4, 1, 12, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.097' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (5, 1, 11, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.097' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (6, 1, 10, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.100' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (7, 1, 9, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.100' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (8, 1, 8, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.100' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (9, 1, 7, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.100' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (10, 1, 6, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.100' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (11, 1, 5, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.100' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (12, 1, 4, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.103' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (13, 1, 3, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.103' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (14, 1, 2, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.103' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliClinic] ([ID], [ClinicID], [PoliID], [RowStatus], [CreatedBy], [Createddate], [ModifiedBy], [ModifiedDate]) VALUES (15, 1, 1, 0, N'M008-adminsuper', CAST(N'2019-09-14T11:48:10.107' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[PoliClinic] OFF
GO
SET IDENTITY_INSERT [dbo].[PoliFlowTemplate] ON 
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Default', 0, 1, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'Default', 0, 2, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'Default', 0, 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'Default', 0, 4, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, N'Default', 1, 4, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'Default', 1, 5, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'Default', 2, 4, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'Default', 2, 5, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, N'Default', 3, 4, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, N'Default', 3, 5, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, N'Default', 4, 1, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, N'Default', 4, 2, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, N'Default', 4, 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, N'Default', 4, 5, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (15, N'Default', 1, 2, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (16, N'Default', 1, 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (17, N'Default', 2, 1, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (18, N'Default', 2, 3, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (19, N'Default', 3, 1, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[PoliFlowTemplate] ([ID], [Code], [PoliTypeID], [PoliTypeIDTo], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20, N'Default', 3, 2, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[PoliFlowTemplate] OFF
GO
SET IDENTITY_INSERT [dbo].[Privilege] ON 
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10002, N'VIEW_M_ORG', N'View Master Organization', 2, 0, N'SYSTEM', CAST(N'2019-02-19T12:45:24.820' AS DateTime), NULL, CAST(N'2019-02-28T08:57:18.790' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10003, N'ADD_M_ORG', N'ADD Master Organization', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:45:41.287' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10004, N'EDIT_M_ORG', N'Edit Master Organization', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:48:45.103' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10005, N'DELETE_M_ORG', N'Delete Master Organization', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:49:15.407' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10006, N'VIEW_M_PRIVILEGE', N'View Master Privilege', 6, 0, N'SYSTEM', CAST(N'2019-02-19T12:50:01.217' AS DateTime), NULL, CAST(N'2019-02-28T09:00:43.857' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10007, N'ADD_M_PRIVILEGE', N'Add Master Privilege', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:50:29.153' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10008, N'EDIT_M_PRIVILEGE', N'Edit Master Privilege', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:52:03.537' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10009, N'DELETE_M_PRIVILEGE', N'Delete master Privilege', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:52:30.007' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10010, N'VIEW_M_ROLE', N'View Master Role', 10, 0, N'SYSTEM', CAST(N'2019-02-19T12:52:52.357' AS DateTime), NULL, CAST(N'2019-02-28T09:00:56.547' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10011, N'ADD_M_ROLE', N'ADD Master Role', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:53:36.073' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10012, N'EDIT_M_ROLE', N'Edit Master Role', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:53:51.943' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10013, N'Login_Access', N'Accces Login', 20011, 0, N'SYSTEM', CAST(N'2019-02-23T20:32:19.467' AS DateTime), NULL, CAST(N'2019-02-28T12:21:03.037' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10014, N'DELETE_M_ROLE', N'Delete Master Role', NULL, 0, N'SYSTEM', CAST(N'2019-02-23T20:32:59.560' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10015, N'VIEW_M_EMPLOYEE', N'View Master Employee', 19, 0, N'SYSTEM', CAST(N'2019-02-23T20:33:25.070' AS DateTime), NULL, CAST(N'2019-02-28T09:01:26.557' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10016, N'ADD_M_EMPLOYEE', N'Add data Master Employee', NULL, 0, N'SYSTEM', CAST(N'2019-02-23T20:33:46.170' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10017, N'EDIT_M_EMPLOYEE', N'Edit Master Employee', NULL, 0, N'SYSTEM', CAST(N'2019-02-23T20:34:06.163' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10018, N'DELETE_M_EMPLOYEE', N'Delete Master Employee', NULL, 0, N'SYSTEM', CAST(N'2019-02-23T20:34:22.120' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20013, N'VIEW_M_USER', N'View Master User', 14, 0, N'SYSTEM', CAST(N'2019-02-28T09:02:26.540' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20014, N'VIEW_M_DATA', N'View Master Data', 1, 0, N'SYSTEM', CAST(N'2019-02-28T09:02:56.823' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20015, N'ADD_M_USER', N'Add Master User', NULL, 0, N'SYSTEM', CAST(N'2019-02-28T09:03:51.507' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20016, N'EDIT_M_USER', N'Edit Master User', NULL, 0, N'SYSTEM', CAST(N'2019-02-28T09:04:02.903' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20017, N'DELETE_M_USER', N'Delete Master User', NULL, 0, N'SYSTEM', CAST(N'2019-02-28T09:04:16.667' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30013, N'VIEW_ADM', N'Administration Menu', 23, 0, N'SYSTEM', CAST(N'2019-03-02T06:56:56.873' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30014, N'VIEW_LOG', N'View Logging Page', 24, 0, N'SYSTEM', CAST(N'2019-03-02T06:57:57.573' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40001, N'VIEW_REGISTRATION', N'View Patient Registration', 20013, 0, N'SYSTEM', CAST(N'2019-02-19T12:45:24.820' AS DateTime), NULL, CAST(N'2019-02-28T08:57:18.790' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40002, N'ADD_REGISTRATION', N'ADD Patient Registration', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:45:41.287' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40003, N'EDIT_REGISTRATION', N'Edit Patient Registration', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:48:45.103' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40004, N'DELETE_REGISTRATION', N'Edit Patient Registration', NULL, 0, N'SYSTEM', CAST(N'2019-02-19T12:48:45.103' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40005, N'VIEW_M_DOCTOR', N'View Master Doctor', 20015, 0, N'SYSTEM', CAST(N'2019-03-18T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40006, N'ADD_M_DOCTOR', N'Add Master Doctor', NULL, 0, N'SYSTEM', CAST(N'2019-03-18T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40007, N'EDIT_M_DOCTOR', N'Edit Master Doctor', NULL, 0, N'SYSTEM', CAST(N'2019-03-18T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40008, N'DELETE_M_DOCTOR', N'Delete Master Doctor', NULL, 0, N'SYSTEM', CAST(N'2019-03-18T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40009, N'VIEW_M_CLINIC', N'View Master Clinic', 10011, 0, N'SYSTEM', CAST(N'2019-03-19T17:27:01.683' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40011, N'ADD_M_CLINIC', N'Add Master Clinic', NULL, 0, N'SYSTEM', CAST(N'2019-03-19T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40012, N'EDIT_M_CLINIC', N'Edit Master Clinic', NULL, 0, N'SYSTEM', CAST(N'2019-03-19T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40013, N'DELETE_M_CLINIC', N'Delete Master Clinic', NULL, 0, N'SYSTEM', CAST(N'2019-03-19T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40014, N'VIEW_M_POLISCHEDULE', N'View Poli Schedule', 20026, 0, N'SYSTEM', CAST(N'2019-03-19T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40015, N'ADD_M_POLISCHEDULE', N'Add Poli Schedule', NULL, 0, N'SYSTEM', CAST(N'2019-03-19T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40016, N'EDIT_M_POLISCHEDULE', N'Edit Poli Schedule', NULL, 0, N'SYSTEM', CAST(N'2019-03-19T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40017, N'DELETE_M_POLISCHEDULE', N'Delete Poli Schedule', NULL, 0, N'SYSTEM', CAST(N'2019-03-19T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40018, N'VIEW_REGISTRATION_UMUM', N'View Patient Registration', 20021, 0, N'SYSTEM', CAST(N'2019-02-19T12:45:24.820' AS DateTime), NULL, CAST(N'2019-02-28T08:57:18.790' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40019, N'VIEW_M_PARAMEDIC', N'View Master Paramedic', 20022, 0, N'SYSTEM', CAST(N'2019-03-20T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40020, N'ADD_M_PARAMEDIC', N'Add Master Paramedic', NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40021, N'EDIT_M_PARAMEDIC', N'Edit Master Paramedic', NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40022, N'DELETE_M_PARAMEDIC', N'Delete Master Paramedic', NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40023, N'VIEW_M_POLISCHEDULE_M', N'View Poli Schedule Master', 20027, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40024, N'VIEW_SCHEDULE', N'View Schedule', 20020, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40025, N'ADD_M_POLISCHEDULE_M', N'Add Poli Schedule Master', NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40026, N'EDIT_M_POLISCHEDULE_M', N'Edit Poli Schedule Master', NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40027, N'DELETE_M_POLISCHEDULE_M', N'Delete Poli Schedule Master', NULL, 0, N'SYSTEM', CAST(N'2019-03-21T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40028, N'VIEW_M_PATIENT', N'View Pasien', 20029, 0, N'M008-adminsuper', CAST(N'2019-03-27T09:42:57.657' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40029, N'ADD_M_PATIENT', N'Add New Pasien', 20029, 0, N'M008-adminsuper', CAST(N'2019-03-27T09:43:12.217' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40030, N'EDIT_M_PATIENT', N'EDIT Pasien', 20029, 0, N'M008-adminsuper', CAST(N'2019-03-27T09:43:24.640' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40031, N'DELETE_M_PATIENT', N'DeletePasien', 20029, 0, N'M008-adminsuper', CAST(N'2019-03-27T09:43:41.193' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40033, N'VIEW_POLI_PATIENT_LIST', N'View doctor''s patient list in each poli', 20031, 0, N'M008-adminsuper', CAST(N'2019-04-11T09:36:42.600' AS DateTime), NULL, CAST(N'2019-04-11T10:02:16.993' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40035, N'ADD_POLI_FORM_EXAMINE', N'Add new form examine in POLI', 20031, 0, N'M008-adminsuper', CAST(N'2019-04-14T19:26:36.573' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40036, N'VIEW_M_MENU', N'Menu List', 20032, 0, N'M008-adminsuper', CAST(N'2019-04-18T05:48:54.207' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40037, N'ADD_M_MENU', N'Add new menu', 20032, 0, N'M008-adminsuper', CAST(N'2019-04-18T05:59:54.277' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40038, N'EDIT_M_MENU', N'Edit  menu', 20032, 0, N'M008-adminsuper', CAST(N'2019-04-18T06:00:11.250' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40039, N'VIEW_M_PRODUCT', N'Master product page', 20033, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:20:48.543' AS DateTime), NULL, CAST(N'2019-04-18T10:25:01.843' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40040, N'VIEW_M_PRODUCT_UNIT', N'Master of product unit', 20035, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:28:23.113' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40041, N'VIEW_M_PRODUCT_CATEGORY', N'Master of product category', 20034, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:29:53.383' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40042, N'ADD_M_PRODUCT', N'Add new product', 20033, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:31:34.797' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40043, N'EDIT_M_PRODUCT', N'Edit product', 20033, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:31:43.433' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40044, N'ADD_M_PRODUCT_CATEGORY', N'Add new product category', 20034, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:32:01.477' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40045, N'EDIT_M_PRODUCT_CATEGORY', N'Edit product category', 20034, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:32:10.277' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40046, N'ADD_M_PRODUCT_UNIT', N'Add new product unit', 20035, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:32:26.930' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40047, N'EDIT_M_PRODUCT_UNIT', N'Edit product unit', 20035, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:32:37.273' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40048, N'VIEW_M_PRODUCT_MEDICINE', N'Master of product medicine', 20036, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:35:05.993' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40049, N'ADD_M_PRODUCT_MEDICINE', N'Add product medicine', 20036, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:35:18.227' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40050, N'EDIT_M_PRODUCT_MEDICINE', N'Edit product medicine', 20036, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:35:25.173' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40051, N'VIEW_M_MEDICINE', N'Master of medicine', 20037, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:43:56.093' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40052, N'ADD_M_MEDICINE', N'Add medicine', 20037, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:44:05.377' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40053, N'EDIT_M_MEDICINE', N'Edit medicine', 20037, 0, N'M008-adminsuper', CAST(N'2019-04-18T10:44:12.117' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40054, N'VIEW_M_LAB_ITEM', N'Master of Lab item', 20038, 0, N'M008-adminsuper', CAST(N'2019-04-18T14:01:44.507' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40055, N'ADD_M_LAB_ITEM', N'Add Lab item', 20038, 0, N'M008-adminsuper', CAST(N'2019-04-18T14:01:53.833' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40056, N'EDIT_M_LAB_ITEM', N'Edit Lab item', 20038, 0, N'M008-adminsuper', CAST(N'2019-04-18T14:02:01.563' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40057, N'VIEW_M_LAB_ITEM_CATEGORY', N'Master of lab category', 20039, 0, N'M008-adminsuper', CAST(N'2019-04-18T14:02:17.593' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40058, N'ADD_M_LAB_ITEM_CATEGORY', N'Add lab category', 20039, 0, N'M008-adminsuper', CAST(N'2019-04-18T14:02:29.637' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40059, N'EDIT_M_LAB_ITEM_CATEGORY', N'Edit lab category', 20039, 0, N'M008-adminsuper', CAST(N'2019-04-18T14:02:37.360' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40060, N'VIEW_QUEUE_LAB', N'View Antrian Laboratorium', 30042, 0, N'M008-adminsuper', CAST(N'2019-04-20T20:52:19.490' AS DateTime), NULL, CAST(N'2019-05-01T06:46:47.317' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40061, N'ADD_Lab_Item', N'Add Lab Item', 20040, 0, N'M008-adminsuper', CAST(N'2019-04-21T22:39:59.463' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50060, N'VIEW_PREEXAMINE', N'VIEW PREEXAMINE', 30041, 0, N'M008-adminsuper', CAST(N'2019-04-27T13:59:43.313' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50061, N'ADD_PREEXAMINE', N'ADD PREEXAMINE', 30041, 0, N'M008-adminsuper', CAST(N'2019-04-27T13:59:54.623' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50062, N'EDIT_PREEXAMINE', N'EDIT PREEXAMINE', 30041, 0, N'M008-adminsuper', CAST(N'2019-04-27T14:00:13.757' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50063, N'VIEW_INPUT_LAB', N'VIEW Menu Input Lab Result', 30043, 0, N'M008-adminsuper', CAST(N'2019-05-01T06:47:40.970' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50064, N'ADD_LAB_RESULT', N'Add Lab Result', 20040, 0, N'M008-adminsuper', CAST(N'2019-05-02T20:42:11.047' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50065, N'VIEW_M_SERVICE', N'View Service', 30044, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:14:12.583' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50066, N'EDIT_M_SERVICE', N'Edit Service', 30044, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:14:20.583' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50067, N'ADD_M_SERVICE', N'Add Service', 30044, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:14:27.177' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50068, N'VIEW_M_POLI_SERVICE', N'View Poli Service', 30045, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:14:42.760' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50069, N'ADD_M_POLI_SERVICE', N'Add Poli Service', 30045, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:14:48.967' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50070, N'EDIT_M_POLI_SERVICE', N'Edit Poli Service', 30045, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:14:56.237' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50071, N'DELETE_M_POLI_SERVICE', N'Delete Poli Service', 30045, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:19:27.250' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50072, N'DELETE_M_SERVICE', N'Delete Service', 30044, 0, N'M008-adminsuper', CAST(N'2019-05-03T17:19:36.160' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50073, N'VIEW_M_POLI', N'View Master Poli', 30046, 0, N'M2020-mande', CAST(N'2019-05-06T08:50:10.550' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50074, N'ADD_M_POLI', N'Add Master Poli', 30046, 0, N'M2020-mande', CAST(N'2019-05-06T09:08:02.003' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50075, N'EDIT_M_POLI', N'Edit Master Poli', 30046, 0, N'M2020-mande', CAST(N'2019-05-06T09:08:15.723' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50076, N'DELETE_M_POLI', N'Delete Master Poli', 30046, 0, N'M2020-mande', CAST(N'2019-05-06T09:08:27.107' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50077, N'VIEW_M_CASHIER', N'View Master Cashier', 30050, 0, N'M2020-mande', CAST(N'2019-05-06T09:24:39.263' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50078, N'VIEW_QUEUE_RADIOLOGI', N'View Queue Radiologi', 30043, 0, N'M008-adminsuper', CAST(N'2019-05-07T22:51:31.290' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50079, N'ADD_RADIOLOGI_ITEM', N'ADD RADIOLOGI ITEM', 30043, 0, N'M008-adminsuper', CAST(N'2019-05-07T22:51:58.653' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50080, N'ADD_RADIOLOGI_RESULT', N'ADD RADIOLOGI RESULT', 30043, 0, N'M008-adminsuper', CAST(N'2019-05-07T22:52:21.757' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50081, N'VIEW_M_ACL', N'View Master Access Control List', 30051, 0, N'M008-adminsuper', CAST(N'2019-05-08T02:27:02.913' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50082, N'VIEW_SURAT', N'View Page Surat', 30052, 0, N'M008-adminsuper', CAST(N'2019-07-18T10:58:01.363' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50083, N'CREATE_SURAT_RUJUKAN', N'CREATE SURAT RUJUKAN', 30052, 0, N'M008-adminsuper', CAST(N'2019-07-19T13:49:59.040' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50084, N'VIEW_M_GUDANG', N'MASTER GUDANG', 30053, 0, N'M008-adminsuper', CAST(N'2019-08-12T20:19:41.673' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50085, N'ADD_M_GUDANG', N'ADD MASTER GUDANG', 0, 0, N'M008-adminsuper', CAST(N'2019-08-12T20:20:14.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50086, N'EDIT_M_GUDANG', N'EDIT M GUDANG', 0, 0, N'M008-adminsuper', CAST(N'2019-08-12T20:21:06.997' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50087, N'DELETE_M_GUDANG', N'DELETE_M_GUDANG', 0, 0, N'M008-adminsuper', CAST(N'2019-08-12T20:35:28.330' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50088, N'DELETE_M_DELIVERYORDER', N'DELETE_M_DELIVERYORDER', 0, 0, N'M008-adminsuper', CAST(N'2019-08-13T07:42:24.607' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50089, N'EDIT_M_DELIVERYORDER', N'EDIT_M_DELIVERYORDER', 0, 0, N'M008-adminsuper', CAST(N'2019-08-13T07:42:47.110' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50090, N'EDIT_M_DELIVERYORDER', N'EDIT_M_DELIVERYORDER', 0, -1, N'M008-adminsuper', CAST(N'2019-08-13T07:43:00.597' AS DateTime), N'M008-adminsuper', CAST(N'2019-08-13T07:44:50.510' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50091, N'ADD_M_DELIVERYORDER', N'ADD_M_DELIVERYORDER', 0, 0, N'M008-adminsuper', CAST(N'2019-08-13T07:43:16.787' AS DateTime), NULL, CAST(N'2019-08-13T07:45:26.397' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50092, N'VIEW_M_DELIVERYORDER', N'VIEW_M_DELIVERYORDER', 30061, 0, N'M008-adminsuper', CAST(N'2019-08-13T07:43:32.670' AS DateTime), NULL, CAST(N'2019-08-17T17:09:01.943' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50093, N'VIEW_M_REEDOO', N'VIEW_M_REEDOO', 30056, 0, N'M008-adminsuper', CAST(N'2019-08-16T13:44:19.087' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50094, N'VIEW_M_DELIVERYORDERPUSAT', N'VIEW_M_DELIVERYORDERPUSAT', 30058, 0, N'M008-adminsuper', CAST(N'2019-08-16T13:49:53.377' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50095, N'ADD_M_DELIVERYORDERPUSAT', N'ADD_M_DELIVERYORDERPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-16T13:50:38.777' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50096, N'EDIT_M_DELIVERYORDERPUSAT', N'EDIT_M_DELIVERYORDERPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-16T13:50:53.863' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50097, N'DELETE_M_DELIVERYORDERPUSAT', N'DELETE_M_DELIVERYORDERPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-16T13:51:18.037' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50098, N'VIEW_M_PURCHASEORDER', N'VIEW_M_PURCHASEORDER', 30059, 0, N'M008-adminsuper', CAST(N'2019-08-16T22:00:55.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50099, N'ADD_M_PURCHASEORDER', N'ADD_M_PURCHASEORDER', 0, 0, N'M008-adminsuper', CAST(N'2019-08-16T22:01:06.177' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50100, N'EDIT_M_PURCHASEORDER', N'EDIT_M_PURCHASEORDER', 0, 0, N'M008-adminsuper', CAST(N'2019-08-16T22:01:13.020' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50101, N'DELETE_M_PURCHASEORDER', N'DELETE_M_PURCHASEORDER', 0, 0, N'M008-adminsuper', CAST(N'2019-08-16T22:01:17.447' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50102, N'VIEW_M_VENDOR', N'VIEW_M_VENDOR', 30060, 0, N'M008-adminsuper', CAST(N'2019-08-17T15:57:48.007' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50103, N'ADD_M_VENDOR', N'ADD_M_VENDOR', 0, 0, N'M008-adminsuper', CAST(N'2019-08-17T15:58:16.657' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50104, N'EDIT_M_VENDOR', N'EDIT_M_VENDOR', 0, 0, N'M008-adminsuper', CAST(N'2019-08-17T15:58:29.280' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50105, N'DELETE_M_VENDOR', N'DELETE_M_VENDOR', 0, 0, N'M008-adminsuper', CAST(N'2019-08-17T15:58:38.963' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50106, N'VIEW_FARMASI', N'View Farmasi Dasboard', 30062, 0, N'M008-adminsuper', CAST(N'2019-08-17T18:07:44.037' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50107, N'VIEW_M_PURCHASEORDERPUSAT', N'VIEW_M_PURCHASEORDERPUSAT', 30063, 0, N'M008-adminsuper', CAST(N'2019-08-17T21:37:32.467' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50108, N'ADD_M_PURCHASEORDERPUSAT', N'ADD_M_PURCHASEORDERPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-17T21:37:42.207' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50109, N'EDIT_M_PURCHASEORDERPUSAT', N'EDIT_M_PURCHASEORDERPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-17T21:37:48.283' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50110, N'DELETE_M_PURCHASEORDERPUSAT', N'DELETE_M_PURCHASEORDERPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-17T21:37:53.860' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50111, N'VIEW_M_PURCHASEREQUEST', N'VIEW_M_PURCHASEREQUEST', 30064, 0, N'M008-adminsuper', CAST(N'2019-08-18T13:11:52.370' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50112, N'ADD_M_PURCHASEREQUEST', N'ADD_M_PURCHASEREQUEST', 0, 0, N'M008-adminsuper', CAST(N'2019-08-18T13:12:03.123' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50113, N'EDIT_M_PURCHASEREQUEST', N'EDIT_M_PURCHASEREQUEST', 0, 0, N'M008-adminsuper', CAST(N'2019-08-18T13:12:08.297' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50114, N'DELETE_M_PURCHASEREQUEST', N'DELETE_M_PURCHASEREQUEST', 0, 0, N'M008-adminsuper', CAST(N'2019-08-18T13:12:13.530' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50115, N'VIEW_M_PURCHASEREQUESTPUSAT', N'VIEW_M_PURCHASEREQUESTPUSAT', 30073, 0, N'M008-adminsuper', CAST(N'2019-08-18T17:50:31.870' AS DateTime), NULL, CAST(N'2019-09-06T10:10:25.227' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50116, N'ADD_M_PURCHASEREQUESTPUSAT', N'ADD_M_PURCHASEREQUESTPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-18T17:50:43.270' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50117, N'EDIT_M_PURCHASEREQUESTPUSAT', N'EDIT_M_PURCHASEREQUESTPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-18T17:50:48.557' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50118, N'DELETE_M_PURCHASEREQUESTPUSAT', N'DELETE_M_PURCHASEREQUESTPUSAT', 0, 0, N'M008-adminsuper', CAST(N'2019-08-18T17:51:00.063' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50119, N'EDIT_FORM_EXAMINE_MEDICINE', N'EDIT_FORM_EXAMINE_MEDICINE', 0, 0, N'M008-adminsuper', CAST(N'2019-08-19T03:22:36.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50120, N'VIEW_M_PRODUCTINGUDANG', N'VIEW_M_PRODUCTINGUDANG', 30066, 0, N'M008-adminsuper', CAST(N'2019-08-23T10:30:56.923' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50121, N'ADD_M_PRODUCTINGUDANG', N'ADD_M_PRODUCTINGUDANG', 0, 0, N'M008-adminsuper', CAST(N'2019-08-23T10:31:12.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50122, N'EDIT_M_PRODUCTINGUDANG', N'EDIT_M_PRODUCTINGUDANG', 0, 0, N'M008-adminsuper', CAST(N'2019-08-23T10:31:19.720' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50123, N'DELETE_M_PRODUCTINGUDANG', N'DELETE_M_PRODUCTINGUDANG', 0, 0, N'M008-adminsuper', CAST(N'2019-08-23T10:31:27.303' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50124, N'VIEW_M_PURCHASEREQUESTCONFIG', N'VIEW_M_PURCHASEREQUESTCONFIG', 30069, 0, N'0721-mustarifin', CAST(N'2019-09-02T20:04:01.383' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50125, N'ADD_M_PURCHASEREQUESTCONFIG', N'ADD_M_PURCHASEREQUESTCONFIG', 0, 0, N'0721-mustarifin', CAST(N'2019-09-02T20:04:13.560' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50126, N'EDIT_M_PURCHASEREQUESTCONFIG', N'EDIT_M_PURCHASEREQUESTCONFIG', 0, 0, N'0721-mustarifin', CAST(N'2019-09-02T20:04:22.550' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50127, N'DELETE_M_PURCHASEREQUESTCONFIG', N'DELETE_M_PURCHASEREQUESTCONFIG', 0, 0, N'0721-mustarifin', CAST(N'2019-09-02T21:53:27.177' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50128, N'APPROVE_M_PURCHASEREQUEST', N'APPROVE_M_PURCHASEREQUEST', 0, 0, N'0721-mustarifin', CAST(N'2019-09-03T13:10:42.873' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50129, N'VALIDATION_M_PURCHASEREQUEST', N'VALIDATION_M_PURCHASEREQUEST', 0, 0, N'0721-mustarifin', CAST(N'2019-09-03T13:10:54.190' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50130, N'APPROVE_M_PURCHASEORDER', N'APPROVE_M_PURCHASEORDER', 0, 0, N'0721-mustarifin', CAST(N'2019-09-03T14:51:42.577' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50131, N'VALIDATION_M_PURCHASEORDER', N'VALIDATION_M_PURCHASEORDER', 0, 0, N'0721-mustarifin', CAST(N'2019-09-03T14:51:54.307' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50132, N'VIEW_PENGAMBILAN_OBAT', N'VIEW PENGAMBILAN OBAT', 30070, 0, N'M0011-admin', CAST(N'2019-09-03T21:21:35.723' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50133, N'VIEW_REPORTS', N'Display Reports', 30071, 0, N'M008-adminsuper', CAST(N'2019-09-06T05:36:28.190' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50134, N'APPROVE_M_PURCHASEREQUESTPUSAT', N'APPROVE_M_PURCHASEREQUESTPUSAT', 0, 0, N'0721-mustarifin', CAST(N'2019-09-06T09:56:44.310' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50135, N'VALIDATION_M_PURCHASEREQUESTPUSAT', N'VALIDATION_M_PURCHASEREQUESTPUSAT', 0, 0, N'0721-mustarifin', CAST(N'2019-09-06T09:57:25.833' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50136, N'VIEW_GUDANG_PUSAT', N'VIEW_GUDANG_PUSAT', 30072, 0, N'0721-mustarifin', CAST(N'2019-09-06T10:04:51.163' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50137, N'VIEW_TOP_10_DISEASES', N'Display top 10 diseases', 30076, 0, N'M008-adminsuper', CAST(N'2019-09-09T06:03:26.017' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50138, N'VIEW_TOP_10_REFERALS', N'Display Top 10 Referals Report', 30077, 0, N'M008-adminsuper', CAST(N'2019-09-09T06:20:02.833' AS DateTime), NULL, CAST(N'2019-09-09T06:26:11.603' AS DateTime))
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50139, N'VIEW_MEDICAL_HISTORY', N'VIEW MEDICAL HISTORY', 30078, 0, N'M0011-admin', CAST(N'2019-09-09T22:24:05.060' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50140, N'APPROVE_M_PURCHASEORDERPUSAT', N'APPROVE_M_PURCHASEORDERPUSAT', 0, 0, N'0721-mustarifin', CAST(N'2019-09-10T15:18:19.657' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50141, N'VALIDATION_M_PURCHASEORDERPUSAT', N'VALIDATION_M_PURCHASEORDERPUSAT', 0, 0, N'0721-mustarifin', CAST(N'2019-09-10T15:18:58.357' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50142, N'APPROVE_M_DELIVERYORDERPUSAT', N'APPROVE_M_DELIVERYORDERPUSAT', 0, 0, N'0721-mustarifin', CAST(N'2019-09-11T23:02:49.297' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50143, N'VALIDATION_M_DELIVERYORDERPUSAT', N'VALIDATION_M_DELIVERYORDERPUSAT', 0, 0, N'0721-mustarifin', CAST(N'2019-09-11T23:03:11.623' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50144, N'VIEW_M_LOOKUP_CATEGORY', N'Display Category List', 30081, 0, N'M008-adminsuper', CAST(N'2019-09-15T05:46:32.980' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50145, N'ADD_M_LOOKUP_CATEGORY', N'Add Look Up Category', 0, 0, N'M008-adminsuper', CAST(N'2019-09-15T05:46:50.853' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50146, N'EDIT_M_LOOKUP_CATEGORY', N'Edit Look Up category', 0, 0, N'M008-adminsuper', CAST(N'2019-09-15T05:47:01.467' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50147, N'DELETE_M_LOOKUP_CATEGORY', N'Delete Look Up Category', 0, 0, N'M008-adminsuper', CAST(N'2019-09-15T05:47:11.700' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Privilege] OFF
GO
SET IDENTITY_INSERT [dbo].[Product] ON 
GO
INSERT [dbo].[Product] ([ID], [Code], [Name], [ClinicID], [Vendor], [ProductCategoryID], [ProductUnitID], [RetailPrice], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'1-001', N'Novax tab', NULL, NULL, 1, 6, CAST(5000.0000 AS Decimal(19, 4)), 0, N'M008-adminsuper', CAST(N'2019-09-14T14:35:07.580' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Product] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductCategory] ON 
GO
INSERT [dbo].[ProductCategory] ([ID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Obat Oral', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:27:39.393' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[ProductCategory] ([ID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'Injection&BHP', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:38:48.237' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[ProductCategory] ([ID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'Racikan', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:38:59.223' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[ProductCategory] ([ID], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'Request', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:39:06.167' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[ProductCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductUnit] ON 
GO
INSERT [dbo].[ProductUnit] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, NULL, N'tube', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:39:36.490' AS DateTime), N'M008-adminsuper', CAST(N'2019-09-14T12:40:15.993' AS DateTime))
GO
INSERT [dbo].[ProductUnit] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, NULL, N'pcs', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:39:40.457' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[ProductUnit] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, NULL, N'bot', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:39:44.540' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[ProductUnit] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, NULL, N'pack', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:40:06.790' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[ProductUnit] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, NULL, N'vial', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:40:34.980' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[ProductUnit] ([ID], [Code], [Name], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, NULL, N'tab', 0, N'M008-adminsuper', CAST(N'2019-09-14T12:40:52.860' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[ProductUnit] OFF
GO
SET IDENTITY_INSERT [dbo].[RolePrivilege] ON 
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, 10002, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, 10003, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 1, 10006, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, 1, 10007, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, 1, 10004, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, 1, 10008, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, 1, 10013, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, 1, 20014, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, 1, 30013, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, 1, 30014, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, 1, 10016, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, 1, 10017, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, 1, 10015, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, 1, 10018, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (15, 1, 20013, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (16, 1, 20015, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (17, 1, 20016, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (18, 1, 20017, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (19, 1, 10010, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (20, 1, 10011, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (21, 1, 10005, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (22, 1, 10009, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (23, 1, 10012, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (24, 1, 10014, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (25, 1, 40001, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (26, 1, 40002, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (27, 1, 40003, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (28, 1, 40004, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (29, 1, 40006, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (30, 1, 40005, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (31, 1, 40007, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (32, 1, 40008, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (33, 1, 40009, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (34, 1, 40011, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (35, 1, 40012, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (36, 1, 40013, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (37, 1, 40014, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (38, 1, 40015, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (39, 1, 40016, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (40, 1, 40017, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (41, 1, 40019, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (42, 1, 40020, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (43, 1, 40021, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (44, 1, 40022, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (45, 1, 40023, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (46, 1, 40024, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (47, 1, 40025, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (48, 1, 40026, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (49, 1, 40027, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (50, 1, 40028, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (51, 1, 40029, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (52, 1, 40030, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (53, 1, 40031, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (54, 1, 40036, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (55, 1, 40037, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (56, 1, 40038, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (57, 1, 40039, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (58, 1, 40040, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (59, 1, 40041, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (60, 1, 40047, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (61, 1, 40046, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (62, 1, 40044, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (63, 1, 40045, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (64, 1, 40043, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (65, 1, 40042, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (66, 1, 40049, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (67, 1, 40048, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (68, 1, 40050, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (69, 1, 40053, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (70, 1, 40051, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (71, 1, 40052, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (72, 1, 40059, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (73, 1, 40058, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (74, 1, 40057, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (75, 1, 40056, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (76, 1, 40055, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (77, 1, 40054, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (78, 1, 40060, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (79, 1, 40061, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (80, 1, 50062, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (81, 1, 50061, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (82, 1, 50060, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (83, 1, 50063, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (84, 1, 50064, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (85, 1, 50065, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (86, 1, 50066, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (87, 1, 50067, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (88, 1, 50068, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (89, 1, 50070, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (90, 1, 50069, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (91, 1, 50072, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (92, 1, 50071, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (93, 1, 40018, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (94, 1, 50080, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (95, 1, 50079, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (96, 1, 50078, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (97, 1, 50081, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (98, 1, 50077, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (99, 1, 50076, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (100, 1, 50075, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (101, 1, 50074, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (102, 1, 50073, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (103, 1, 50082, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (104, 1, 50083, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (105, 1, 50086, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (106, 1, 50085, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (107, 1, 50084, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (108, 1, 50087, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (109, 1, 50092, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (110, 1, 50091, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (111, 1, 50089, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (112, 1, 50088, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (113, 1, 50093, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (114, 1, 50097, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (115, 1, 50096, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (116, 1, 50095, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (117, 1, 50094, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (118, 1, 50101, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (119, 1, 50100, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (120, 1, 50099, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (121, 1, 50098, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (122, 1, 50105, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (123, 1, 50104, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (124, 1, 50103, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (125, 1, 50102, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (126, 1, 50110, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (127, 1, 50109, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (128, 1, 50108, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (129, 1, 50107, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (130, 1, 50114, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (131, 1, 50113, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (132, 1, 50112, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (133, 1, 50111, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (134, 1, 50118, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (135, 1, 50117, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (136, 1, 50116, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (137, 1, 50115, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (138, 1, 10002, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (139, 1, 10003, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (140, 1, 10006, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (141, 1, 10007, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (142, 1, 10004, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (143, 1, 10008, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (144, 1, 10013, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (145, 1, 20014, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (146, 1, 30013, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (147, 1, 30014, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (148, 1, 10016, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (149, 1, 10017, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (150, 1, 10015, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (151, 1, 10018, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (152, 1, 20013, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (153, 1, 20015, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (154, 1, 20016, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (155, 1, 20017, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (156, 1, 10010, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (157, 1, 10011, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (158, 1, 10005, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (159, 1, 10009, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (160, 1, 10012, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (161, 1, 10014, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (162, 1, 40001, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (163, 1, 40002, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (164, 1, 40003, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (165, 1, 40004, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (166, 1, 40006, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (167, 1, 40005, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (168, 1, 40007, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (169, 1, 40008, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (170, 1, 40009, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (171, 1, 40011, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (172, 1, 40012, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (173, 1, 40013, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (174, 1, 40014, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (175, 1, 40015, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (176, 1, 40016, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (177, 1, 40017, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (178, 1, 40019, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (179, 1, 40020, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (180, 1, 40021, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (181, 1, 40022, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (182, 1, 40023, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (183, 1, 40024, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (184, 1, 40025, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (185, 1, 40026, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (186, 1, 40027, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (187, 1, 40028, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (188, 1, 40029, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (189, 1, 40030, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (190, 1, 40031, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (191, 1, 40036, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (192, 1, 40037, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (193, 1, 40038, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (194, 1, 40039, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (195, 1, 40040, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (196, 1, 40041, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (197, 1, 40047, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (198, 1, 40046, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (199, 1, 40044, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (200, 1, 40045, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (201, 1, 40043, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (202, 1, 40042, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (203, 1, 40049, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (204, 1, 40048, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (205, 1, 40050, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (206, 1, 40053, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (207, 1, 40051, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (208, 1, 40052, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (209, 1, 40059, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (210, 1, 40058, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (211, 1, 40057, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (212, 1, 40056, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (213, 1, 40055, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (214, 1, 40054, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (215, 1, 40060, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (216, 1, 40061, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (217, 1, 50062, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (218, 1, 50061, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (219, 1, 50060, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (220, 1, 50063, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (221, 1, 50064, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (222, 1, 50065, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (223, 1, 50066, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (224, 1, 50067, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (225, 1, 50068, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (226, 1, 50070, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (227, 1, 50069, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (228, 1, 50072, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (229, 1, 50071, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (230, 1, 40018, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (231, 1, 50080, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (232, 1, 50079, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (233, 1, 50078, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (234, 1, 50081, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (235, 1, 50077, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (236, 1, 50076, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (237, 1, 50075, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (238, 1, 50074, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (239, 1, 50073, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (240, 1, 50082, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (241, 1, 50083, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (242, 1, 50086, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (243, 1, 50085, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (244, 1, 50084, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (245, 1, 50087, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (246, 1, 50092, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (247, 1, 50091, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (248, 1, 50089, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (249, 1, 50088, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (250, 1, 50093, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (251, 1, 50097, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (252, 1, 50096, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (253, 1, 50095, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (254, 1, 50094, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (255, 1, 50101, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (256, 1, 50100, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (257, 1, 50099, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (258, 1, 50098, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (259, 1, 50105, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (260, 1, 50104, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (261, 1, 50103, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (262, 1, 50102, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (263, 1, 50110, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (264, 1, 50109, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (265, 1, 50108, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (266, 1, 50107, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (267, 1, 50114, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (268, 1, 50113, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (269, 1, 50112, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (270, 1, 50111, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (271, 1, 50118, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (272, 1, 50117, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (273, 1, 50116, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (274, 1, 50115, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (275, 1, 50106, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (276, 1, 50123, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (277, 1, 50122, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (278, 1, 50121, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (279, 1, 50120, NULL, 0, N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:48:08.493' AS DateTime))
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (381, 4, 50138, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (382, 4, 50137, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (383, 4, 50133, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (384, 4, 50123, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (385, 4, 50122, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (386, 4, 50121, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.430' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (387, 4, 50120, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.433' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (388, 4, 50119, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.433' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (389, 4, 50118, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.433' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (390, 4, 50117, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.433' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (391, 4, 50116, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.433' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (392, 4, 50115, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.433' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (393, 4, 50114, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.433' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (394, 4, 50113, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (395, 4, 50112, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (396, 4, 50111, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (397, 4, 50110, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (398, 4, 50109, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (399, 4, 50108, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (400, 4, 50107, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (401, 4, 50106, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.437' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (402, 4, 50105, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.440' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (403, 4, 50104, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.440' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (404, 4, 50103, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.440' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (405, 4, 50102, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.440' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (406, 4, 50101, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.443' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (407, 4, 50100, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.443' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (408, 4, 50099, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.443' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (409, 4, 50098, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.443' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (410, 4, 50097, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.447' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (411, 4, 50096, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.447' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (412, 4, 50095, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.447' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (413, 4, 50094, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.447' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (414, 4, 50093, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.447' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (415, 4, 50092, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.447' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (416, 4, 50091, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.450' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (417, 4, 50089, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.450' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (418, 4, 50088, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.450' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (419, 4, 50087, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.450' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (420, 4, 50086, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.453' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (421, 4, 50085, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.453' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (422, 4, 50084, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.453' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (423, 4, 50083, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.457' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (424, 4, 50082, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.457' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (425, 4, 50077, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.457' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (426, 4, 50076, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.457' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (427, 4, 50075, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.460' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (428, 4, 50074, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.460' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (429, 4, 50073, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.460' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (430, 4, 50081, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.463' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (431, 4, 50080, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.463' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (432, 4, 50079, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.467' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (433, 4, 50078, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.467' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (434, 4, 40018, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.467' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (435, 4, 50072, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.470' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (436, 4, 50071, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.473' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (437, 4, 50065, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.477' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (438, 4, 50066, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.477' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (439, 4, 50067, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.477' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (440, 4, 50068, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.480' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (441, 4, 50069, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.480' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (442, 4, 50070, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.483' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (443, 4, 50064, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.483' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (444, 4, 50063, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.487' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (445, 4, 50062, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.487' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (446, 4, 50061, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.490' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (447, 4, 50060, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (448, 4, 40061, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.497' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (449, 4, 40060, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.497' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (450, 4, 40059, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.497' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (451, 4, 40058, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.500' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (452, 4, 40057, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.503' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (453, 4, 40056, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.503' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (454, 4, 40055, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.507' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (455, 4, 40054, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.507' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (456, 4, 40053, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.507' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (457, 4, 40052, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.510' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (458, 4, 40051, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.510' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (459, 4, 40048, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.513' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (460, 4, 40049, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.517' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (461, 4, 40050, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.520' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (462, 4, 40047, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.523' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (463, 4, 40046, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.527' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (464, 4, 40045, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.527' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (465, 4, 40044, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.527' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (466, 4, 40043, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.530' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (467, 4, 40042, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.530' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (468, 4, 40041, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.533' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (469, 4, 40040, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.533' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (470, 4, 40039, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.537' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (471, 4, 40038, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.537' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (472, 4, 40037, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.540' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (473, 4, 40036, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.543' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (474, 4, 40035, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.543' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (475, 4, 40033, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.547' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (476, 4, 40031, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.550' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (477, 4, 40030, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.550' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (478, 4, 40029, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.553' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (479, 4, 40028, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.553' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (480, 4, 40027, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.557' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (481, 4, 40026, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.560' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (482, 4, 40025, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.560' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (483, 4, 40024, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.563' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (484, 4, 40023, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.567' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (485, 4, 40022, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.567' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (486, 4, 40021, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.567' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (487, 4, 40020, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.573' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (488, 4, 40019, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.577' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (489, 4, 40017, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.577' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (490, 4, 40016, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.580' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (491, 4, 40015, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.583' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (492, 4, 40014, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.583' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (493, 4, 40013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.587' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (494, 4, 40012, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.587' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (495, 4, 40011, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.590' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (496, 4, 40009, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.593' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (497, 4, 40008, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.597' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (498, 4, 40007, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.597' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (499, 4, 40006, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.600' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (500, 4, 40005, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.603' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (501, 4, 40004, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.603' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (502, 4, 40003, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.607' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (503, 4, 40002, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.610' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (504, 4, 40001, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.613' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (505, 4, 10014, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.613' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (506, 4, 10012, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.617' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (507, 4, 10009, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.620' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (508, 4, 10005, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.620' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (509, 4, 10011, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.623' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (510, 4, 10010, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.627' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (511, 4, 20017, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.627' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (512, 4, 20016, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.630' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (513, 4, 20015, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.633' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (514, 4, 20013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.637' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (515, 4, 10018, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.637' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (516, 4, 10015, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.640' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (517, 4, 10017, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.643' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (518, 4, 10016, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.647' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (519, 4, 30014, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.650' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (520, 4, 30013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.653' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (521, 4, 20014, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.657' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (522, 4, 10013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.660' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (523, 4, 10008, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.660' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (524, 4, 10004, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.663' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (525, 4, 10007, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.667' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (526, 4, 10006, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.670' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (527, 4, 10003, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.670' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (528, 4, 10002, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:17:31.673' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (536, 3, 10009, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (537, 3, 10008, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (538, 3, 10007, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (539, 3, 10006, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (540, 3, 50081, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (541, 3, 10013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (542, 3, 40033, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.210' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (543, 3, 20016, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.213' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (544, 3, 20015, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.213' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (545, 3, 20013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T10:18:12.213' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (546, 7, 50119, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:25:10.777' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (547, 7, 40033, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:25:10.780' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (548, 7, 10013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:25:10.780' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (549, 8, 50062, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.240' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (550, 8, 50061, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.240' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (551, 8, 50060, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.240' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (552, 8, 10013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.243' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (553, 8, 40001, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.243' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (554, 8, 40002, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.243' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (555, 8, 40003, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.243' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (556, 8, 40014, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.247' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (557, 8, 40015, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.247' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (558, 8, 40016, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.247' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (559, 8, 40017, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.247' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (560, 8, 40018, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.247' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (561, 8, 40023, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.247' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (562, 8, 40024, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.250' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (563, 8, 40025, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.250' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (564, 8, 40026, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.250' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (565, 8, 40027, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.250' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (566, 8, 40028, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.253' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (567, 8, 40029, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.253' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (568, 8, 40030, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.253' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (569, 8, 40031, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.257' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (570, 8, 50082, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.257' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (571, 8, 50083, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:27:17.257' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (572, 2, 10013, NULL, 0, N'M008-adminsuper', CAST(N'2019-09-17T05:36:32.213' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[RolePrivilege] OFF
GO
SET IDENTITY_INSERT [dbo].[Services] ON 
GO
INSERT [dbo].[Services] ([ID], [Code], [Name], [Price], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'S001', N'Chest X-Ray', 18050.0000, 0, N'M008-adminsuper', CAST(N'2019-09-14T12:17:28.737' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Services] ([ID], [Code], [Name], [Price], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'S002', N'EKG', 13236.0000, 0, N'M008-adminsuper', CAST(N'2019-09-14T12:17:45.293' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Services] ([ID], [Code], [Name], [Price], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'S003', N'Audiometri', 25872.0000, 0, N'M008-adminsuper', CAST(N'2019-09-14T12:18:02.203' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Services] ([ID], [Code], [Name], [Price], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, N'S004', N'Spirometri', 28279.0000, 0, N'M008-adminsuper', CAST(N'2019-09-14T12:18:23.180' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Services] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([ID], [OrganizationID], [UserName], [Password], [EmployeeID], [ExpiredDate], [ResetPasswordCode], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, N'adminsuper', N'lMdsjhhmPnQWj1fgjj4uJg==', NULL, CAST(N'2029-12-31T00:00:00.000' AS DateTime), NULL, 1, 0, N'SYSTEM', CAST(N'2019-03-02T07:32:38.017' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[User] ([ID], [OrganizationID], [UserName], [Password], [EmployeeID], [ExpiredDate], [ResetPasswordCode], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 2, N'drumum', N'OKbWGvMsBCE3vbs27Rqdaw==', 3, CAST(N'2019-12-23T17:55:47.877' AS DateTime), NULL, 1, 0, N'M008-adminsuper', CAST(N'2019-09-14T17:55:47.877' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[User] ([ID], [OrganizationID], [UserName], [Password], [EmployeeID], [ExpiredDate], [ResetPasswordCode], [Status], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, 2, N'admin', N'OKbWGvMsBCE3vbs27Rqdaw==', 4, CAST(N'2019-12-23T18:16:58.427' AS DateTime), NULL, 1, 0, N'M008-adminsuper', CAST(N'2019-09-14T18:16:58.427' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET IDENTITY_INSERT [dbo].[UserRole] ON 
GO
INSERT [dbo].[UserRole] ([Id], [UserID], [RoleID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, 1, 0, N'SYSTEM', CAST(N'2019-09-14T09:41:38.850' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:41:38.850' AS DateTime))
GO
INSERT [dbo].[UserRole] ([Id], [UserID], [RoleID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, 2, 0, N'SYSTEM', CAST(N'2019-09-14T09:41:38.850' AS DateTime), N'SYSTEM', CAST(N'2019-09-14T09:41:38.850' AS DateTime))
GO
INSERT [dbo].[UserRole] ([Id], [UserID], [RoleID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 3, 7, NULL, N'M008-adminsuper', CAST(N'2019-09-14T18:28:00.733' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[UserRole] ([Id], [UserID], [RoleID], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, 4, 8, NULL, N'M008-adminsuper', CAST(N'2019-09-14T18:28:09.560' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[UserRole] OFF
GO
ALTER TABLE [dbo].[Appointment] ADD  CONSTRAINT [DF_Appointment_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Clinic] ADD  CONSTRAINT [DF_Clinic_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Doctor] ADD  CONSTRAINT [DF_Doctor_TypeID]  DEFAULT ((0)) FOR [TypeID]
GO
ALTER TABLE [dbo].[Doctor] ADD  CONSTRAINT [DF_Doctor_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[DoctorClinic] ADD  CONSTRAINT [DF_DoctorClinic_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[EmployeeAssignment] ADD  CONSTRAINT [DF_EmployeeAssignment_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[EmployeeStatus] ADD  CONSTRAINT [DF_EmployeeStatus_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[FamilyRelationship] ADD  CONSTRAINT [DF_FamilyRelationship_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[FileArchieve] ADD  CONSTRAINT [DF_FileArchieve_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[FormExamineLab] ADD  CONSTRAINT [DF_FormExamineLab_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[GeneralMaster] ADD  CONSTRAINT [DF_GeneralMaster_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Gudang] ADD  CONSTRAINT [DF_Warehouses_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Log] ADD  CONSTRAINT [DF_Log_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[MCUPackage] ADD  CONSTRAINT [DF_MCUPackage_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Organization] ADD  CONSTRAINT [DF_Organization_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[OrganizationPrivilege] ADD  CONSTRAINT [DF_OrganizationPrivilege_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[OrganizationRole] ADD  CONSTRAINT [DF_OrganizationRole_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[PasswordHistory] ADD  CONSTRAINT [DF_PasswordHistory_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Patient] ADD  CONSTRAINT [DF_Patient_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[PatientClinic] ADD  CONSTRAINT [DF_PatientClinic_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Poli] ADD  CONSTRAINT [DF_Poli_Rowstatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[PoliClinic] ADD  CONSTRAINT [DF_PoliClinic_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[PoliFlowTemplate] ADD  CONSTRAINT [DF_PoliFlowTemplate_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[PoliSchedule] ADD  CONSTRAINT [DF_PoliSchedule_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[PoliScheduleMaster] ADD  CONSTRAINT [DF_PoliScheduleMaster_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Privilege] ADD  CONSTRAINT [DF_Privilege_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF__PurchaseO__statu__3A179ED3]  DEFAULT ((0)) FOR [statusop]
GO
ALTER TABLE [dbo].[PurchaseOrderDetail] ADD  CONSTRAINT [DF__PurchaseO__statu__4B422AD5]  DEFAULT ((0)) FOR [statusop]
GO
ALTER TABLE [dbo].[PurchaseOrderDetail] ADD  CONSTRAINT [DF_PurchaseOrderDetail_Verified]  DEFAULT ((0)) FOR [Verified]
GO
ALTER TABLE [dbo].[PurchaseOrderPusat] ADD  CONSTRAINT [DF__PurchaseOPusat__statu__3A179ED3]  DEFAULT ((0)) FOR [statusop]
GO
ALTER TABLE [dbo].[PurchaseOrderPusatDetail] ADD  CONSTRAINT [DF_PurchaseOrderPusatDetail_Verified]  DEFAULT ((0)) FOR [Verified]
GO
ALTER TABLE [dbo].[PurchaseRequest] ADD  CONSTRAINT [DF__PurchaseR__statu__3A179ED3]  DEFAULT ((0)) FOR [statusop]
GO
ALTER TABLE [dbo].[PurchaseRequestDetail] ADD  CONSTRAINT [DF__PurchaseRD__statu__4B422AD5]  DEFAULT ((0)) FOR [statusop]
GO
ALTER TABLE [dbo].[PurchaseRequestPusat] ADD  CONSTRAINT [DF__PurchaseRP__statu__3A179ED3]  DEFAULT ((0)) FOR [statusop]
GO
ALTER TABLE [dbo].[QueuePoli] ADD  CONSTRAINT [DF_QueuePoli_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[RolePrivilege] ADD  CONSTRAINT [DF_RolePrivilege_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[stok] ADD  DEFAULT ((0)) FOR [statusop]
GO
ALTER TABLE [dbo].[stoks] ADD  DEFAULT ((0)) FOR [statusop]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[UserRole] ADD  CONSTRAINT [DF_UserRole_RowStatus]  DEFAULT ((0)) FOR [RowStatus]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FK_Appointment_Clinic]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FK_Appointment_Employee]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_MCUPackage] FOREIGN KEY([MCUPackageID])
REFERENCES [dbo].[MCUPackage] ([ID])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FK_Appointment_MCUPackage]
GO
ALTER TABLE [dbo].[Clinic]  WITH NOCHECK ADD  CONSTRAINT [FK_Clinic_City] FOREIGN KEY([CityID])
REFERENCES [dbo].[City] ([Id])
GO
ALTER TABLE [dbo].[Clinic] NOCHECK CONSTRAINT [FK_Clinic_City]
GO
ALTER TABLE [dbo].[DeliveryOrder]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrder_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[DeliveryOrder] CHECK CONSTRAINT [FK_DeliveryOrder_Gudang]
GO
ALTER TABLE [dbo].[DeliveryOrder]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrder_PurchaseOrder] FOREIGN KEY([poid])
REFERENCES [dbo].[PurchaseOrder] ([id])
GO
ALTER TABLE [dbo].[DeliveryOrder] CHECK CONSTRAINT [FK_DeliveryOrder_PurchaseOrder]
GO
ALTER TABLE [dbo].[DeliveryOrder]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrder_Source] FOREIGN KEY([SourceId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[DeliveryOrder] CHECK CONSTRAINT [FK_DeliveryOrder_Source]
GO
ALTER TABLE [dbo].[DeliveryOrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrderDetail_DeliveryOrder] FOREIGN KEY([DeliveryOderId])
REFERENCES [dbo].[DeliveryOrder] ([id])
GO
ALTER TABLE [dbo].[DeliveryOrderDetail] CHECK CONSTRAINT [FK_DeliveryOrderDetail_DeliveryOrder]
GO
ALTER TABLE [dbo].[DeliveryOrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrderDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[DeliveryOrderDetail] CHECK CONSTRAINT [FK_DeliveryOrderDetail_Product]
GO
ALTER TABLE [dbo].[DeliveryOrderPusat]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrderPusat_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[DeliveryOrderPusat] CHECK CONSTRAINT [FK_DeliveryOrderPusat_Gudang]
GO
ALTER TABLE [dbo].[DeliveryOrderPusat]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrderPusat_PurchaseOrderPusat] FOREIGN KEY([poid])
REFERENCES [dbo].[PurchaseOrderPusat] ([id])
GO
ALTER TABLE [dbo].[DeliveryOrderPusat] CHECK CONSTRAINT [FK_DeliveryOrderPusat_PurchaseOrderPusat]
GO
ALTER TABLE [dbo].[DeliveryOrderPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrderPusatDetail_DeliveryOrderPusat] FOREIGN KEY([DeliveryOrderPusatId])
REFERENCES [dbo].[DeliveryOrderPusat] ([id])
GO
ALTER TABLE [dbo].[DeliveryOrderPusatDetail] CHECK CONSTRAINT [FK_DeliveryOrderPusatDetail_DeliveryOrderPusat]
GO
ALTER TABLE [dbo].[DeliveryOrderPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrderPusatDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[DeliveryOrderPusatDetail] CHECK CONSTRAINT [FK_DeliveryOrderPusatDetail_Product]
GO
ALTER TABLE [dbo].[DeliveryOrderPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryOrderPusatDetail_Vendor] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendor] ([id])
GO
ALTER TABLE [dbo].[DeliveryOrderPusatDetail] CHECK CONSTRAINT [FK_DeliveryOrderPusatDetail_Vendor]
GO
ALTER TABLE [dbo].[Doctor]  WITH CHECK ADD  CONSTRAINT [FK_Doctor_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[Doctor] CHECK CONSTRAINT [FK_Doctor_Employee]
GO
ALTER TABLE [dbo].[DoctorClinic]  WITH CHECK ADD  CONSTRAINT [FK_DoctorClinic_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[DoctorClinic] CHECK CONSTRAINT [FK_DoctorClinic_Clinic]
GO
ALTER TABLE [dbo].[DoctorClinic]  WITH CHECK ADD  CONSTRAINT [FK_DoctorClinic_Doctor] FOREIGN KEY([DoctorID])
REFERENCES [dbo].[Doctor] ([ID])
GO
ALTER TABLE [dbo].[DoctorClinic] CHECK CONSTRAINT [FK_DoctorClinic_Doctor]
GO
ALTER TABLE [dbo].[DoctorClinic]  WITH CHECK ADD  CONSTRAINT [FK_DoctorClinic_FileArchieve] FOREIGN KEY([PhotoID])
REFERENCES [dbo].[FileArchieve] ([ID])
GO
ALTER TABLE [dbo].[DoctorClinic] CHECK CONSTRAINT [FK_DoctorClinic_FileArchieve]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_EmployeeStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[EmployeeStatus] ([ID])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_EmployeeStatus]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_FamilyRelationship] FOREIGN KEY([EmpType])
REFERENCES [dbo].[FamilyRelationship] ([ID])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_FamilyRelationship]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_GeneralMaster] FOREIGN KEY([EmpType])
REFERENCES [dbo].[GeneralMaster] ([ID])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_GeneralMaster]
GO
ALTER TABLE [dbo].[EmployeeAssignment]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAssignment_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[EmployeeAssignment] CHECK CONSTRAINT [FK_EmployeeAssignment_Employee]
GO
ALTER TABLE [dbo].[EmployeeAssignment]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAssignment_EmployeeStatus] FOREIGN KEY([EmpStatus])
REFERENCES [dbo].[EmployeeStatus] ([ID])
GO
ALTER TABLE [dbo].[EmployeeAssignment] CHECK CONSTRAINT [FK_EmployeeAssignment_EmployeeStatus]
GO
ALTER TABLE [dbo].[FormExamine]  WITH NOCHECK ADD  CONSTRAINT [FK_FormExamine_Doctor] FOREIGN KEY([DoctorID])
REFERENCES [dbo].[Doctor] ([ID])
GO
ALTER TABLE [dbo].[FormExamine] NOCHECK CONSTRAINT [FK_FormExamine_Doctor]
GO
ALTER TABLE [dbo].[FormExamine]  WITH CHECK ADD  CONSTRAINT [FK_FormExamine_FormMedical] FOREIGN KEY([FormMedicalID])
REFERENCES [dbo].[FormMedical] ([ID])
GO
ALTER TABLE [dbo].[FormExamine] CHECK CONSTRAINT [FK_FormExamine_FormMedical]
GO
ALTER TABLE [dbo].[FormExamine]  WITH CHECK ADD  CONSTRAINT [FK_FormExamine_Poli] FOREIGN KEY([PoliID])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[FormExamine] CHECK CONSTRAINT [FK_FormExamine_Poli]
GO
ALTER TABLE [dbo].[FormExamineAttachment]  WITH CHECK ADD  CONSTRAINT [FK_FormExamineAttachment_FileArchieve] FOREIGN KEY([FileAttach])
REFERENCES [dbo].[FileArchieve] ([ID])
GO
ALTER TABLE [dbo].[FormExamineAttachment] CHECK CONSTRAINT [FK_FormExamineAttachment_FileArchieve]
GO
ALTER TABLE [dbo].[FormExamineAttachment]  WITH CHECK ADD  CONSTRAINT [FK_FormExamineAttachment_FormExamine] FOREIGN KEY([FormExamineID])
REFERENCES [dbo].[FormExamine] ([ID])
GO
ALTER TABLE [dbo].[FormExamineAttachment] CHECK CONSTRAINT [FK_FormExamineAttachment_FormExamine]
GO
ALTER TABLE [dbo].[FormExamineLab]  WITH CHECK ADD  CONSTRAINT [FK_FormExamineLab_FormMedical] FOREIGN KEY([FormMedicalID])
REFERENCES [dbo].[FormMedical] ([ID])
GO
ALTER TABLE [dbo].[FormExamineLab] CHECK CONSTRAINT [FK_FormExamineLab_FormMedical]
GO
ALTER TABLE [dbo].[FormExamineLab]  WITH CHECK ADD  CONSTRAINT [FK_FormExamineLab_LabItem] FOREIGN KEY([LabItemID])
REFERENCES [dbo].[LabItem] ([ID])
GO
ALTER TABLE [dbo].[FormExamineLab] CHECK CONSTRAINT [FK_FormExamineLab_LabItem]
GO
ALTER TABLE [dbo].[FormExamineMedicine]  WITH CHECK ADD  CONSTRAINT [FK_FormExamineMedicine_FormExamine] FOREIGN KEY([FormExamineID])
REFERENCES [dbo].[FormExamine] ([ID])
GO
ALTER TABLE [dbo].[FormExamineMedicine] CHECK CONSTRAINT [FK_FormExamineMedicine_FormExamine]
GO
ALTER TABLE [dbo].[FormExamineMedicine]  WITH NOCHECK ADD  CONSTRAINT [FK_FormExamineMedicine_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[FormExamineMedicine] NOCHECK CONSTRAINT [FK_FormExamineMedicine_Product]
GO
ALTER TABLE [dbo].[FormExamineMedicineDetail]  WITH CHECK ADD  CONSTRAINT [FK_FormExamineMedicineDetail_FormExamineMedicine] FOREIGN KEY([FormExamineMedicineID])
REFERENCES [dbo].[FormExamineMedicine] ([ID])
GO
ALTER TABLE [dbo].[FormExamineMedicineDetail] CHECK CONSTRAINT [FK_FormExamineMedicineDetail_FormExamineMedicine]
GO
ALTER TABLE [dbo].[FormExamineService]  WITH CHECK ADD  CONSTRAINT [FK_FormExamineService_FormExamine] FOREIGN KEY([FormExamineID])
REFERENCES [dbo].[FormExamine] ([ID])
GO
ALTER TABLE [dbo].[FormExamineService] CHECK CONSTRAINT [FK_FormExamineService_FormExamine]
GO
ALTER TABLE [dbo].[FormExamineService]  WITH CHECK ADD  CONSTRAINT [FK_FormExamineService_Service] FOREIGN KEY([ServiceID])
REFERENCES [dbo].[Services] ([ID])
GO
ALTER TABLE [dbo].[FormExamineService] CHECK CONSTRAINT [FK_FormExamineService_Service]
GO
ALTER TABLE [dbo].[FormMedical]  WITH CHECK ADD  CONSTRAINT [FK_FormMedical_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[FormMedical] CHECK CONSTRAINT [FK_FormMedical_Clinic]
GO
ALTER TABLE [dbo].[FormMedical]  WITH CHECK ADD  CONSTRAINT [FK_FormMedical_Patient] FOREIGN KEY([PatientID])
REFERENCES [dbo].[Patient] ([ID])
GO
ALTER TABLE [dbo].[FormMedical] CHECK CONSTRAINT [FK_FormMedical_Patient]
GO
ALTER TABLE [dbo].[FormPreExamine]  WITH NOCHECK ADD  CONSTRAINT [FK_FormPreExamine_Doctor] FOREIGN KEY([DoctorID])
REFERENCES [dbo].[Doctor] ([ID])
GO
ALTER TABLE [dbo].[FormPreExamine] NOCHECK CONSTRAINT [FK_FormPreExamine_Doctor]
GO
ALTER TABLE [dbo].[FormPreExamine]  WITH CHECK ADD  CONSTRAINT [FK_FormPreExamine_FormMedical] FOREIGN KEY([FormMedicalID])
REFERENCES [dbo].[FormMedical] ([ID])
GO
ALTER TABLE [dbo].[FormPreExamine] CHECK CONSTRAINT [FK_FormPreExamine_FormMedical]
GO
ALTER TABLE [dbo].[Gudang]  WITH CHECK ADD  CONSTRAINT [FK_Gudang_Clinic] FOREIGN KEY([ClinicId])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[Gudang] CHECK CONSTRAINT [FK_Gudang_Clinic]
GO
ALTER TABLE [dbo].[HistoryProductInGudang]  WITH CHECK ADD  CONSTRAINT [FK_HistoryProductInGudang_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[HistoryProductInGudang] CHECK CONSTRAINT [FK_HistoryProductInGudang_Gudang]
GO
ALTER TABLE [dbo].[HistoryProductInGudang]  WITH CHECK ADD  CONSTRAINT [FK_HistoryProductInGudang_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[HistoryProductInGudang] CHECK CONSTRAINT [FK_HistoryProductInGudang_Product]
GO
ALTER TABLE [dbo].[LabItem]  WITH CHECK ADD  CONSTRAINT [FK_LabItem_LabItemCategory] FOREIGN KEY([LabItemCategoryID])
REFERENCES [dbo].[LabItemCategory] ([ID])
GO
ALTER TABLE [dbo].[LabItem] CHECK CONSTRAINT [FK_LabItem_LabItemCategory]
GO
ALTER TABLE [dbo].[LabItemCategory]  WITH CHECK ADD  CONSTRAINT [FK_LabItemCategory_Poli] FOREIGN KEY([PoliID])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[LabItemCategory] CHECK CONSTRAINT [FK_LabItemCategory_Poli]
GO
ALTER TABLE [dbo].[Organization]  WITH NOCHECK ADD  CONSTRAINT [FK_Organization_Clinic] FOREIGN KEY([KlinikID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[Organization] NOCHECK CONSTRAINT [FK_Organization_Clinic]
GO
ALTER TABLE [dbo].[OrganizationPrivilege]  WITH CHECK ADD  CONSTRAINT [FK_OrganizationPrivilege_Organization] FOREIGN KEY([OrgID])
REFERENCES [dbo].[Organization] ([ID])
GO
ALTER TABLE [dbo].[OrganizationPrivilege] CHECK CONSTRAINT [FK_OrganizationPrivilege_Organization]
GO
ALTER TABLE [dbo].[OrganizationPrivilege]  WITH CHECK ADD  CONSTRAINT [FK_OrganizationPrivilege_Privilege] FOREIGN KEY([PrivilegeID])
REFERENCES [dbo].[Privilege] ([ID])
GO
ALTER TABLE [dbo].[OrganizationPrivilege] CHECK CONSTRAINT [FK_OrganizationPrivilege_Privilege]
GO
ALTER TABLE [dbo].[OrganizationRole]  WITH CHECK ADD  CONSTRAINT [FK_OrganizationRole_Organization] FOREIGN KEY([OrgID])
REFERENCES [dbo].[Organization] ([ID])
GO
ALTER TABLE [dbo].[OrganizationRole] CHECK CONSTRAINT [FK_OrganizationRole_Organization]
GO
ALTER TABLE [dbo].[PanggilanPoli]  WITH CHECK ADD  CONSTRAINT [FK_PanggilanPoli_Poli] FOREIGN KEY([PoliID])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[PanggilanPoli] CHECK CONSTRAINT [FK_PanggilanPoli_Poli]
GO
ALTER TABLE [dbo].[PasswordHistory]  WITH CHECK ADD  CONSTRAINT [FK_PasswordHistory_Organization] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([ID])
GO
ALTER TABLE [dbo].[PasswordHistory] CHECK CONSTRAINT [FK_PasswordHistory_Organization]
GO
ALTER TABLE [dbo].[Patient]  WITH NOCHECK ADD  CONSTRAINT [FK_Patient_City] FOREIGN KEY([CityID])
REFERENCES [dbo].[City] ([Id])
GO
ALTER TABLE [dbo].[Patient] NOCHECK CONSTRAINT [FK_Patient_City]
GO
ALTER TABLE [dbo].[Patient]  WITH NOCHECK ADD  CONSTRAINT [FK_Patient_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[Patient] NOCHECK CONSTRAINT [FK_Patient_Employee]
GO
ALTER TABLE [dbo].[PatientClinic]  WITH CHECK ADD  CONSTRAINT [FK_PatientClinic_City] FOREIGN KEY([TempCityID])
REFERENCES [dbo].[City] ([Id])
GO
ALTER TABLE [dbo].[PatientClinic] CHECK CONSTRAINT [FK_PatientClinic_City]
GO
ALTER TABLE [dbo].[PatientClinic]  WITH CHECK ADD  CONSTRAINT [FK_PatientClinic_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[PatientClinic] CHECK CONSTRAINT [FK_PatientClinic_Clinic]
GO
ALTER TABLE [dbo].[PatientClinic]  WITH NOCHECK ADD  CONSTRAINT [FK_PatientClinic_FileArchieve] FOREIGN KEY([PhotoID])
REFERENCES [dbo].[FileArchieve] ([ID])
GO
ALTER TABLE [dbo].[PatientClinic] NOCHECK CONSTRAINT [FK_PatientClinic_FileArchieve]
GO
ALTER TABLE [dbo].[PatientClinic]  WITH CHECK ADD  CONSTRAINT [FK_PatientClinic_Patient] FOREIGN KEY([PatientID])
REFERENCES [dbo].[Patient] ([ID])
GO
ALTER TABLE [dbo].[PatientClinic] CHECK CONSTRAINT [FK_PatientClinic_Patient]
GO
ALTER TABLE [dbo].[PoliClinic]  WITH CHECK ADD  CONSTRAINT [FK_PoliClinic_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[PoliClinic] CHECK CONSTRAINT [FK_PoliClinic_Clinic]
GO
ALTER TABLE [dbo].[PoliClinic]  WITH CHECK ADD  CONSTRAINT [FK_PoliClinic_Poli] FOREIGN KEY([PoliID])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[PoliClinic] CHECK CONSTRAINT [FK_PoliClinic_Poli]
GO
ALTER TABLE [dbo].[PoliSchedule]  WITH CHECK ADD  CONSTRAINT [FK_PoliSchedule_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[PoliSchedule] CHECK CONSTRAINT [FK_PoliSchedule_Clinic]
GO
ALTER TABLE [dbo].[PoliSchedule]  WITH CHECK ADD  CONSTRAINT [FK_PoliSchedule_Doctor] FOREIGN KEY([DoctorID])
REFERENCES [dbo].[Doctor] ([ID])
GO
ALTER TABLE [dbo].[PoliSchedule] CHECK CONSTRAINT [FK_PoliSchedule_Doctor]
GO
ALTER TABLE [dbo].[PoliSchedule]  WITH CHECK ADD  CONSTRAINT [FK_PoliSchedule_Poli] FOREIGN KEY([PoliID])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[PoliSchedule] CHECK CONSTRAINT [FK_PoliSchedule_Poli]
GO
ALTER TABLE [dbo].[PoliScheduleMaster]  WITH CHECK ADD  CONSTRAINT [FK_PoliScheduleMaster_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[PoliScheduleMaster] CHECK CONSTRAINT [FK_PoliScheduleMaster_Clinic]
GO
ALTER TABLE [dbo].[PoliScheduleMaster]  WITH CHECK ADD  CONSTRAINT [FK_PoliScheduleMaster_Doctor] FOREIGN KEY([DoctorID])
REFERENCES [dbo].[Doctor] ([ID])
GO
ALTER TABLE [dbo].[PoliScheduleMaster] CHECK CONSTRAINT [FK_PoliScheduleMaster_Doctor]
GO
ALTER TABLE [dbo].[PoliScheduleMaster]  WITH CHECK ADD  CONSTRAINT [FK_PoliScheduleMaster_Poli] FOREIGN KEY([PoliID])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[PoliScheduleMaster] CHECK CONSTRAINT [FK_PoliScheduleMaster_Poli]
GO
ALTER TABLE [dbo].[PoliServices]  WITH CHECK ADD  CONSTRAINT [FK_PoliServices_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[PoliServices] CHECK CONSTRAINT [FK_PoliServices_Clinic]
GO
ALTER TABLE [dbo].[PoliServices]  WITH CHECK ADD  CONSTRAINT [FK_PoliServices_Poli] FOREIGN KEY([PoliID])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[PoliServices] CHECK CONSTRAINT [FK_PoliServices_Poli]
GO
ALTER TABLE [dbo].[PoliServices]  WITH CHECK ADD  CONSTRAINT [FK_PoliServices_Services] FOREIGN KEY([ServicesID])
REFERENCES [dbo].[Services] ([ID])
GO
ALTER TABLE [dbo].[PoliServices] CHECK CONSTRAINT [FK_PoliServices_Services]
GO
ALTER TABLE [dbo].[Privilege]  WITH NOCHECK ADD  CONSTRAINT [FK_Privilege_Menu] FOREIGN KEY([MenuID])
REFERENCES [dbo].[Menu] ([ID])
GO
ALTER TABLE [dbo].[Privilege] NOCHECK CONSTRAINT [FK_Privilege_Menu]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Clinic]
GO
ALTER TABLE [dbo].[Product]  WITH NOCHECK ADD  CONSTRAINT [FK_Product_ProductCategory] FOREIGN KEY([ProductCategoryID])
REFERENCES [dbo].[ProductCategory] ([ID])
GO
ALTER TABLE [dbo].[Product] NOCHECK CONSTRAINT [FK_Product_ProductCategory]
GO
ALTER TABLE [dbo].[Product]  WITH NOCHECK ADD  CONSTRAINT [FK_Product_ProductUnit] FOREIGN KEY([ProductUnitID])
REFERENCES [dbo].[ProductUnit] ([ID])
GO
ALTER TABLE [dbo].[Product] NOCHECK CONSTRAINT [FK_Product_ProductUnit]
GO
ALTER TABLE [dbo].[ProductInGudang]  WITH NOCHECK ADD  CONSTRAINT [FK_ProductInGudang_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[ProductInGudang] NOCHECK CONSTRAINT [FK_ProductInGudang_Gudang]
GO
ALTER TABLE [dbo].[ProductInGudang]  WITH NOCHECK ADD  CONSTRAINT [FK_ProductInGudang_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[ProductInGudang] NOCHECK CONSTRAINT [FK_ProductInGudang_Product]
GO
ALTER TABLE [dbo].[ProductMedicine]  WITH CHECK ADD  CONSTRAINT [FK_ProductMedicine_Medicine] FOREIGN KEY([MedicineID])
REFERENCES [dbo].[Medicine] ([ID])
GO
ALTER TABLE [dbo].[ProductMedicine] CHECK CONSTRAINT [FK_ProductMedicine_Medicine]
GO
ALTER TABLE [dbo].[ProductMedicine]  WITH CHECK ADD  CONSTRAINT [FK_ProductMedicine_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[ProductMedicine] CHECK CONSTRAINT [FK_ProductMedicine_Product]
GO
ALTER TABLE [dbo].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_Gudang]
GO
ALTER TABLE [dbo].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_PurchaseRequest] FOREIGN KEY([PurchaseRequestId])
REFERENCES [dbo].[PurchaseRequest] ([id])
GO
ALTER TABLE [dbo].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_PurchaseRequest]
GO
ALTER TABLE [dbo].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_Source] FOREIGN KEY([SourceId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_Source]
GO
ALTER TABLE [dbo].[PurchaseOrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[PurchaseOrderDetail] CHECK CONSTRAINT [FK_PurchaseOrderDetail_Product]
GO
ALTER TABLE [dbo].[PurchaseOrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderDetail_PurchaseOrder] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[PurchaseOrder] ([id])
GO
ALTER TABLE [dbo].[PurchaseOrderDetail] CHECK CONSTRAINT [FK_PurchaseOrderDetail_PurchaseOrder]
GO
ALTER TABLE [dbo].[PurchaseOrderPusat]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderPusat_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[PurchaseOrderPusat] CHECK CONSTRAINT [FK_PurchaseOrderPusat_Gudang]
GO
ALTER TABLE [dbo].[PurchaseOrderPusat]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderPusat_PurchaseRequestPusat] FOREIGN KEY([PurchaseRequestId])
REFERENCES [dbo].[PurchaseRequestPusat] ([id])
GO
ALTER TABLE [dbo].[PurchaseOrderPusat] CHECK CONSTRAINT [FK_PurchaseOrderPusat_PurchaseRequestPusat]
GO
ALTER TABLE [dbo].[PurchaseOrderPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderPusatDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[PurchaseOrderPusatDetail] CHECK CONSTRAINT [FK_PurchaseOrderPusatDetail_Product]
GO
ALTER TABLE [dbo].[PurchaseOrderPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderPusatDetail_PurchaseOrderPusat] FOREIGN KEY([PurchaseOrderPusatId])
REFERENCES [dbo].[PurchaseOrderPusat] ([id])
GO
ALTER TABLE [dbo].[PurchaseOrderPusatDetail] CHECK CONSTRAINT [FK_PurchaseOrderPusatDetail_PurchaseOrderPusat]
GO
ALTER TABLE [dbo].[PurchaseOrderPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderPusatDetail_Vendor] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendor] ([id])
GO
ALTER TABLE [dbo].[PurchaseOrderPusatDetail] CHECK CONSTRAINT [FK_PurchaseOrderPusatDetail_Vendor]
GO
ALTER TABLE [dbo].[PurchaseRequest]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseRequest_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[PurchaseRequest] CHECK CONSTRAINT [FK_PurchaseRequest_Gudang]
GO
ALTER TABLE [dbo].[PurchaseRequestConfig]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseRequestConfig_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[PurchaseRequestConfig] CHECK CONSTRAINT [FK_PurchaseRequestConfig_Gudang]
GO
ALTER TABLE [dbo].[PurchaseRequestDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseRequestDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[PurchaseRequestDetail] CHECK CONSTRAINT [FK_PurchaseRequestDetail_Product]
GO
ALTER TABLE [dbo].[PurchaseRequestDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseRequestDetail_PurchaseRequest] FOREIGN KEY([PurchaseRequestId])
REFERENCES [dbo].[PurchaseRequest] ([id])
GO
ALTER TABLE [dbo].[PurchaseRequestDetail] CHECK CONSTRAINT [FK_PurchaseRequestDetail_PurchaseRequest]
GO
ALTER TABLE [dbo].[PurchaseRequestPusat]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseRequestPusat_Gudang] FOREIGN KEY([GudangId])
REFERENCES [dbo].[Gudang] ([id])
GO
ALTER TABLE [dbo].[PurchaseRequestPusat] CHECK CONSTRAINT [FK_PurchaseRequestPusat_Gudang]
GO
ALTER TABLE [dbo].[PurchaseRequestPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseRequestPusatDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[PurchaseRequestPusatDetail] CHECK CONSTRAINT [FK_PurchaseRequestPusatDetail_Product]
GO
ALTER TABLE [dbo].[PurchaseRequestPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseRequestPusatDetail_PurchaseRequestPusat] FOREIGN KEY([PurchaseRequestPusatId])
REFERENCES [dbo].[PurchaseRequestPusat] ([id])
GO
ALTER TABLE [dbo].[PurchaseRequestPusatDetail] CHECK CONSTRAINT [FK_PurchaseRequestPusatDetail_PurchaseRequestPusat]
GO
ALTER TABLE [dbo].[PurchaseRequestPusatDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseRequestPusatDetail_Vendor] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendor] ([id])
GO
ALTER TABLE [dbo].[PurchaseRequestPusatDetail] CHECK CONSTRAINT [FK_PurchaseRequestPusatDetail_Vendor]
GO
ALTER TABLE [dbo].[QueuePoli]  WITH CHECK ADD  CONSTRAINT [FK_QueuePoli_Clinic] FOREIGN KEY([ClinicID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[QueuePoli] CHECK CONSTRAINT [FK_QueuePoli_Clinic]
GO
ALTER TABLE [dbo].[QueuePoli]  WITH NOCHECK ADD  CONSTRAINT [FK_QueuePoli_Doctor] FOREIGN KEY([DoctorID])
REFERENCES [dbo].[Doctor] ([ID])
GO
ALTER TABLE [dbo].[QueuePoli] NOCHECK CONSTRAINT [FK_QueuePoli_Doctor]
GO
ALTER TABLE [dbo].[QueuePoli]  WITH NOCHECK ADD  CONSTRAINT [FK_QueuePoli_FormMedical] FOREIGN KEY([FormMedicalID])
REFERENCES [dbo].[FormMedical] ([ID])
GO
ALTER TABLE [dbo].[QueuePoli] NOCHECK CONSTRAINT [FK_QueuePoli_FormMedical]
GO
ALTER TABLE [dbo].[QueuePoli]  WITH CHECK ADD  CONSTRAINT [FK_QueuePoli_Patient] FOREIGN KEY([PatientID])
REFERENCES [dbo].[Patient] ([ID])
GO
ALTER TABLE [dbo].[QueuePoli] CHECK CONSTRAINT [FK_QueuePoli_Patient]
GO
ALTER TABLE [dbo].[QueuePoli]  WITH CHECK ADD  CONSTRAINT [FK_QueuePoli_PoliFrom] FOREIGN KEY([PoliFrom])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[QueuePoli] CHECK CONSTRAINT [FK_QueuePoli_PoliFrom]
GO
ALTER TABLE [dbo].[QueuePoli]  WITH CHECK ADD  CONSTRAINT [FK_QueuePoli_PoliTo] FOREIGN KEY([PoliTo])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[QueuePoli] CHECK CONSTRAINT [FK_QueuePoli_PoliTo]
GO
ALTER TABLE [dbo].[RolePrivilege]  WITH NOCHECK ADD  CONSTRAINT [FK_RolePrivilege_Menu] FOREIGN KEY([MenuID])
REFERENCES [dbo].[Menu] ([ID])
GO
ALTER TABLE [dbo].[RolePrivilege] NOCHECK CONSTRAINT [FK_RolePrivilege_Menu]
GO
ALTER TABLE [dbo].[RolePrivilege]  WITH CHECK ADD  CONSTRAINT [FK_RolePrivilege_OrganizationRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[OrganizationRole] ([ID])
GO
ALTER TABLE [dbo].[RolePrivilege] CHECK CONSTRAINT [FK_RolePrivilege_OrganizationRole]
GO
ALTER TABLE [dbo].[RolePrivilege]  WITH CHECK ADD  CONSTRAINT [FK_RolePrivilege_Privilege] FOREIGN KEY([PrivilegeID])
REFERENCES [dbo].[Privilege] ([ID])
GO
ALTER TABLE [dbo].[RolePrivilege] CHECK CONSTRAINT [FK_RolePrivilege_Privilege]
GO
ALTER TABLE [dbo].[stok]  WITH CHECK ADD  CONSTRAINT [FK_stok_m_Clinic] FOREIGN KEY([ClinicId])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[stok] CHECK CONSTRAINT [FK_stok_m_Clinic]
GO
ALTER TABLE [dbo].[stok]  WITH CHECK ADD  CONSTRAINT [FK_stok_m_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[stok] CHECK CONSTRAINT [FK_stok_m_Product]
GO
ALTER TABLE [dbo].[stoks]  WITH CHECK ADD  CONSTRAINT [FK_stoks_m_Clinic] FOREIGN KEY([ClinicId])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[stoks] CHECK CONSTRAINT [FK_stoks_m_Clinic]
GO
ALTER TABLE [dbo].[stoks]  WITH CHECK ADD  CONSTRAINT [FK_stoks_m_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[stoks] CHECK CONSTRAINT [FK_stoks_m_Product]
GO
ALTER TABLE [dbo].[substitute]  WITH CHECK ADD  CONSTRAINT [FK_substitute_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ID])
GO
ALTER TABLE [dbo].[substitute] CHECK CONSTRAINT [FK_substitute_Product]
GO
ALTER TABLE [dbo].[substitute]  WITH CHECK ADD  CONSTRAINT [FK_substitute_PurchaseOrderDetail] FOREIGN KEY([PurchaseOrderDetailId])
REFERENCES [dbo].[PurchaseOrderDetail] ([id])
GO
ALTER TABLE [dbo].[substitute] CHECK CONSTRAINT [FK_substitute_PurchaseOrderDetail]
GO
ALTER TABLE [dbo].[SuratRujukanLabKeluar]  WITH CHECK ADD  CONSTRAINT [FK_SuratRujukanLabKeluar_LabItem] FOREIGN KEY([LabItemId])
REFERENCES [dbo].[LabItem] ([ID])
GO
ALTER TABLE [dbo].[SuratRujukanLabKeluar] CHECK CONSTRAINT [FK_SuratRujukanLabKeluar_LabItem]
GO
ALTER TABLE [dbo].[User]  WITH NOCHECK ADD  CONSTRAINT [FK_User_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([ID])
GO
ALTER TABLE [dbo].[User] NOCHECK CONSTRAINT [FK_User_Employee]
GO
ALTER TABLE [dbo].[User]  WITH NOCHECK ADD  CONSTRAINT [FK_User_Organization] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([ID])
GO
ALTER TABLE [dbo].[User] NOCHECK CONSTRAINT [FK_User_Organization]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_OrganizationRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[OrganizationRole] ([ID])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_OrganizationRole]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
/****** Object:  StoredProcedure [dbo].[SP_EmployeeSync]    Script Date: 17/09/2019 05:46:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_EmployeeSync] 
	-- Add the parameters for the stored procedure here
	@LastUpdateTime DATETIME = '2019-08-01 00:00:00', 
	@RangeMinute INT = 10,
	@lastupdateby VARCHAR(30) = 'SYSTEM'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @CreatedTime DATETIME = GETDATE()
	SET @LastUpdateTime = DATEADD(MINUTE, (-1*@RangeMinute), @LastUpdateTime);
	
	--INSERT into Patient
	DECLARE @patientb TABLE (
		[EmployeeID] [bigint],
		[FamilyRelationshipID] [smallint],
		[MRNumber] [varchar](50),
		[Name] [varchar](100),
		[Gender] [char](1),
		[MaritalStatus] [char](1),
		[BirthDate] [datetime] ,
		[KTPNumber] [varchar](20) ,
		[Address] [varchar](100) ,
		[CityID] [int] ,
		[HPNumber] [nvarchar](50) ,
		[Type] [smallint] ,
		[BPJSNumber] [varchar](50) ,
		[BloodType] [char](2) ,
		[RowStatus] [smallint] ,
		[CreatedBy] [nvarchar](50) ,
		[CreatedDate] [datetime] ,
		[ModifiedBy] [nvarchar](50) ,
		[ModifiedDate] [datetime] ,
		[PatientKey] [nvarchar](100)
	)
	INSERT INTO @patientb([EmployeeID]
           ,[FamilyRelationshipID]
           ,[MRNumber]
           ,[Name]
           ,[Gender]
           ,[MaritalStatus]
           ,[BirthDate]
           ,[KTPNumber]
           ,[Address]
           ,[CityID]
           ,[HPNumber]
           ,[Type]
           ,[BPJSNumber]
           ,[BloodType]
           ,[RowStatus]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[PatientKey])
	SELECT a.ID,a.EmpType,dbo.fn_GenerateMRNumber(@CreatedTime,a.rn),a.EmpName,a.Gender,case when a.EmpType in (1,2) THEN 'M' ELSE 'S' END,a.BirthDate,
			a.KTPNumber,'KERINCI',1,a.HPNumber,2,'','',0,@lastupdateby,@CreatedTime,@lastupdateby,@CreatedTime,''
	FROM (
		SELECT x.*,
			row_number() over(order by (select 1)) as rn
		FROM Employee x WITH (nolock)
		WHERE x.RowStatus= 0 AND x.ModifiedDate >= @LastUpdateTime
	) a
	
	MERGE INTO dbo.Patient AS Targ
	USING @patientb AS Sourc
	ON (Targ.[EmployeeID] = Sourc.[EmployeeID] AND Targ.[FamilyRelationshipID] = Sourc.[FamilyRelationshipID])
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			[EmployeeID]
           ,[FamilyRelationshipID]
           ,[MRNumber]
           ,[Name]
           ,[Gender]
           ,[MaritalStatus]
           ,[BirthDate]
           ,[KTPNumber]
           ,[Address]
           ,[CityID]
           ,[HPNumber]
           ,[Type]
           ,[BPJSNumber]
           ,[BloodType]
           ,[RowStatus]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[PatientKey]
		)
		VALUES (Sourc.[EmployeeID], Sourc.[FamilyRelationshipID], Sourc.[MRNumber],Sourc.[Name], Sourc.[Gender], Sourc.[MaritalStatus]
				, Sourc.[BirthDate],Sourc.[KTPNumber], Sourc.[Address], Sourc.[CityID], Sourc.[HPNumber],Sourc.[Type], Sourc.[BPJSNumber], Sourc.[BloodType]
				, 0,Sourc.[CreatedBy], Sourc.[CreatedDate], Sourc.[ModifiedBy], Sourc.[ModifiedDate], Sourc.[PatientKey]);
		
	--Mapping to all Clinic
	DECLARE @PatientClinicTb TABLE(
		[PatientID] [bigint],
		[ClinicID] [bigint],
		[TempAddress] [varchar](100),
		[TempCityID] [int],
		[RefferencePerson] [varchar](50),
		[RefferenceNumber] [varchar](20),
		[RefferenceRelation] [int],
		[PhotoID] [bigint],
		[OldMRNumber] [varchar](50),
		[RowStatus] [smallint],
		[CreatedBy] [nvarchar](50),
		[CreatedDate] [datetime],
		[ModifiedBy] [nvarchar](50),
		[ModifiedDate] [datetime]
	)
	INSERT INTO @PatientClinicTb
        ([PatientID]
        ,[ClinicID]
        ,[TempAddress]
        ,[TempCityID]
        ,[RefferencePerson]
        ,[RefferenceNumber]
        ,[RefferenceRelation]
        ,[PhotoID]
        ,[OldMRNumber]
        ,[RowStatus]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate])
	SELECT a.ID,b.ID, a.Address,a.CityID,'','','','',1,0,@lastupdateby,@CreatedTime,@lastupdateby,@CreatedTime
	FROM Patient a WITH (nolock), Clinic b WITH (nolock)
	WHERE a.CreatedBy = @lastupdateby and a.CreatedDate = @CreatedTime AND b.RowStatus = 0
	
	--Mapping to all Clinic
	INSERT INTO @PatientClinicTb
        ([PatientID]
        ,[ClinicID]
        ,[TempAddress]
        ,[TempCityID]
        ,[RefferencePerson]
        ,[RefferenceNumber]
        ,[RefferenceRelation]
        ,[PhotoID]
        ,[OldMRNumber]
        ,[RowStatus]
        ,[CreatedBy]
        ,[CreatedDate]
        ,[ModifiedBy]
        ,[ModifiedDate])
	SELECT a.ID,b.ID, a.Address,a.CityID,'','','','',1,0,@lastupdateby,@CreatedTime,@lastupdateby,@CreatedTime
	FROM Patient a WITH (nolock), Clinic b WITH (nolock)
	WHERE b.CreatedDate >= @LastUpdateTime AND b.RowStatus = 0 AND a.ID not in (select s.PatientID from @PatientClinicTb s)

	MERGE INTO dbo.PatientClinic AS Targ
	USING @PatientClinicTb AS Sourc
	ON (Targ.[PatientID] = Sourc.[PatientID] AND Targ.[ClinicID] = Sourc.[ClinicID])
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
			[PatientID]
           ,[ClinicID]
           ,[TempAddress]
           ,[TempCityID]
           ,[RefferencePerson]
           ,[RefferenceNumber]
           ,[RefferenceRelation]
           ,[PhotoID]
           ,[OldMRNumber]
           ,[RowStatus]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
		)
		VALUES (Sourc.[PatientID], Sourc.[ClinicID], Sourc.[TempAddress], Sourc.[TempCityID], Sourc.[RefferencePerson]
				, Sourc.[RefferenceNumber],Sourc.[RefferenceRelation], Sourc.[PhotoID], Sourc.[OldMRNumber]
				, 0,Sourc.[CreatedBy], Sourc.[CreatedDate], Sourc.[ModifiedBy], Sourc.[ModifiedDate]);

	--INSERT INTO user
	DECLARE @OrgID BIGINT =0
	DECLARE @UserTb TABLE(
		[OrganizationID] [bigint],
		[UserName] [nvarchar](50),
		[Password] [nvarchar](250),
		[EmployeeID] [bigint],
		[ExpiredDate] [datetime],
		[ResetPasswordCode] [nvarchar](50),
		[Status] [bit],
		[RowStatus] [smallint],
		[CreatedBy] [nvarchar](50),
		[CreatedDate] [datetime],
		[ModifiedBy] [nvarchar](50),
		[ModifiedDate] [datetime]
	)

END
GO
/****** Object:  StoredProcedure [dbo].[usp_MCU_registration]    Script Date: 17/09/2019 05:46:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 

CREATE PROCEDURE [dbo].[usp_MCU_registration]

AS

 
--get data istransfer = N
DECLARE @interface TABLE 
(
	-- Add the column definitions for the TABLE variable here
	REG_ID INT,
	REG_NUMBER VARCHAR(10),
	RESERVE_DATE DATE,
	EMPL_ID INT,
	EMPL_NAME VARCHAR(40),
	SCHEDULE_ID INT,
	SCHEDULE_CODE VARCHAR(10),
	DOCUMENT_STATUS VARCHAR(50),
	IS_TRANSFERRED CHAR(1),
	CREATED_DATE DATETIME,
	CREATED_BY_NAME VARCHAR(50), 
	MODIFIED_DATE DATETIME,
	MODIFIED_BY_NAME VARCHAR(50)
)
DECLARE @jum int = 0, @RegNumber Varchar(50), @RegID INT

INSERT INTO @interface(
REG_ID,
REG_NUMBER,
RESERVE_DATE,
EMPL_ID,
EMPL_NAME,
SCHEDULE_ID,
SCHEDULE_CODE,
DOCUMENT_STATUS,
IS_TRANSFERRED,
CREATED_DATE,
CREATED_BY_NAME,
MODIFIED_DATE,
MODIFIED_BY_NAME
)
select REG_ID,
REG_NUMBER,
RESERVE_DATE,
EMPL_ID,
EMPL_NAME,
SCHECULE_ID,
SCHEDULE_CODE,
DOCUMENT_STATUS,
IS_TRANSFERRED,
CREATED_DATE,
CREATED_BY_NAME,
MODIFIED_DATE,
MODIFIED_BY_NAME
from MedicalCheckDB.dbo.fusp_registrations_get_by_status('SUBMITTED','N')

--select * from @interface
SELECT 'BEGIN'
SELECT * FROM MCURegistrationInterface

SET IDENTITY_INSERT MCURegistrationInterface ON

--INSERT IF NOT EXIST
INSERT INTO MCURegistrationInterface(
REG_ID,
REG_NUMBER,
RESERVE_DATE,
EMPL_ID,
EMPL_NAME,
SCHEDULE_ID,
SCHEDULE_CODE,
DOCUMENT_STATUS,
IS_TRANSFERRED,
CREATED_DATE,
CREATED_BY_NAME,
MODIFIED_DATE,
MODIFIED_BY_NAME
)
SELECT REG_ID,
REG_NUMBER,
RESERVE_DATE,
EMPL_ID,
EMPL_NAME,
SCHEDULE_ID,
SCHEDULE_CODE,
DOCUMENT_STATUS,
IS_TRANSFERRED,
GETDATE(),
'INTERFACE',
GETDATE(),
'INTERFACE'
FROM @interface
WHERE REG_NUMBER NOT IN (
SELECT REG_NUMBER FROM MCURegistrationInterface
)

SET IDENTITY_INSERT MCURegistrationInterface OFF

--SELECT 'Inset New Data'

--SELECT * FROM MCURegistrationInterface

SELECT @jum = COUNT(*)
FROM MCURegistrationInterface
WHERE IS_TRANSFERRED= 'N' AND DOCUMENT_STATUS = 'SUBMITTED'

--SELECT @jum

WHILE @jum > 0
BEGIN
	SELECT TOP 1 @RegNumber = REG_NUMBER, @RegID = REG_ID
	FROM MCURegistrationInterface
	WHERE IS_TRANSFERRED= 'N' AND DOCUMENT_STATUS = 'SUBMITTED'

	UPDATE MCURegistrationInterface
	SET IS_TRANSFERRED= 'Y', MODIFIED_BY_NAME = 'INTERFACE', MODIFIED_DATE = GETDATE() 
	WHERE REG_ID = @RegID

	
	exec MedicalCheckDB.dbo. usp_registrations_update_by_regnumber @RegNumber

	DELETE FROM @interface WHERE REG_ID = @RegID

	SELECT @jum = COUNT(*)
	FROM MCURegistrationInterface
	WHERE IS_TRANSFERRED= 'N' AND DOCUMENT_STATUS = 'SUBMITTED'

	--SELECT @jum
END

--SELECT 'Update Istransfer'

--SELECT * FROM MCURegistrationInterface

--UPDATE IF EXIST
INSERT INTO @interface(
REG_ID,
REG_NUMBER,
RESERVE_DATE,
EMPL_ID,
EMPL_NAME,
SCHEDULE_ID,
SCHEDULE_CODE,
DOCUMENT_STATUS,
IS_TRANSFERRED,
CREATED_DATE,
CREATED_BY_NAME,
MODIFIED_DATE,
MODIFIED_BY_NAME
)
select REG_ID,
REG_NUMBER,
RESERVE_DATE,
EMPL_ID,
EMPL_NAME,
SCHECULE_ID,
SCHEDULE_CODE,
DOCUMENT_STATUS,
IS_TRANSFERRED,
CREATED_DATE,
CREATED_BY_NAME,
MODIFIED_DATE,
MODIFIED_BY_NAME
from MedicalCheckDB.dbo.fusp_registrations_get_by_status('CANCELED','N')

SELECT @jum = COUNT(*)
FROM @interface

--SELECT 'Jumlah Update',@jum

IF @jum>0
BEGIN
UPDATE a
SET
a.RESERVE_DATE=b.RESERVE_DATE,
a.EMPL_ID=b.EMPL_ID,
a.EMPL_NAME=b.EMPL_NAME,
a.SCHEDULE_ID=b.SCHEDULE_ID,
a.SCHEDULE_CODE=b.SCHEDULE_CODE,
a.DOCUMENT_STATUS=b.DOCUMENT_STATUS,
a.IS_TRANSFERRED=b.IS_TRANSFERRED,
a.CREATED_DATE=GETDATE(),
a.CREATED_BY_NAME='INTERFACE',
a.MODIFIED_DATE=GETDATE(),
a.MODIFIED_BY_NAME='INTERFACE'
FROM MCURegistrationInterface a
JOIN @interface b on a.REG_ID=b.REG_ID

--SELECT 'Result UPDATE'

--select a.*
--from MCURegistrationInterface a
--JOIN @interface b on a.REG_ID=b.REG_ID

SELECT @jum = COUNT(*)
FROM MCURegistrationInterface
WHERE IS_TRANSFERRED= 'N' --AND DOCUMENT_STATUS = 'SUBMITTED'

SELECT @jum

WHILE @jum > 0
BEGIN
	SELECT TOP 1 @RegNumber = REG_NUMBER, @RegID = REG_ID
	FROM MCURegistrationInterface
	WHERE IS_TRANSFERRED= 'N' --AND DOCUMENT_STATUS = 'SUBMITTED'

	UPDATE MCURegistrationInterface
	SET IS_TRANSFERRED= 'Y', MODIFIED_BY_NAME = 'INTERFACE', MODIFIED_DATE = GETDATE()
	WHERE REG_ID = @RegID

	
	exec MedicalCheckDB.dbo. usp_registrations_update_by_regnumber @RegNumber

	DELETE FROM @interface WHERE REG_ID = @RegID

	SELECT @jum = COUNT(*)
	FROM MCURegistrationInterface
	WHERE IS_TRANSFERRED= 'N' --AND DOCUMENT_STATUS = 'SUBMITTED'

	--SELECT @jum
END

END

SELECT 'Result'

SELECT * FROM MCURegistrationInterface
GO
/****** Object:  StoredProcedure [dbo].[usp_registrations_get_by_status]    Script Date: 17/09/2019 05:46:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 

CREATE PROCEDURE [dbo].[usp_registrations_get_by_status]

@DocumentStatus nvarchar(50),

@IsTransferred char(1)

AS

 

SELECT a.REG_ID, a.REG_NUMBER, a.RESERVE_DATE, a.EMPL_ID, a.EMPL_NAME, a.SCHEDULE_ID, a.SCHEDULE_CODE, a.DOCUMENT_STATUS,

          a.IS_TRANSFERRED, a.CREATED_DATE, a.CREATED_BY_NAME, a.MODIFIED_DATE, a.MODIFIED_BY_NAME

FROM MCURegistrationInterface a

WHERE a.DOCUMENT_STATUS=@DocumentStatus AND a.IS_TRANSFERRED =@IsTransferred

 
GO
/****** Object:  StoredProcedure [dbo].[usp_registrations_update_by_regnumber]    Script Date: 17/09/2019 05:46:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 

CREATE PROCEDURE [dbo].[usp_registrations_update_by_regnumber]

@RegNumber nvarchar(50)

 

AS

 

UPDATE McRegistrationInterface SET IS_TRANSFERRED='Y', MODIFIED_DATE = GETDATE(), MODIFIED_BY_NAME = 'MCURS_Service'

WHERE REG_NUMBER=@RegNumber
GO
USE [master]
GO
ALTER DATABASE [KlinikDBUAT] SET  READ_WRITE 
GO
