USE [master]
GO
/****** Object:  Database [KlinikDB]    Script Date: 2/10/2019 1:18:53 PM ******/
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
/****** Object:  Table [dbo].[Clinic]    Script Date: 2/10/2019 1:18:53 PM ******/
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
/****** Object:  Table [dbo].[Log]    Script Date: 2/10/2019 1:18:53 PM ******/
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
/****** Object:  Table [dbo].[Organization]    Script Date: 2/10/2019 1:18:53 PM ******/
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
/****** Object:  Table [dbo].[OrganizationPrivilege]    Script Date: 2/10/2019 1:18:53 PM ******/
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
/****** Object:  Table [dbo].[OrganizationRole]    Script Date: 2/10/2019 1:18:53 PM ******/
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
/****** Object:  Table [dbo].[PasswordHistory]    Script Date: 2/10/2019 1:18:53 PM ******/
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
/****** Object:  Table [dbo].[Privilege]    Script Date: 2/10/2019 1:18:53 PM ******/
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
/****** Object:  Table [dbo].[RolePrivilege]    Script Date: 2/10/2019 1:18:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePrivilege](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleID] [bigint] NOT NULL,
	[PrivilegeID] [bigint] NOT NULL,
 CONSTRAINT [PK_RolePrivilege] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 2/10/2019 1:18:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrganizationID] [bigint] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
	[EmployeeID] [nvarchar](20) NOT NULL,
	[ExpiredDate] [datetime] NULL,
	[Email] [nvarchar](50) NULL,
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
/****** Object:  Table [dbo].[UserRole]    Script Date: 2/10/2019 1:18:53 PM ******/
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
SET IDENTITY_INSERT [dbo].[Organization] ON 

INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, N'M008', N'PT BSI', 2, NULL, CAST(N'2019-02-10 13:10:17.533' AS DateTime), NULL, CAST(N'2019-02-10 13:16:58.910' AS DateTime))
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, N'M0014', N'Siloam Group', 1, NULL, CAST(N'2019-02-10 13:11:37.910' AS DateTime), NULL, NULL)
INSERT [dbo].[Organization] ([ID], [OrgCode], [OrgName], [KlinikID], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, N'MTR001', N'Mitra Keluarga', 2, NULL, CAST(N'2019-02-10 13:13:50.213' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Organization] OFF
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
