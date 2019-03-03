USE [master]
GO
/****** Object:  Database [KlinikDB]    Script Date: 2/23/2019 11:00:02 PM ******/
CREATE DATABASE [KlinikDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'KlinikDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER2014\MSSQL\DATA\KlinikDB.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'KlinikDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER2014\MSSQL\DATA\KlinikDB_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
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
USE [KlinikDB]
GO
/****** Object:  Table [dbo].[Clinic]    Script Date: 2/23/2019 11:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 2/23/2019 11:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GeneralMaster]    Script Date: 2/23/2019 11:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Log]    Script Date: 2/23/2019 11:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Start] [datetime] NOT NULL,
	[Module] [varchar](50) NOT NULL,
	[Account] [bigint] NOT NULL,
	[Command] [text] NULL,
	[OldValue] [nvarchar](500) NULL,
	[NewValue] [nvarchar](500) NULL,
	[Status] [varchar](50) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 2/23/2019 11:00:03 PM ******/
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
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Organization]    Script Date: 2/23/2019 11:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrganizationPrivilege]    Script Date: 2/23/2019 11:00:03 PM ******/
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
/****** Object:  Table [dbo].[OrganizationRole]    Script Date: 2/23/2019 11:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PasswordHistory]    Script Date: 2/23/2019 11:00:03 PM ******/
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
/****** Object:  Table [dbo].[Privilege]    Script Date: 2/23/2019 11:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Privilege](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Privilege_Name] [nvarchar](150) NOT NULL,
	[Privilege_Desc] [nvarchar](500) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Privilege] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RolePrivilege]    Script Date: 2/23/2019 11:00:03 PM ******/
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
/****** Object:  Table [dbo].[User]    Script Date: 2/23/2019 11:00:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
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
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 2/23/2019 11:00:03 PM ******/
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
INSERT [dbo].[Clinic] ([ID], [Code], [Name], [Address], [LegalNumber], [LegalDate], [ContactNumber], [Email], [Lat], [Long], [CityID], [ClinicType], [ReffID], [RowStatus], [CreatedBy], [DateCreated], [LastUpdateBy], [DateModified]) VALUES (2, N'K0002', N'Klinik Gajebo', N'Jakarta', N'877654', CAST(N'2018-12-31' AS Date), N'38778438432', N'klinik.gajebo@gmail.com', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Clinic] OFF
SET IDENTITY_INSERT [dbo].[Employee] ON 

INSERT [dbo].[Employee] ([id], [EmpID], [EmpName], [BirthDate], [Gender], [EmpType], [EmpDept], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Email]) VALUES (1, N'A00057', N'Aditya Wahyu B', CAST(N'1988-03-02' AS Date), N'L', 1, 4, NULL, NULL, NULL, NULL, NULL, N'adityawa@outlook.com')
INSERT [dbo].[Employee] ([id], [EmpID], [EmpName], [BirthDate], [Gender], [EmpType], [EmpDept], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Email]) VALUES (2, N'B007892', N'Beny Susanto', CAST(N'1985-01-01' AS Date), N'L', 1, 4, NULL, NULL, NULL, NULL, NULL, N'benny.susanto@bsi.co.id')
INSERT [dbo].[Employee] ([id], [EmpID], [EmpName], [BirthDate], [Gender], [EmpType], [EmpDept], [RowStatus], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Email]) VALUES (3, N'D00012', N'Dwi Arisandy', CAST(N'1982-01-03' AS Date), N'F', 3, 7, NULL, N'SYSTEM', CAST(N'2019-02-19 12:31:04.063' AS DateTime), N'SYSTEM', CAST(N'2019-02-19 12:39:54.780' AS DateTime), N'darisandy')
SET IDENTITY_INSERT [dbo].[Employee] OFF
SET IDENTITY_INSERT [dbo].[GeneralMaster] ON 

INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (1, N'EmploymentType', N'Permanent', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (2, N'EmploymentType', N'Outsource', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (3, N'EmploymentType', N'PKWT', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (4, N'Department', N'ISA', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (5, N'Department', N'Sales', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (6, N'Department', N'ERP', NULL)
INSERT [dbo].[GeneralMaster] ([Id], [Type], [Name], [Value]) VALUES (7, N'Department', N'HRD', NULL)
SET IDENTITY_INSERT [dbo].[GeneralMaster] OFF
SET IDENTITY_INSERT [dbo].[Organization] ON 

INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'M008', N'PT BSI', 2, NULL, CAST(N'2019-02-10 13:10:17.533' AS DateTime), NULL, CAST(N'2019-02-10 13:16:58.910' AS DateTime))
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'M0014', N'Siloam Group', 1, NULL, CAST(N'2019-02-10 13:11:37.910' AS DateTime), NULL, NULL)
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'MTR001', N'Mitra Keluarga', 2, NULL, CAST(N'2019-02-10 13:13:50.213' AS DateTime), NULL, NULL)
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, N'AD1766', N'Graci', 1, NULL, CAST(N'2019-02-10 17:02:27.553' AS DateTime), NULL, NULL)
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, N'SK8765', N'Sakura', 2, NULL, CAST(N'2019-02-10 17:04:35.047' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Organization] OFF
SET IDENTITY_INSERT [dbo].[OrganizationPrivilege] ON 

INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (14, 7, 10006)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (15, 7, 10010)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (16, 6, 10002)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (17, 6, 10003)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (18, 6, 10006)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (19, 6, 10007)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (20, 6, 10004)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (21, 6, 10008)
INSERT [dbo].[OrganizationPrivilege] ([ID], [OrgID], [PrivilegeID]) VALUES (22, 6, 10013)
SET IDENTITY_INSERT [dbo].[OrganizationPrivilege] OFF
SET IDENTITY_INSERT [dbo].[OrganizationRole] ON 

INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (1, 7, N'Admin')
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (3, 8, N'Super Admin')
INSERT [dbo].[OrganizationRole] ([ID], [OrgID], [RoleName]) VALUES (4, 6, N'Manager')
SET IDENTITY_INSERT [dbo].[OrganizationRole] OFF
SET IDENTITY_INSERT [dbo].[Privilege] ON 

INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'View Master', N'View All Master', N'SYSTEM', CAST(N'2019-02-10 21:21:37.337' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10002, N'VIEW_M_ORG', N'View Master Organization', N'SYSTEM', CAST(N'2019-02-19 12:45:24.820' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10003, N'ADD_M_ORG', N'ADD Master Organization', N'SYSTEM', CAST(N'2019-02-19 12:45:41.287' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10004, N'EDIT_M_ORG', N'Edit Master Organization', N'SYSTEM', CAST(N'2019-02-19 12:48:45.103' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10005, N'DELETE_M_ORG', N'Delete Master Organization', N'SYSTEM', CAST(N'2019-02-19 12:49:15.407' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10006, N'VIEW_M_PRIVILEGE', N'View Master Privilege', N'SYSTEM', CAST(N'2019-02-19 12:50:01.217' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10007, N'ADD_M_PRIVILEGE', N'Add Master Privilege', N'SYSTEM', CAST(N'2019-02-19 12:50:29.153' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10008, N'EDIT_M_PRIVILEGE', N'Edit Master Privilege', N'SYSTEM', CAST(N'2019-02-19 12:52:03.537' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10009, N'DELETE_M_PRIVILEGE', N'Delete master Privilege', N'SYSTEM', CAST(N'2019-02-19 12:52:30.007' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10010, N'VIEW_M_ROLE', N'View Master Role', N'SYSTEM', CAST(N'2019-02-19 12:52:52.357' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10011, N'ADD_M_ROLE', N'ADD Master Role', N'SYSTEM', CAST(N'2019-02-19 12:53:36.073' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10012, N'EDIT_M_ROLE', N'Edit Master Role', N'SYSTEM', CAST(N'2019-02-19 12:53:51.943' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10013, N'Login_Access', N'Accces Login', N'SYSTEM', CAST(N'2019-02-23 20:32:19.467' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10014, N'DELETE_M_ROLE', N'Delete Master Role', N'SYSTEM', CAST(N'2019-02-23 20:32:59.560' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10015, N'VIEW_M_EMPLOYEE', N'View Master Employee', N'SYSTEM', CAST(N'2019-02-23 20:33:25.070' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10016, N'ADD_M_EMPLOYEE', N'Add data Master Employee', N'SYSTEM', CAST(N'2019-02-23 20:33:46.170' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10017, N'EDIT_M_EMPLOYEE', N'Edit Master Employee', N'SYSTEM', CAST(N'2019-02-23 20:34:06.163' AS DateTime), NULL, NULL)
INSERT [dbo].[Privilege] ([ID], [Privilege_Name], [Privilege_Desc], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10018, N'DELETE_M_EMPLOYEE', N'Delete Master Employee', N'SYSTEM', CAST(N'2019-02-23 20:34:22.120' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Privilege] OFF
SET IDENTITY_INSERT [dbo].[RolePrivilege] ON 

INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (8, 1, 10006, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (9, 1, 10010, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (10, 4, 10006, NULL)
INSERT [dbo].[RolePrivilege] ([ID], [RoleID], [PrivilegeID], [MenuId]) VALUES (11, 4, 10013, NULL)
SET IDENTITY_INSERT [dbo].[RolePrivilege] OFF
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([ID], [OrganizationID], [UserName], [Password], [EmployeeID], [ExpiredDate], [Status], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, 6, N'bsusanto', N'u9IjyrsvQZrh+hPzWBWRkQ==', 2, CAST(N'2019-05-31 00:00:00.000' AS DateTime), 1, N'SYSTEM', CAST(N'2019-02-16 00:09:47.773' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[User] OFF
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
