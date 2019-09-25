USE [master]
GO
/****** Object:  Database [Kli]    Script Date: 25/09/2019 21:23:49 ******/
CREATE DATABASE [Kli]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Kli', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\Kli.mdf' , SIZE = 6144KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Kli_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\Kli_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Kli] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Kli].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Kli] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Kli] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Kli] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Kli] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Kli] SET ARITHABORT OFF 
GO
ALTER DATABASE [Kli] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Kli] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Kli] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Kli] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Kli] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Kli] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Kli] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Kli] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Kli] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Kli] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Kli] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Kli] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Kli] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Kli] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Kli] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Kli] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Kli] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Kli] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Kli] SET  MULTI_USER 
GO
ALTER DATABASE [Kli] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Kli] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Kli] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Kli] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [Kli]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GenerateMRNumber]    Script Date: 25/09/2019 21:23:49 ******/
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
/****** Object:  UserDefinedFunction [dbo].[fusp_registrations_get_by_status]    Script Date: 25/09/2019 21:23:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fusp_registrations_get_by_status] 
(
	@DocumentStatus nvarchar(50),
	@IsTransferred char(1)
)
RETURNS 
@interface TABLE 
(
	-- Add the column definitions for the TABLE variable here
	REG_ID INT,
	REG_NUMBER VARCHAR(10),
	RESERVE_DATE DATE,
	EMPL_ID INT,
	EMPL_NAME VARCHAR(40),
	SCHECULE_ID INT,
	SCHEDULE_CODE VARCHAR(10),
	DOCUMENT_STATUS VARCHAR(50),
	IS_TRANSFERRED CHAR(1),
	CREATED_DATE DATETIME,
	CREATED_BY_NAME VARCHAR(50), 
	MODIFIED_DATE DATETIME,
	MODIFIED_BY_NAME VARCHAR(50)
)
AS
BEGIN
	-- Fill the table variable with the rows for your result set
	INSERT INTO @interface(
	REG_ID,
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
	)
	SELECT a.REG_ID, a.REG_NUMBER, a.RESERVE_DATE, a.EMPL_ID, a.EMPL_NAME, a.SCHECULE_ID, a.SCHEDULE_CODE, a.DOCUMENT_STATUS,a.IS_TRANSFERRED, a.CREATED_DATE, a.CREATED_BY_NAME, a.MODIFIED_DATE, a.MODIFIED_BY_NAME
	FROM McRegistrationInterface a
	WHERE a.DOCUMENT_STATUS=@DocumentStatus AND a.IS_TRANSFERRED =@IsTransferred
	
	RETURN 
END

GO
/****** Object:  UserDefinedFunction [dbo].[GetAge]    Script Date: 25/09/2019 21:23:49 ******/
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
/****** Object:  UserDefinedFunction [dbo].[GetAgeCategory]    Script Date: 25/09/2019 21:23:49 ******/
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
/****** Object:  Table [dbo].[AppConfig]    Script Date: 25/09/2019 21:23:49 ******/
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
/****** Object:  Table [dbo].[Appointment]    Script Date: 25/09/2019 21:23:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointment](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PatientID] [bigint] NULL,
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
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[Jam] [datetime] NULL,
 CONSTRAINT [PK_Appointment] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[City]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[Clinic]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[DeliveryOrder]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[DeliveryOrderDetail]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[DeliveryOrderPusat]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[DeliveryOrderPusatDetail]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[Doctor]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[DoctorClinic]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[Employee]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[EmployeeAssignment]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[EmployeeStatus]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FamilyRelationship]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FileArchieve]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FormExamine]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FormExamineAttachment]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FormExamineICDInfo]    Script Date: 25/09/2019 21:23:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormExamineICDInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FormExamineId] [bigint] NULL,
	[ICDId] [bigint] NULL,
	[RowStatus] [int] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_FormExamineICDInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormExamineLab]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FormExamineMedicine]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FormExamineMedicineDetail]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FormExamineService]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FormMedical]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[FormPreExamine]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[GeneralMaster]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[Gudang]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[HistoryProductInGudang]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[ICDTheme]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[LabItem]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[LabItemCategory]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[Letter]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[Log]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[LookupCategory]    Script Date: 25/09/2019 21:23:50 ******/
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
/****** Object:  Table [dbo].[MCUPackage]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[MCURegistrationInterface]    Script Date: 25/09/2019 21:23:51 ******/
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
	[MODIFIED_BY_NAME] [varchar](50) NULL,
 CONSTRAINT [PK_MCURegistrationInterface] PRIMARY KEY CLUSTERED 
(
	[REG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Medicine]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[Menu]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[Organization]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[OrganizationPrivilege]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[OrganizationRole]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PanggilanPoli]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PasswordHistory]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[Patient]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PatientAge]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PatientClinic]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[Poli]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PoliClinic]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PoliFlowTemplate]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PoliSchedule]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PoliScheduleMaster]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PoliServices]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[Privilege]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[Product]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[ProductInGudang]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[ProductMedicine]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[ProductUnit]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PurchaseOrderDetail]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PurchaseOrderPusat]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PurchaseOrderPusatDetail]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PurchaseRequest]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PurchaseRequestConfig]    Script Date: 25/09/2019 21:23:51 ******/
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
	[request_by] [varchar](30) NULL,
	[Status] [bit] NULL,
 CONSTRAINT [PK__PurchaseRC__3213E83FAC292246] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseRequestDetail]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PurchaseRequestPusat]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[PurchaseRequestPusatDetail]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[QueuePoli]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[RolePrivilege]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[Services]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[stok]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[stok_bulanan]    Script Date: 25/09/2019 21:23:51 ******/
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
/****** Object:  Table [dbo].[stoks]    Script Date: 25/09/2019 21:23:52 ******/
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
/****** Object:  Table [dbo].[substitute]    Script Date: 25/09/2019 21:23:52 ******/
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
/****** Object:  Table [dbo].[SuratRujukanLabKeluar]    Script Date: 25/09/2019 21:23:52 ******/
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
/****** Object:  Table [dbo].[User]    Script Date: 25/09/2019 21:23:52 ******/
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
/****** Object:  Table [dbo].[UserRole]    Script Date: 25/09/2019 21:23:52 ******/
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
/****** Object:  Table [dbo].[Vendor]    Script Date: 25/09/2019 21:23:52 ******/
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
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_Doctor] FOREIGN KEY([DoctorID])
REFERENCES [dbo].[Doctor] ([ID])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FK_Appointment_Doctor]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_MCUPackage] FOREIGN KEY([MCUPackageID])
REFERENCES [dbo].[MCUPackage] ([ID])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FK_Appointment_MCUPackage]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_Patient] FOREIGN KEY([PatientID])
REFERENCES [dbo].[Patient] ([ID])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FK_Appointment_Patient]
GO
ALTER TABLE [dbo].[Appointment]  WITH CHECK ADD  CONSTRAINT [FK_Appointment_Poli] FOREIGN KEY([PoliID])
REFERENCES [dbo].[Poli] ([ID])
GO
ALTER TABLE [dbo].[Appointment] CHECK CONSTRAINT [FK_Appointment_Poli]
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
/****** Object:  StoredProcedure [dbo].[SP_EmployeeInsert]    Script Date: 25/09/2019 21:23:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_EmployeeInsert] 
	-- Add the parameters for the stored procedure here
	@EmpID	varchar(50),
	@EmpName	varchar(100),
	--@BirthNation	varchar(100),
	--@BirthCity	varchar(100),
	@BirthDate	date,
	@Gender	varchar(50),
	@EmpType	varchar(50),
	@KTPNumber	varchar(50) = '',
	@HPNumber	varchar(50) = '',
	--@BPJSNumber	varchar(100) = '',
	@Email	varchar(50),
	@LastEmpID	varchar(50),
	@StartDate	datetime = '1753-01-01',
	@EndDate	datetime = '1753-01-01',
	@Department	varchar(50),
	@BusinessUnit	varchar(50),
	@Region	varchar(50),
	@Grade	varchar(50) = '',
	@EmpStatus	varchar(50),
	@LastUpdateTime DATETIME = NULL, 
	@lastupdateby VARCHAR(30) = 'SYSTEM'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @reffID int, @GenderID char(1), @empTypeID int, @employeeID int, @employeeIDreff int, @lastemployeeID int, @statusID int, @empstatusID int

	IF @LastUpdateTime is null
	BEGIN
		SET @LastUpdateTime = GETDATE()
	END

	IF LEN(@EmpType) = 0
	BEGIN
		SET @EmpType = 'Employee'
	END

	SELECT TOP 1 @empTypeID = COALESCE(a.ID,0)
	FROM FamilyRelationship a
	WHERE a.RowStatus = 0 AND a.Name like @EmpType+'%'
	ORDER BY ID DESC

	SELECT @empTypeID

	IF @empTypeID = 0
	BEGIN
		SET @empTypeID = 1
	END

	IF @empTypeID > 0 
	BEGIN
		SEt @employeeIDreff = 0

		SELECT @employeeIDreff = COALESCE(a.id,0)
		FROM Employee a
		JOIN EmployeeAssignment b on a.id = b.EmployeeID
		WHERE a.RowStatus = 0 AND b.RowStatus = 0 AND a.EmpID = @EmpID 
				AND a.Status = 1

		select 'id emp',@employeeIDreff

		IF LEN(@LastEmpID)>0
		BEGIN
			SELECT @lastemployeeID = COALESCE(a.id,0)
			FROM Employee a
			JOIN EmployeeAssignment b on a.id = b.EmployeeID
			WHERE a.RowStatus = 0 AND b.RowStatus = 0 AND a.EmpID = @LastEmpID
		END
		ELSE
		BEGIN
			SET @lastemployeeID = 0
		END

		select 'lastempid',@lastemployeeID

		IF LEN(@EmpStatus)>0
		BEGIN
			SELECT @empstatusID = a.ID, @statusID = CASE WHEN a.Status = 'F' THEN 0 ELSE 1 END
			FROM EmployeeStatus a
			WHERE a.RowStatus = 0 AND a.Name = @EmpStatus
		END
		ELSE
		BEGIN
			SET @empstatusID = 0
			SET @statusID = 0
		END

		SELECT @empstatusID,@statusID

		IF @empTypeID = 1 --employee
		BEGIN
			IF @employeeIDreff = 0
			BEGIN
				--insert employee
				SET @GenderID = LEFT(@Gender,1)

				IF @GenderID NOT iN ('M','F')
				BEGIN
					SET @GenderID = 'M'
				END

				INSERT INTO [dbo].[Employee]
				   ([EmpID]
				   ,[EmpName]
				   --,[BirthNation]
				   --,[BirthCity]
				   ,[BirthDate]
				   ,[ReffEmpID]
				   ,[Gender]
				   ,[EmpType]
				   ,[KTPNumber]
				   ,[HPNumber]
				   --,[BPJSNumber]
				   ,[Email]
				   ,[LastEmpID]
				   ,[Status]
				   ,[RowStatus]
				   ,[CreatedBy]
				   ,[CreatedDate]
				   ,[ModifiedBy]
				   ,[ModifiedDate])
			 VALUES
				   (@EmpID
				   ,@EmpName
				   --,@BirthNation
				   --,@BirthCity
				   ,@BirthDate
				   ,@EmpID
				   ,@GenderID
				   ,@empTypeID
				   ,@KTPNumber
				   ,@HPNumber
				   --,@BPJSNumber
				   ,@Email
				   ,@lastemployeeID
				   ,@statusID
				   ,0
				   ,@lastupdateby
				   ,@LastUpdateTime
				   ,@lastupdateby
				   ,@LastUpdateTime)
				
				set @employeeID = (SELECT SCOPE_IDENTITY())
				SELECT 'INSERT Employee Assignment ' + @EmpID + ' = ' + CONVERT(varchar(10),@employeeID) + ' SUKSES'

				INSERT INTO [dbo].[EmployeeAssignment]
				   ([EmployeeID]
				   ,[StartDate]
				   ,[EndDate]
				   ,[Department]
				   ,[BusinessUnit]
				   ,[Region]
				   ,[Grade]
				   ,[EmpStatus]
				   ,[LastEmpID]
				   ,[RowStatus]
				   ,[CreatedBy]
				   ,[CreatedDate]
				   ,[ModifiedBy]
				   ,[ModifiedDate])
			 VALUES
				   (@employeeID
				   ,@StartDate
				   ,@EndDate
				   ,@Department
				   ,@BusinessUnit
				   ,@Region
				   ,@Grade
				   ,@empstatusID
				   ,@lastemployeeID
				   ,0
				   ,@lastupdateby
				   ,@LastUpdateTime
				   ,@lastupdateby
				   ,@LastUpdateTime)

			SELECT 'INSERT Employee Assignment ' + @EmpID + ' SUKSES'
			END
			ELSE
			BEGIN
				--UPDATE
				UPDATE [dbo].[Employee]
			   SET [EmpID] = @EmpID
				  ,[EmpName] = @EmpName
				  --,[BirthNation] = @BirthNation
				  --,[BirthCity] = @BirthCity
				  --,[BPJSNumber] = @BPJSNumber
				  ,[BirthDate] = @BirthDate
				  ,[ReffEmpID] = @EmpID
				  ,[Gender] = @GenderID
				  ,[EmpType] = @empTypeID
				  ,[KTPNumber] = @KTPNumber
				  ,[HPNumber] = @HPNumber
				  ,[Email] = @Email
				  ,[LastEmpID] = @lastemployeeID
				  ,[Status] = @statusID
				  ,[RowStatus] = 0
				  ,[ModifiedBy] = @lastupdateby
				  ,[ModifiedDate] = @LastUpdateTime
			 WHERE ID = @employeeIDreff

			 SELECT 'UPDATE Employee ' + @EmpID + ' SUKSES'
			END
		END
		ELSE
		BEGIN
			--family
			DECLARE @jumchild int,@empcode varchar(50),@famcode varchar(10)

			SELECT @employeeID = a.ID, @empcode = a.EmpID, @statusID = a.Status
			FROm Employee a
			WHERE a.RowStatus = 0 and a.Status = 1 and a.ReffEmpID = @EmpID and a.EmpName = @EmpName

			IF @employeeID > 0
			BEGIN
				--update
				UPDATE [dbo].[Employee]
				   SET [EmpName] = @EmpName
					  --,[BirthNation] = @BirthNation
					  --,[BirthCity] = @BirthCity
					  ,[BirthDate] = @BirthDate
					  ,[Gender] = @GenderID
					  ,[KTPNumber] = @KTPNumber
					  ,[HPNumber] = @HPNumber
					  --,[BPJSNumber] = @BPJSNumber
					  ,[Email] = @Email
					  ,[RowStatus] = 0
					  ,[ModifiedBy] = @lastupdateby
					  ,[ModifiedDate] = @LastUpdateTime
				 WHERE ID = @employeeID

				 SELECT 'UPDATE Employee Family ' + @empcode + ' SUKSES'
			END
			ELSE
			BEGIN
				--insert
				SELECT @employeeID = COALESCE(a.ID,0), @EmpID = a.EmpID, @statusID = a.Status
				FROm Employee a
				WHERE a.RowStatus = 0 and a.Status = 1 and a.EmpID = @EmpID 
				
				IF @employeeID >0
				BEGIN
					IF @EmpType = 'Spouse'
					BEGIN
						SET @famcode = 'S'
					END
					ELSE
					BEGIN
						SELECT TOP 1 @famcode = COALESCE(b.Code,'')
						FROm Employee a
						JOIN FamilyRelationship b on a.EmpType = b.ID
						WHERE a.RowStatus = 0 and a.Status = 1 and a.ReffEmpID = @EmpID and b.Name like 'Child%'
						ORDER BY b.iD DESC

						IF LEN(@famcode) = 0 OR @famcode IS NULL
						BEGIN
							SET @famcode = 'C1'
						END
						ELSE
						BEGIN
							SELECT @famcode,LEN(@famcode),RIGHT(@famcode,LEN(@famcode))
							SET @jumchild = CONVERT(INT,RIGHT(@famcode,LEN(@famcode)))
							SET @jumchild = @jumchild+1
							SET @famcode = LEFT(@famcode,1) + CONVERT(varchar(2),@jumchild)
						END
					END
					

					SET @empcode = @EmpID

					SELECT TOP 1 @empTypeID = COALESCE(a.ID,0)
					FROM FamilyRelationship a
					WHERE a.RowStatus = 0 AND a.Code = @famcode
					ORDER BY ID DESC

					SET @EmpID = @EmpID + @famcode

					INSERT INTO [dbo].[Employee]
						([EmpID]
						,[EmpName]
						--,[BirthNation]
						--,[BirthCity]
						,[BirthDate]
						,[ReffEmpID]
						,[Gender]
						,[EmpType]
						,[KTPNumber]
						,[HPNumber]
						--,[BPJSNumber]
						,[Email]
						,[LastEmpID]
						,[Status]
						,[RowStatus]
						,[CreatedBy]
						,[CreatedDate]
						,[ModifiedBy]
						,[ModifiedDate])
					VALUES
						(@EmpID
						,@EmpName
						--,@BirthNation
						--,@BirthCity
						,@BirthDate
						,@empcode
						,@GenderID
						,@empTypeID
						,@KTPNumber
						,@HPNumber
						--,@BPJSNumber
						,@Email
						,0
						,@statusID
						,0
						,@lastupdateby
						,@LastUpdateTime
						,@lastupdateby
						,@LastUpdateTime)
					SELECT 'INSERT Employee Famili ' + @EmpID + ' SUKSES'
				END
				ELSE
				BEGIN
					SELECT 'INSERT Employee Famili ' + @EmpID + ' GAGAL employee RAPP TIDAK AKTIF'
				END
			END
		END
	END
	ELSE
	BEGIN
		SELECT 'Tipe Employee belum ada'
	END

END

GO
/****** Object:  StoredProcedure [dbo].[SP_EmployeeSync]    Script Date: 25/09/2019 21:23:52 ******/
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
	SELECT a.ID,a.EmpType,dbo.fn_GenerateMRNumber(@CreatedTime,a.rn),a.EmpName,COALESCE(a.Gender,'M'),case when a.EmpType in (1,2) THEN 'M' ELSE 'S' END,a.BirthDate,
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
/****** Object:  StoredProcedure [dbo].[SP_GeneratePoliSchedule]    Script Date: 25/09/2019 21:23:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GeneratePoliSchedule] 
	-- Add the parameters for the stored procedure here
	@StartDate DATE = '20190801', 
	@Range INT = 1,
	@lastupdateby VARCHAR(30) = 'SYSTEM'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET DATEFIRST 7;
	SET DATEFORMAT mdy;
	SET LANGUAGE US_ENGLISH;

	DECLARE @EndDate DATE = DATEADD(MONTH, @Range, @StartDate);
	DECLARE @months TABLE (sdate date,sday int)

	INSERT INTO @months(sdate,sday)
	SELECT d as tggl,(DATEPART(dw,d)-1) as hari
	FROM
	(
	  SELECT d = DATEADD(DAY, rn - 1, @StartDate)
	  FROM 
	  (
		SELECT TOP (DATEDIFF(DAY, @StartDate, @EndDate)) 
		  rn = ROW_NUMBER() OVER (ORDER BY s1.[object_id])
		FROM sys.all_objects AS s1
		CROSS JOIN sys.all_objects AS s2
		ORDER BY s1.[object_id]
	  ) AS x
	) AS y;

	UPDATE @months set sday = 7 WHERE sday = 0

	DECLARE @sch TABLE(
		[ClinicID] BIGINT
      ,[DoctorID] INT
      ,[PoliID] INT
      ,[StartDate] DATETIME
      ,[EndDate] DATETIME
      ,[ReffID] BIGINT
      ,[Remark] varchar(150)
      ,[Status] INT
      ,[RowStatus] SMALLINT
      ,[CreatedBy] nvarchar(50)
      ,[CreatedDate] DATETIME
      ,[ModifiedBy] VARCHAR(30)
      ,[ModifiedDate] nvarchar(50)
	)
	INSERT INTO @sch
	SELECT a.ClinicID,
				a.DoctorID,
				a.PoliID,
				CAST(b.sdate AS DATETIME) + CAST(a.StartTime AS DATETIME) as startdate,
				CAST(b.sdate AS DATETIME) + CAST(a.endtime AS DATETIME) as enddate,
				0 as reffID,
				'' as remark,
				1 as status,
				0 as rowstatus,
				@lastupdateby as createdBy,
				GETDATE() as createddate,
				@lastupdateby as modifiedby,
				GETDATE() as modifieddate 
	FROM PoliScheduleMaster a
	OUTER APPLY(
		SELECT *
		FROM @months b
		WHERE b.sday = a.Day
	) b
	WHERE a.RowStatus = 0

	--select * from @sch

	MERGE INTO dbo.PoliSchedule AS Target
	USING @sch AS Source
	ON (Target.ClinicID = Source.ClinicID AND Target.PoliID = Source.PoliID AND Target.DoctorID = Source.DoctorID AND Target.startdate = Source.startdate AND Target.enddate = Source.enddate)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (
				[ClinicID]
			  ,[DoctorID]
			  ,[PoliID]
			  ,[StartDate]
			  ,[EndDate]
			  ,[ReffID]
			  ,[Remark]
			  ,[Status]
			  ,[RowStatus]
			  ,[CreatedBy]
			  ,[CreatedDate]
			  ,[ModifiedBy]
			  ,[ModifiedDate]
		)
		VALUES (Source.[ClinicID], Source.[DoctorID], Source.[PoliID],Source.[StartDate], Source.[EndDate], Source.[ReffID]
				, Source.[Remark],Source.[Status], Source.[RowStatus], Source.[CreatedBy]
				,Source.[CreatedDate], Source.[ModifiedBy], Source.[ModifiedDate]);
	

END

GO
/****** Object:  StoredProcedure [dbo].[usp_MCU_registration]    Script Date: 25/09/2019 21:23:52 ******/
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
from KlinikDB.dbo.fusp_registrations_get_by_status('SUBMITTED','N')

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

	
	exec KlinikDB.dbo.usp_registrations_update_by_regnumber @RegNumber

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
from KlinikDB.dbo.fusp_registrations_get_by_status('CANCELED','N')

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

	
	exec KlinikDB.dbo.usp_registrations_update_by_regnumber @RegNumber

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
/****** Object:  StoredProcedure [dbo].[usp_registrations_get_by_status]    Script Date: 25/09/2019 21:23:52 ******/
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
/****** Object:  StoredProcedure [dbo].[usp_registrations_update_by_regnumber]    Script Date: 25/09/2019 21:23:52 ******/
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
ALTER DATABASE [Kli] SET  READ_WRITE 
GO
