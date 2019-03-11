USE [master]
GO
/****** Object:  Database [KlinikDB]    Script Date: 3/11/2019 12:45:47 PM ******/
CREATE DATABASE [KlinikDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'KlinikDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\KlinikDB.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'KlinikDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\KlinikDB_log.ldf' , SIZE = 1536KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [KlinikDB] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [KlinikDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [KlinikDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [KlinikDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [KlinikDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [KlinikDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [KlinikDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [KlinikDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [KlinikDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [KlinikDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [KlinikDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [KlinikDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [KlinikDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [KlinikDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [KlinikDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [KlinikDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [KlinikDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [KlinikDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [KlinikDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [KlinikDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [KlinikDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [KlinikDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [KlinikDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [KlinikDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [KlinikDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [KlinikDB] SET  MULTI_USER 
GO
ALTER DATABASE [KlinikDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [KlinikDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [KlinikDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [KlinikDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [KlinikDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'KlinikDB', N'ON'
GO
USE [KlinikDB]
GO
/****** Object:  Table [dbo].[Clinic]    Script Date: 3/11/2019 12:45:47 PM ******/
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
	[CityID] [bigint] NULL,
	[ClinicType] [bigint] NULL,
	[ReffID] [int] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[DateCreated] [datetime] NULL,
	[LastUpdateBy] [varchar](50) NULL,
	[DateModified] [datetime] NULL,
 CONSTRAINT [PK_clinic] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[EmpID] [varchar](50) NULL,
	[EmpName] [varchar](100) NOT NULL,
	[BirthDate] [date] NULL,
	[Gender] [char](1) NULL,
	[EmpType] [bigint] NULL,
	[EmpDept] [bigint] NULL,
	[RowStatus] [smallint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[Email] [nvarchar](50) NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GeneralMaster]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeneralMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Value] [nvarchar](250) NULL,
 CONSTRAINT [PK_GeneralMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Start] [datetime] NOT NULL,
	[Module] [varchar](50) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[Organization] [varchar](20) NULL,
	[Command] [text] NULL,
	[OldValue] [nvarchar](max) NULL,
	[NewValue] [nvarchar](max) NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
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
	[icon] [nvarchar](50) NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Organization]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Organization](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrgCode] [nvarchar](30) NOT NULL,
	[OrgName] [nvarchar](50) NOT NULL,
	[KlinikID] [bigint] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganizationPrivilege]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganizationPrivilege](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrgID] [bigint] NOT NULL,
	[PrivilegeID] [bigint] NOT NULL,
 CONSTRAINT [PK_OrganizationPrivilege] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrganizationRole]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganizationRole](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrgID] [bigint] NOT NULL,
	[RoleName] [varchar](30) NOT NULL,
 CONSTRAINT [PK_OrganizationRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PasswordHistory]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrganizationID] [bigint] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_PasswordHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Privilege]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Privilege](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Privilege_Name] [nvarchar](150) NOT NULL,
	[Privilege_Desc] [nvarchar](500) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[MenuID] [bigint] NULL,
 CONSTRAINT [PK_Privilege] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePrivilege]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePrivilege](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleID] [bigint] NOT NULL,
	[PrivilegeID] [bigint] NOT NULL,
	[MenuId] [bigint] NULL,
 CONSTRAINT [PK_RolePrivilege] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[ID] [bigint] NOT NULL,
	[OrganizationID] [bigint] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
	[EmployeeID] [bigint] NOT NULL,
	[ExpiredDate] [datetime] NULL,
	[Status] [bit] NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[ResetPasswordCode] [varchar](100) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 3/11/2019 12:45:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NOT NULL,
	[RoleID] [bigint] NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Clinic] ON 

INSERT [dbo].[Clinic] ([ID], [Code], [Name], [Address], [LegalNumber], [LegalDate], [ContactNumber], [Email], [Lat], [Long], [CityID], [ClinicType], [ReffID], [RowStatus], [CreatedBy], [DateCreated], [LastUpdateBy], [DateModified]) VALUES (1, N'K0001', N'Klinik Jahanam', N'Bandung', N'1234', CAST(N'2019-01-01' AS Date), N'870028283', N'klinik.jahanam@yahoo.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Clinic] ([ID], [Code], [Name], [Address], [LegalNumber], [LegalDate], [ContactNumber], [Email], [Lat], [Long], [CityID], [ClinicType], [ReffID], [RowStatus], [CreatedBy], [DateCreated], [LastUpdateBy], [DateModified]) VALUES (2, N'K0002', N'Klinik Gajebo', N'Jakarta', N'877654', CAST(N'2018-12-31' AS Date), N'38778438432', N'klinik.gajebo@gmail.com', 0, 0, 10, 18, NULL, NULL, NULL, NULL, N'SYSTEM', CAST(N'2019-03-10T21:44:20.297' AS DateTime))
INSERT [dbo].[Clinic] ([ID], [Code], [Name], [Address], [LegalNumber], [LegalDate], [ContactNumber], [Email], [Lat], [Long], [CityID], [ClinicType], [ReffID], [RowStatus], [CreatedBy], [DateCreated], [LastUpdateBy], [DateModified]) VALUES (4, N'C56678', N'Klinik Nakal', N'owihfoiw', N'12345', CAST(N'2020-02-01' AS Date), NULL, N'nakal@gmail.com', 0, 0, 12, 18, 0, 0, N'SYSTEM', CAST(N'2019-03-10T22:46:04.843' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Clinic] OFF
SET IDENTITY_INSERT [dbo].[Employee] ON 

INSERT [dbo].[Employee] ([id], [EmpID], [EmpName], [BirthDate], [Gender], [EmpType], [EmpDept], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Email]) VALUES (1, N'A00057', N'Aditya Wahyu B', CAST(N'1988-03-02' AS Date), N'L', 1, 4, NULL, NULL, NULL, NULL, NULL, N'adityawa@outlook.com')
INSERT [dbo].[Employee] ([id], [EmpID], [EmpName], [BirthDate], [Gender], [EmpType], [EmpDept], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Email]) VALUES (2, N'B007892', N'Beny Susanto', CAST(N'1985-01-01' AS Date), N'L', 1, 4, NULL, NULL, NULL, NULL, NULL, N'benny.susanto@bsi.co.id')
INSERT [dbo].[Employee] ([id], [EmpID], [EmpName], [BirthDate], [Gender], [EmpType], [EmpDept], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Email]) VALUES (3, N'D00012', N'Dwi Arisandy', CAST(N'1982-01-03' AS Date), N'F', 3, 7, NULL, N'SYSTEM', CAST(N'2019-02-19T12:31:04.063' AS DateTime), N'SYSTEM', CAST(N'2019-03-10T21:56:22.210' AS DateTime), N'darisandy@yahoo.com')
INSERT [dbo].[Employee] ([id], [EmpID], [EmpName], [BirthDate], [Gender], [EmpType], [EmpDept], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Email]) VALUES (5, N'G3366502', N'Arif Hidayat', CAST(N'1983-08-01' AS Date), N'M', 3, 4, NULL, N'SYSTEM', CAST(N'2019-02-24T21:14:14.010' AS DateTime), N'SYSTEM', CAST(N'2019-03-10T21:53:58.480' AS DateTime), N'areefhide@gmail.com')
INSERT [dbo].[Employee] ([id], [EmpID], [EmpName], [BirthDate], [Gender], [EmpType], [EmpDept], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Email]) VALUES (10005, N'99999', N'Om Admin', CAST(N'1980-01-01' AS Date), N'M', 1, 4, NULL, N'SYSTEM', CAST(N'2019-03-02T07:32:05.973' AS DateTime), NULL, NULL, N'adytnesta@yahoo.com')
SET IDENTITY_INSERT [dbo].[Employee] OFF
SET IDENTITY_INSERT [dbo].[GeneralMaster] ON 

INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (1, N'EmploymentType', N'Permanent', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (2, N'EmploymentType', N'Outsource', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (3, N'EmploymentType', N'PKWT', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (4, N'Department', N'ISA', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (5, N'Department', N'Sales', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (6, N'Department', N'ERP', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (7, N'Department', N'HRD', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (8, N'Department', N'Accounting', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (9, N'City', N'DKI Jakarta', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (10, N'City', N'Bogor', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (11, N'City', N'Depok', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (12, N'City', N'Tangerang', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (13, N'City', N'Semarang', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (14, N'City', N'Riau', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (15, N'City', N'Surabaya', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (16, N'City', N'DI Yogyakarta', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (17, N'ClinicType', N'HO', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (18, N'ClinicType', N'Klinik', NULL)
SET IDENTITY_INSERT [dbo].[GeneralMaster] OFF
SET IDENTITY_INSERT [dbo].[Log] ON 

INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (1, CAST(N'2019-03-11T11:40:32.033' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (2, CAST(N'2019-03-11T12:14:03.117' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'UNRECOGNIZED')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (3, CAST(N'2019-03-11T12:14:08.457' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (4, CAST(N'2019-03-11T12:15:40.417' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (5, CAST(N'2019-03-11T12:22:42.427' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (6, CAST(N'2019-03-11T12:23:14.637' AS DateTime), N'MASTER_ORGANIZATION', N'adminsuper', NULL, N'Add New Organization', NULL, N'{"ID":10005,"OrgCode":"HRM001","OrgName":"Hermina","KlinikID":4,"CreatedBy":null,"CreatedDate":"2019-03-11T12:23:14.6232147+07:00","ModifiedBy":null,"ModifiedDate":null,"Clinic":null,"OrganizationPrivileges":[],"OrganizationRoles":[],"Users":[]}', N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (7, CAST(N'2019-03-11T12:26:32.737' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (8, CAST(N'2019-03-11T12:27:45.437' AS DateTime), N'MASTER_ORGANIZATION', N'adminsuper', NULL, N'Add New Organization', NULL, N'{"OrgCode":"HRM001","OrgName":"Hermina Hospital Group","KlinikID":4,"ClinicLists":null,"Id":10005,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":20003,"EmployeeID":10005,"Roles":[10005],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40015,40032,40018],"Id":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":null}}', N'ERROR')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (9, CAST(N'2019-03-11T12:29:43.717' AS DateTime), N'MASTER_ORGANIZATION', N'adminsuper', NULL, N'Add New Organization', NULL, N'{"OrgCode":"HRM001","OrgName":"Hermina Hospital Group","KlinikID":4,"ClinicLists":null,"Id":10005,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":20003,"EmployeeID":10005,"Roles":[10005],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40015,40032,40018],"Id":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":null}}', N'ERROR')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (10, CAST(N'2019-03-11T12:30:41.360' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (11, CAST(N'2019-03-11T12:38:46.097' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (12, CAST(N'2019-03-11T12:39:04.730' AS DateTime), N'MASTER_ORGANIZATION', N'adminsuper', NULL, N'Add New Organization', N'{"OrgCode":"HRM001","OrgName":"Hermina RS","KlinikID":4,"ClinicLists":null,"Id":10005,"CreatedDate":"2019-03-11T12:23:14.623","ModifiedDate":"2019-03-11T12:39:01.9817228+07:00","CreatedBy":null,"ModifiedBy":null,"Account":null}', N'{"OrgCode":"HRM001","OrgName":"Hermina RS","KlinikID":4,"ClinicLists":null,"Id":10005,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":20003,"EmployeeID":10005,"Roles":[10005],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40015,40032,40018],"Id":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":null}}', N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (13, CAST(N'2019-03-11T12:44:30.630' AS DateTime), N'LOGIN', N'adminsuper', N'M008', N'Login To System', NULL, NULL, N'SUCCESS')
INSERT [dbo].[Log] ([Id], [Start], [Module], [UserName], [Organization], [Command], [OldValue], [NewValue], [Status]) VALUES (14, CAST(N'2019-03-11T12:44:46.547' AS DateTime), N'MASTER_ORGANIZATION', N'adminsuper', NULL, N'Add New Organization', N'{"OrgCode":"HRM001","OrgName":"Hermina RS","KlinikID":4,"ClinicLists":null,"Id":10005,"CreatedDate":"2019-03-11T12:23:14.623","ModifiedDate":"2019-03-11T12:39:01.983","CreatedBy":null,"ModifiedBy":null,"Account":null}', N'{"OrgCode":"HRM001","OrgName":"Hermina Hospital Group","KlinikID":4,"ClinicLists":null,"Id":10005,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":{"UserName":"adminsuper","Password":null,"Email":null,"ResetPasswordCode":null,"UserID":20003,"EmployeeID":10005,"Roles":[10005],"Privileges":{"RoleID":0,"PrivilegeID":0,"RoleDesc":null,"PrivilegeName":null,"PrivilegeDesc":null,"PrivilegeIDs":[10002,10003,10006,10007,10004,10008,10013,20014,30013,30014,10016,10017,10015,10018,20013,20015,20016,20017,10010,10011,10005,10009,10012,10014,40015,40032,40018],"Id":0,"CreatedDate":"0001-01-01T00:00:00","ModifiedDate":null,"CreatedBy":null,"ModifiedBy":null,"Account":null},"Organization":null}}', N'SUCCESS')
SET IDENTITY_INSERT [dbo].[Log] OFF
SET IDENTITY_INSERT [dbo].[Menu] ON 

INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (1, N'Master Data', 0, N'#', 1, 1, 1, NULL, NULL, NULL, 0, N'fa fa-database')
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (2, N'Master Organization', 1, N'~/MasterData/OrganizationList', 1, 0, 1, N'VIEW_M_ORG', N'MasterData', N'OrganizationList', 1, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (3, N'ADD Master Organization', 2, NULL, 2, 0, 0, N'ADD_M_ORG', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (4, N'Edit Master Organization', 2, NULL, 3, 0, 0, N'EDIT_M_ORG', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (5, N'Delete Master Organization', 2, NULL, 4, 0, 0, N'DELETE_M_ORG', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (6, N'Master Privilege', 1, NULL, 2, 0, 1, N'VIEW_M_PRIV', N'MasterData', N'PrivilegeList', 1, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (7, N'ADD Master Privilege', 6, NULL, 1, 0, 0, N'ADD_M_PRIV', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (8, N'Edit Master Privilege', 6, NULL, 2, 0, 0, N'EDIT_M_PRIV', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (9, N'Delete Master Privilege', 6, NULL, 3, 0, 0, N'DELETE_M_PRIV', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (10, N'Master Role', 1, NULL, 3, 0, 1, N'VIEW_M_ROLE', N'MasterData', N'RoleList', 1, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (11, N'ADD Master Role', 10, NULL, 1, 0, 0, N'ADD_M_ROLE', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (12, N'Edit Master Role', 10, NULL, 2, 0, 0, N'EDIT_M_ROLE', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (13, N'Delete Master Role', 10, NULL, 3, 0, 0, N'DELETE_M_ROLE', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (14, N'Master User', 1, NULL, 4, 0, 1, N'VIEW_M_USER', N'MasterData', N'UserList', 1, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (16, N'Add Master User', 14, NULL, 1, 0, 0, N'ADD_M_USER', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (17, N'Edit Master User', 14, NULL, 2, 0, 0, N'EDIT_M_USER', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (18, N'Delete Master User', 14, NULL, 3, 0, 0, N'DELETE_M_USER', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (19, N'Master Employee', 1, NULL, 5, 0, 1, N'VIEW_M_EMPLOYEE', N'MasterData', N'EmployeeList', 1, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (20, N'Add Master Employee', 19, NULL, 1, 0, 0, N'ADD_M_EMPLOYEE', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (21, N'Edit Master Employee', 19, NULL, 2, 0, 0, N'EDIT_M_EMPLOYEE', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (22, N'Delete MasterEmployee', 19, NULL, 3, 0, 0, N'DELETE_M_EMPLOYEE', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (23, N'Administration', 0, N'#', 2, 1, 1, NULL, NULL, NULL, 0, N'fa fa-cog')
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (24, N'Logging', 23, NULL, 1, 0, 1, N'CommandLogging', N'Administration', N'Logging', 1, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (10011, N'Master Clinic', 1, NULL, 6, 0, 1, N'VIEW_M_CLINIC', N'MasterData', N'ClinicList', 1, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (10012, N'Add Master Clinic', 10011, NULL, 1, 0, 0, N'ADD_M_CLINIC', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (10013, N'Edit Master Clinic', 10011, NULL, 2, 0, 0, N'EDIT_M_CLINIC', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (10014, N'Delete Master Clinic', 10011, NULL, 3, 0, 0, N'DELETE_M_CLINIC', NULL, NULL, NULL, NULL)
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (20011, N'Home', 0, NULL, 0, 0, 1, N'Home', N'Home', N'Index', 0, N'fa fa-home')
INSERT [dbo].[Menu] ([Id], [Description], [ParentMenuId], [PageLink], [SortIndex], [HasChild], [IsMenu], [Name], [Controller], [Action], [Level], [icon]) VALUES (20012, N'Reset Password', 23, NULL, 2, 0, 1, N'RESET_PASSWORD', N'Administration', N'ResetPassword', 1, NULL)
SET IDENTITY_INSERT [dbo].[Menu] OFF
SET IDENTITY_INSERT [dbo].[Organization] ON 

INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'M008', N'PT BSI', 2, NULL, CAST(N'2019-02-10T13:10:17.533' AS DateTime), NULL, CAST(N'2019-02-10T13:16:58.910' AS DateTime))
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'M0014', N'Siloam Group', 1, NULL, CAST(N'2019-02-10T13:11:37.910' AS DateTime), NULL, NULL)
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'MTR001', N'Mitra Keluarga', 2, NULL, CAST(N'2019-02-10T13:13:50.213' AS DateTime), NULL, NULL)
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, N'SK8765', N'Sakura', 2, NULL, CAST(N'2019-02-10T17:04:35.047' AS DateTime), NULL, NULL)
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10005, N'HRM001', N'Hermina Hospital Group', 4, NULL, CAST(N'2019-03-11T12:23:14.623' AS DateTime), NULL, CAST(N'2019-03-11T12:44:46.537' AS DateTime))
SET IDENTITY_INSERT [dbo].[Organization] OFF
SET IDENTITY_INSERT [dbo].[OrganizationPrivilege] ON 

INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (14, 7, 10006)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (15, 7, 10010)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2051, 6, 10002)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2052, 6, 10003)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2053, 6, 10006)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2054, 6, 10007)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2055, 6, 10004)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2056, 6, 10008)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2057, 6, 10013)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2058, 6, 20014)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2059, 6, 30013)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2060, 6, 30014)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2061, 6, 10016)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2062, 6, 10017)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2063, 6, 10015)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2064, 6, 10018)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2065, 6, 20013)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2066, 6, 20015)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2067, 6, 20016)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2068, 6, 20017)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2069, 6, 10010)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2070, 6, 10011)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2071, 6, 10005)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2072, 6, 10009)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2073, 6, 10012)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2074, 6, 10014)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2075, 6, 40015)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2076, 6, 40018)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2077, 6, 40031)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (2078, 6, 40032)
SET IDENTITY_INSERT [dbo].[OrganizationPrivilege] OFF
SET IDENTITY_INSERT [dbo].[OrganizationRole] ON 

INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (1, 7, N'Admin')
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (3, 8, N'Super Admin')
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (4, 6, N'Staff')
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (5, 6, N'Unit Head')
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (6, 6, N'Sub Unit Head')
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (7, 6, N'Division Head')
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (10005, 6, N'Super Admin')
SET IDENTITY_INSERT [dbo].[OrganizationRole] OFF
SET IDENTITY_INSERT [dbo].[PasswordHistory] ON 

INSERT [dbo].[PasswordHistory] ([ID], [OrganizationID], [UserName], [Password]) VALUES (2, 0, N'adityawa', N'WZOe8Le+QdzXyE+E2Tqhxw==')
INSERT [dbo].[PasswordHistory] ([ID], [OrganizationID], [UserName], [Password]) VALUES (3, 0, N'adminsuper', N'lMdsjhhmPnQWj1fgjj4uJg==')
SET IDENTITY_INSERT [dbo].[PasswordHistory] OFF
SET IDENTITY_INSERT [dbo].[Privilege] ON 

INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10002, N'VIEW_M_ORG', N'View Master Organization', N'SYSTEM', CAST(N'2019-02-19T12:45:24.820' AS DateTime), NULL, CAST(N'2019-02-28T08:57:18.790' AS DateTime), 2)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10003, N'ADD_M_ORG', N'ADD Master Organization', N'SYSTEM', CAST(N'2019-02-19T12:45:41.287' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10004, N'EDIT_M_ORG', N'Edit Master Organization', N'SYSTEM', CAST(N'2019-02-19T12:48:45.103' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10005, N'DELETE_M_ORG', N'Delete Master Organization', N'SYSTEM', CAST(N'2019-02-19T12:49:15.407' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10006, N'VIEW_M_PRIVILEGE', N'View Master Privilege', N'SYSTEM', CAST(N'2019-02-19T12:50:01.217' AS DateTime), NULL, CAST(N'2019-02-28T09:00:43.857' AS DateTime), 6)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10007, N'ADD_M_PRIVILEGE', N'Add Master Privilege', N'SYSTEM', CAST(N'2019-02-19T12:50:29.153' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10008, N'EDIT_M_PRIVILEGE', N'Edit Master Privilege', N'SYSTEM', CAST(N'2019-02-19T12:52:03.537' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10009, N'DELETE_M_PRIVILEGE', N'Delete master Privilege', N'SYSTEM', CAST(N'2019-02-19T12:52:30.007' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10010, N'VIEW_M_ROLE', N'View Master Role', N'SYSTEM', CAST(N'2019-02-19T12:52:52.357' AS DateTime), NULL, CAST(N'2019-02-28T09:00:56.547' AS DateTime), 10)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10011, N'ADD_M_ROLE', N'ADD Master Role', N'SYSTEM', CAST(N'2019-02-19T12:53:36.073' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10012, N'EDIT_M_ROLE', N'Edit Master Role', N'SYSTEM', CAST(N'2019-02-19T12:53:51.943' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10013, N'Login_Access', N'Accces Login', N'SYSTEM', CAST(N'2019-02-23T20:32:19.467' AS DateTime), NULL, CAST(N'2019-02-28T12:21:03.037' AS DateTime), 20011)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10014, N'DELETE_M_ROLE', N'Delete Master Role', N'SYSTEM', CAST(N'2019-02-23T20:32:59.560' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10015, N'VIEW_M_EMPLOYEE', N'View Master Employee', N'SYSTEM', CAST(N'2019-02-23T20:33:25.070' AS DateTime), NULL, CAST(N'2019-02-28T09:01:26.557' AS DateTime), 19)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10016, N'ADD_M_EMPLOYEE', N'Add data Master Employee', N'SYSTEM', CAST(N'2019-02-23T20:33:46.170' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10017, N'EDIT_M_EMPLOYEE', N'Edit Master Employee', N'SYSTEM', CAST(N'2019-02-23T20:34:06.163' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (10018, N'DELETE_M_EMPLOYEE', N'Delete Master Employee', N'SYSTEM', CAST(N'2019-02-23T20:34:22.120' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (20013, N'VIEW_M_USER', N'View Master User', N'SYSTEM', CAST(N'2019-02-28T09:02:26.540' AS DateTime), NULL, NULL, 14)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (20014, N'VIEW_M_DATA', N'View Master Data', N'SYSTEM', CAST(N'2019-02-28T09:02:56.823' AS DateTime), NULL, NULL, 1)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (20015, N'ADD_M_USER', N'Add Master User', N'SYSTEM', CAST(N'2019-02-28T09:03:51.507' AS DateTime), NULL, NULL, 14)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (20016, N'EDIT_M_USER', N'Edit Master User', N'SYSTEM', CAST(N'2019-02-28T09:04:02.903' AS DateTime), NULL, NULL, 14)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (20017, N'DELETE_M_USER', N'Delete Master User', N'SYSTEM', CAST(N'2019-02-28T09:04:16.667' AS DateTime), NULL, NULL, 14)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (30013, N'VIEW_ADM', N'Administration Menu', N'SYSTEM', CAST(N'2019-03-02T06:56:56.873' AS DateTime), NULL, NULL, 23)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (30014, N'VIEW_LOG', N'View Logging Page', N'SYSTEM', CAST(N'2019-03-02T06:57:57.573' AS DateTime), NULL, NULL, 24)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (40015, N'VIEW_M_CLINIC', N'View Master Clinic', N'SYSTEM', CAST(N'2019-03-10T10:42:48.143' AS DateTime), NULL, NULL, 10011)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (40018, N'ADD_M_CLINIC', N'Add Master Clinic', N'SYSTEM', CAST(N'2019-03-10T10:45:01.410' AS DateTime), NULL, NULL, 10011)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (40031, N'EDIT_M_CLINIC', N'Edit Master Clinic', N'SYSTEM', CAST(N'2019-03-10T19:30:19.987' AS DateTime), NULL, NULL, 10011)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [MenuID]) VALUES (40032, N'DELETE_M_CLINIC', N'Delete Master Clinic', N'SYSTEM', CAST(N'2019-03-10T19:35:09.640' AS DateTime), NULL, NULL, 10011)
SET IDENTITY_INSERT [dbo].[Privilege] OFF
SET IDENTITY_INSERT [dbo].[RolePrivilege] ON 

INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (8, 1, 10006, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (9, 1, 10010, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10002, 4, 10006, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10003, 4, 10002, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10004, 6, 20014, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10005, 6, 10013, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10078, 5, 10015, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10079, 5, 20013, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10080, 5, 10010, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10081, 5, 10002, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10082, 5, 10006, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10083, 5, 20014, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20132, 10005, 10002, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20133, 10005, 10003, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20134, 10005, 10006, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20135, 10005, 10007, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20136, 10005, 10004, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20137, 10005, 10008, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20138, 10005, 10013, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20139, 10005, 20014, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20140, 10005, 30013, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20141, 10005, 30014, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20142, 10005, 10016, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20143, 10005, 10017, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20144, 10005, 10015, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20145, 10005, 10018, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20146, 10005, 20013, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20147, 10005, 20015, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20148, 10005, 20016, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20149, 10005, 20017, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20150, 10005, 10010, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20151, 10005, 10011, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20152, 10005, 10005, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20153, 10005, 10009, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20154, 10005, 10012, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20155, 10005, 10014, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20156, 10005, 40015, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20157, 10005, 40032, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (20158, 10005, 40018, NULL)
SET IDENTITY_INSERT [dbo].[RolePrivilege] OFF
INSERT [dbo].[User] ([ID], [OrganizationID], [UserName], [Password], [EmployeeID], [ExpiredDate], [Status], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ResetPasswordCode]) VALUES (10002, 7, N'darisandy', N'WZOe8Le+QdzXyE+E2Tqhxw==', 3, CAST(N'2019-04-30T00:00:00.000' AS DateTime), 1, N'SYSTEM', CAST(N'2019-02-24T21:14:47.583' AS DateTime), NULL, CAST(N'2019-02-24T21:33:31.453' AS DateTime), NULL)
INSERT [dbo].[User] ([ID], [OrganizationID], [UserName], [Password], [EmployeeID], [ExpiredDate], [Status], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ResetPasswordCode]) VALUES (10003, 6, N'adityawa', N'Hs7q+xw62FKmqZdRsdP1Uw==', 1, CAST(N'2019-05-31T00:00:00.000' AS DateTime), 1, N'SYSTEM', CAST(N'2019-02-24T21:15:33.527' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[User] ([ID], [OrganizationID], [UserName], [Password], [EmployeeID], [ExpiredDate], [Status], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ResetPasswordCode]) VALUES (20002, 6, N'bsusanto', N'WZOe8Le+QdzXyE+E2Tqhxw==', 2, CAST(N'2019-04-30T00:00:00.000' AS DateTime), 1, N'SYSTEM', CAST(N'2019-03-02T07:14:13.530' AS DateTime), NULL, NULL, NULL)
INSERT [dbo].[User] ([ID], [OrganizationID], [UserName], [Password], [EmployeeID], [ExpiredDate], [Status], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ResetPasswordCode]) VALUES (20003, 6, N'adminsuper', N'am/LlPheNX8sk/vj6o8zsA==', 10005, CAST(N'2029-12-31T00:00:00.000' AS DateTime), 1, N'SYSTEM', CAST(N'2019-03-02T07:32:38.017' AS DateTime), NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[UserRole] ON 

INSERT [dbo].[UserRole] ([Id], [UserID], [RoleID]) VALUES (3, 10003, 4)
INSERT [dbo].[UserRole] ([Id], [UserID], [RoleID]) VALUES (4, 10003, 5)
INSERT [dbo].[UserRole] ([Id], [UserID], [RoleID]) VALUES (10002, 20003, 10005)
SET IDENTITY_INSERT [dbo].[UserRole] OFF
ALTER TABLE [dbo].[Clinic]  WITH CHECK ADD  CONSTRAINT [FK_Clinic_Clinic] FOREIGN KEY([CityID])
REFERENCES [dbo].[GeneralMaster] ([Id])
GO
ALTER TABLE [dbo].[Clinic] CHECK CONSTRAINT [FK_Clinic_Clinic]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_GeneralMaster] FOREIGN KEY([EmpType])
REFERENCES [dbo].[GeneralMaster] ([Id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_GeneralMaster]
GO
ALTER TABLE [dbo].[Organization]  WITH CHECK ADD  CONSTRAINT [FK_Organization_Clinic] FOREIGN KEY([KlinikID])
REFERENCES [dbo].[Clinic] ([ID])
GO
ALTER TABLE [dbo].[Organization] CHECK CONSTRAINT [FK_Organization_Clinic]
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
ALTER TABLE [dbo].[Privilege]  WITH CHECK ADD  CONSTRAINT [FK_Privilege_Menu] FOREIGN KEY([MenuID])
REFERENCES [dbo].[Menu] ([Id])
GO
ALTER TABLE [dbo].[Privilege] CHECK CONSTRAINT [FK_Privilege_Menu]
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
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Employee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Employee]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Organization] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([ID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Organization]
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
USE [master]
GO
ALTER DATABASE [KlinikDB] SET  READ_WRITE 
GO
