USE [master]
GO
/****** Object:  Database [BDMPro]    Script Date: 3/13/2024 11:01:00 PM ******/
CREATE DATABASE [BDMPro]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BDMPro', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.HIEUDAT\MSSQL\DATA\BDMPro.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BDMPro_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.HIEUDAT\MSSQL\DATA\BDMPro_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BDMPro].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BDMPro] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BDMPro] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BDMPro] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BDMPro] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BDMPro] SET ARITHABORT OFF 
GO
ALTER DATABASE [BDMPro] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BDMPro] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BDMPro] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BDMPro] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BDMPro] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BDMPro] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BDMPro] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BDMPro] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BDMPro] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BDMPro] SET  ENABLE_BROKER 
GO
ALTER DATABASE [BDMPro] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BDMPro] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BDMPro] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BDMPro] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BDMPro] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BDMPro] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BDMPro] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BDMPro] SET RECOVERY FULL 
GO
ALTER DATABASE [BDMPro] SET  MULTI_USER 
GO
ALTER DATABASE [BDMPro] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BDMPro] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BDMPro] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BDMPro] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [BDMPro] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [BDMPro] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'BDMPro', N'ON'
GO
ALTER DATABASE [BDMPro] SET QUERY_STORE = ON
GO
ALTER DATABASE [BDMPro] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [BDMPro]
GO
/****** Object:  Table [dbo].[ActivationKey]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivationKey](
	[ActivationKeyId] [nvarchar](128) NOT NULL,
	[ActivationKeyName] [nvarchar](255) NOT NULL,
	[SoftwareId] [nvarchar](128) NULL,
	[Category] [nvarchar](100) NULL,
	[Quantity] [int] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ActivationKeyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime2](7) NULL,
	[SystemDefault] [bit] NOT NULL,
	[IsoUtcCreatedOn] [nvarchar](max) NULL,
	[IsoUtcModifiedOn] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[LockoutEndDateUtc] [datetime2](7) NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfigTemplate]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigTemplate](
	[ConfigId] [nvarchar](128) NOT NULL,
	[CpuName] [nvarchar](255) NULL,
	[CpuSpeed] [float] NULL,
	[RamMemory] [int] NULL,
	[RamType] [nvarchar](255) NULL,
	[RamSpeed] [int] NULL,
	[SsdMemory] [int] NULL,
	[SsdType] [nvarchar](255) NULL,
	[SsdSpeed] [int] NULL,
	[HddMemory] [int] NULL,
	[HddType] [nvarchar](255) NULL,
	[HddSpeed] [int] NULL,
	[Cd] [nvarchar](255) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
PRIMARY KEY CLUSTERED 
(
	[ConfigId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contact]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact](
	[ContactId] [nvarchar](128) NOT NULL,
	[ContactName] [nvarchar](255) NOT NULL,
	[Email] [varchar](255) NULL,
	[Phone] [varchar](15) NULL,
	[Notes] [nvarchar](1000) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Device]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Device](
	[DeviceId] [nvarchar](128) NOT NULL,
	[DeviceTypeId] [nvarchar](128) NOT NULL,
	[DeviceCode] [varchar](9) NOT NULL,
	[DeviceName] [nvarchar](255) NOT NULL,
	[BrandName] [nvarchar](255) NULL,
	[Model] [nvarchar](255) NULL,
	[SerialNumber] [varchar](50) NULL,
	[IpAddress] [varchar](50) NULL,
	[MacAddress] [varchar](50) NULL,
	[CpuName] [nvarchar](255) NULL,
	[CpuSpeed] [float] NULL,
	[RamMemory] [int] NULL,
	[RamType] [nvarchar](255) NULL,
	[RamSpeed] [int] NULL,
	[SsdMemory] [int] NULL,
	[SsdType] [nvarchar](255) NULL,
	[SsdSpeed] [int] NULL,
	[HddMemory] [int] NULL,
	[HddType] [nvarchar](255) NULL,
	[HddSpeed] [int] NULL,
	[Cd] [nvarchar](255) NULL,
	[SupplierId] [nvarchar](128) NULL,
	[PurchaseDate] [date] NOT NULL,
	[WarrantyType] [nvarchar](255) NULL,
	[WarrantyDate] [date] NULL,
	[WarrantyEndDate] [date] NULL,
	[Location] [nvarchar](255) NULL,
	[FirstHandoverDate] [date] NULL,
	[Age] [float] NULL,
	[Notes] [nvarchar](1000) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsActive] [bit] NULL,
	[IsActiveRepair] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DeviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceSoftware]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceSoftware](
	[DeviceSoftwareId] [nvarchar](128) NOT NULL,
	[SoftwareId] [nvarchar](128) NOT NULL,
	[DeviceId] [nvarchar](128) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DeviceSoftwareId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceType]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceType](
	[DeviceTypeId] [nvarchar](128) NOT NULL,
	[TypeName] [nvarchar](255) NOT NULL,
	[TypeSymbol] [varchar](5) NOT NULL,
	[Notes] [nvarchar](1000) NULL,
	[OrderCode] [nvarchar](128) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DeviceTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DraftCode]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DraftCode](
	[DraftCodeId] [nvarchar](128) NOT NULL,
	[DeviceTypeId] [nvarchar](128) NULL,
	[DeviceCode] [nvarchar](128) NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DraftCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailTemplate](
	[Id] [nvarchar](128) NOT NULL,
	[Subject] [nvarchar](max) NULL,
	[Body] [nvarchar](max) NULL,
	[Type] [nvarchar](256) NULL,
 CONSTRAINT [PK_dbo.EmailTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorLog]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLog](
	[Id] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](450) NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[ErrorDetails] [nvarchar](max) NULL,
	[ErrorDate] [datetime] NULL,
 CONSTRAINT [PK_dbo.ErrorLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GlobalOptionSet]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GlobalOptionSet](
	[Id] [nvarchar](128) NOT NULL,
	[Code] [nvarchar](256) NULL,
	[DisplayName] [nvarchar](256) NULL,
	[Type] [nvarchar](256) NULL,
	[Status] [nvarchar](256) NULL,
	[OptionOrder] [int] NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[SystemDefault] [bit] NOT NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.GlobalOptionSet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoginHistory]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoginHistory](
	[Id] [nvarchar](128) NOT NULL,
	[AspNetUserId] [nvarchar](450) NULL,
	[LoginDateTime] [datetime] NULL,
	[IsoUtcLoginDateTime] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.LoginHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Module]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Module](
	[Id] [nvarchar](128) NOT NULL,
	[Code] [nvarchar](256) NULL,
	[Name] [nvarchar](256) NULL,
	[MainUrl] [nvarchar](max) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.Module] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RepairDetail]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RepairDetail](
	[RepairDetailId] [nvarchar](128) NOT NULL,
	[DeviceId] [nvarchar](128) NOT NULL,
	[ErrorCondition] [nvarchar](1000) NULL,
	[RepairTypeId] [nvarchar](128) NOT NULL,
	[ReceptionDate] [date] NULL,
	[RepairStartDate] [date] NULL,
	[Handover] [date] NULL,
	[CancellationInfo] [nvarchar](1000) NULL,
	[CancellationDate] [date] NULL,
	[Notes] [nvarchar](1000) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RepairDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RepairType]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RepairType](
	[RepairTypeId] [nvarchar](128) NOT NULL,
	[RepairTypeName] [nvarchar](255) NULL,
	[Notes] [nvarchar](1000) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RepairTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleModulePermission]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleModulePermission](
	[Id] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
	[ModuleId] [nvarchar](128) NOT NULL,
	[ViewRight] [bit] NOT NULL,
	[AddRight] [bit] NOT NULL,
	[EditRight] [bit] NOT NULL,
	[DeleteRight] [bit] NOT NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.RoleModulePermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Software]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Software](
	[SoftwareId] [nvarchar](128) NOT NULL,
	[SoftwareName] [nvarchar](255) NOT NULL,
	[Version] [nvarchar](100) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SoftwareId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SoftwareActivation]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SoftwareActivation](
	[ActivationKeyId] [nvarchar](128) NOT NULL,
	[SoftwareId] [nvarchar](128) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[SupplierId] [nvarchar](128) NOT NULL,
	[SupplierName] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [varchar](20) NULL,
	[Address] [nvarchar](1000) NULL,
	[ContactId] [nvarchar](128) NULL,
	[Notes] [nvarchar](1000) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAttachment]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAttachment](
	[Id] [nvarchar](128) NOT NULL,
	[UserProfileId] [nvarchar](128) NULL,
	[FileUrl] [nvarchar](max) NULL,
	[FileName] [nvarchar](256) NULL,
	[UniqueFileName] [nvarchar](256) NULL,
	[AttachmentTypeId] [nvarchar](128) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.UserAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 3/13/2024 11:01:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[Id] [nvarchar](128) NOT NULL,
	[AspNetUserId] [nvarchar](450) NULL,
	[FirstName] [nvarchar](256) NULL,
	[LastName] [nvarchar](256) NULL,
	[FullName] [nvarchar](256) NULL,
	[IDCardNumber] [nvarchar](256) NULL,
	[DateOfBirth] [datetime] NULL,
	[GenderId] [nvarchar](128) NULL,
	[CountryName] [nvarchar](256) NULL,
	[Address] [nvarchar](max) NULL,
	[PostalCode] [nvarchar](128) NULL,
	[PhoneNumber] [nvarchar](256) NULL,
	[UserStatusId] [nvarchar](128) NULL,
	[CreatedBy] [nvarchar](128) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](128) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsoUtcDateOfBirth] [nvarchar](128) NULL,
	[IsoUtcCreatedOn] [nvarchar](128) NULL,
	[IsoUtcModifiedOn] [nvarchar](128) NULL,
 CONSTRAINT [PK_dbo.UserProfile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn], [ConcurrencyStamp]) VALUES (N'5AEC85B2-54FD-4B1E-81C1-2D158E29DDE3', N'System Admin', N'SYSTEM ADMIN', NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn], [ConcurrencyStamp]) VALUES (N'7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D', N'Normal User', N'NORMAL USER', NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn], [ConcurrencyStamp]) VALUES (N'9E8DBCEC-E5E3-4A89-BC4F-6853C1D2C924', N'Maintenance Department', N'MAINTENANCE DEPARTMENT', NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn], [ConcurrencyStamp]) VALUES (N'DCF4F5BC-D72C-453B-AC68-4CC7583F93B5', N'User Admin', N'USER ADMIN', NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Country] ON 

INSERT [dbo].[Country] ([Id], [Name]) VALUES (1, N'Afghanistan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (2, N'Albania')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (3, N'Algeria')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (4, N'Andorra')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (5, N'Angola')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (6, N'Antigua and Barbuda')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (7, N'Argentina')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (8, N'Armenia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (9, N'Australia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (10, N'Austria')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (11, N'Azerbaijan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (12, N'Bahamas')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (13, N'Bahrain')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (14, N'Bangladesh')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (15, N'Barbados')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (16, N'Belarus')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (17, N'Belgium')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (18, N'Belize')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (19, N'Benin')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (20, N'Bhutan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (21, N'Bolivia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (22, N'Bosnia and Herzegovina')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (23, N'Botswana')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (24, N'Brazil')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (25, N'Brunei')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (26, N'Bulgaria')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (27, N'Burkina Faso')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (28, N'Burundi')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (29, N'Cabo Verde')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (30, N'Cambodia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (31, N'Cameroon')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (32, N'Canada')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (33, N'Central African Republic')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (34, N'Chad')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (35, N'Channel Islands')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (36, N'Chile')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (37, N'China')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (38, N'Colombia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (39, N'Comoros')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (40, N'Congo')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (41, N'Costa Rica')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (42, N'Côte d''Ivoire')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (43, N'Croatia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (44, N'Cuba')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (45, N'Cyprus')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (46, N'Czech Republic')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (47, N'Denmark')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (48, N'Djibouti')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (49, N'Dominica')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (50, N'Dominican Republic')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (51, N'DR Congo')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (52, N'Ecuador')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (53, N'Egypt')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (54, N'El Salvador')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (55, N'Equatorial Guinea')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (56, N'Eritrea')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (57, N'Estonia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (58, N'Eswatini')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (59, N'Ethiopia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (60, N'Faeroe Islands')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (61, N'Fiji')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (62, N'Finland')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (63, N'France')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (64, N'French Guiana')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (65, N'Gabon')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (66, N'Gambia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (67, N'Georgia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (68, N'Germany')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (69, N'Ghana')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (70, N'Gibraltar')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (71, N'Greece')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (72, N'Greenland')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (73, N'Grenada')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (74, N'Guadeloupe')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (75, N'Guatemala')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (76, N'Guinea')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (77, N'Guinea-Bissau')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (78, N'Guyana')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (79, N'Haiti')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (80, N'Holy See')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (81, N'Honduras')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (82, N'Hong Kong')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (83, N'Hungary')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (84, N'Iceland')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (85, N'India')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (86, N'Indonesia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (87, N'Iran')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (88, N'Iraq')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (89, N'Ireland')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (90, N'Isle of Man')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (91, N'Israel')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (92, N'Italy')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (93, N'Jamaica')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (94, N'Japan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (95, N'Jordan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (96, N'Kazakhstan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (97, N'Kenya')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (98, N'Kiribati')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (99, N'Kuwait')
GO
INSERT [dbo].[Country] ([Id], [Name]) VALUES (100, N'Kyrgyzstan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (101, N'Laos')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (102, N'Latvia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (103, N'Lebanon')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (104, N'Lesotho')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (105, N'Liberia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (106, N'Libya')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (107, N'Liechtenstein')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (108, N'Lithuania')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (109, N'Luxembourg')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (110, N'Macao')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (111, N'Madagascar')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (112, N'Malawi')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (113, N'Malaysia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (114, N'Maldives')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (115, N'Mali')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (116, N'Malta')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (117, N'Marshall Islands')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (118, N'Martinique')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (119, N'Mauritania')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (120, N'Mauritius')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (121, N'Mayotte')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (122, N'Mexico')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (123, N'Micronesia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (124, N'Moldova')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (125, N'Monaco')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (126, N'Mongolia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (127, N'Montenegro')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (128, N'Morocco')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (129, N'Mozambique')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (130, N'Myanmar')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (131, N'Namibia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (132, N'Nauru')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (133, N'Nepal')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (134, N'Netherlands')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (135, N'New Caledonia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (136, N'New Zealand')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (137, N'Nicaragua')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (138, N'Niger')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (139, N'Nigeria')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (140, N'North Korea')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (141, N'North Macedonia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (142, N'Norway')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (143, N'Oman')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (144, N'Pakistan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (145, N'Palau')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (146, N'Panama')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (147, N'Papua New Guinea')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (148, N'Paraguay')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (149, N'Peru')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (150, N'Philippines')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (151, N'Poland')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (152, N'Portugal')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (153, N'Qatar')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (154, N'Réunion')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (155, N'Romania')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (156, N'Russia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (157, N'Rwanda')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (158, N'Saint Helena')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (159, N'Saint Kitts and Nevis')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (160, N'Saint Lucia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (161, N'Saint Vincent and the Grenadines')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (162, N'Samoa')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (163, N'San Marino')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (164, N'Sao Tome & Principe')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (165, N'Saudi Arabia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (166, N'Senegal')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (167, N'Serbia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (168, N'Seychelles')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (169, N'Sierra Leone')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (170, N'Singapore')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (171, N'Slovakia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (172, N'Slovenia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (173, N'Solomon Islands')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (174, N'Somalia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (175, N'South Africa')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (176, N'South Korea')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (177, N'South Sudan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (178, N'Spain')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (179, N'Sri Lanka')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (180, N'State of Palestine')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (181, N'Sudan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (182, N'Suriname')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (183, N'Sweden')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (184, N'Switzerland')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (185, N'Syria')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (186, N'Taiwan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (187, N'Tajikistan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (188, N'Tanzania')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (189, N'Thailand')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (190, N'Timor-Leste')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (191, N'Togo')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (192, N'Tonga')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (193, N'Trinidad and Tobago')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (194, N'Tunisia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (195, N'Turkey')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (196, N'Turkmenistan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (197, N'Tuvalu')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (198, N'Uganda')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (199, N'Ukraine')
GO
INSERT [dbo].[Country] ([Id], [Name]) VALUES (200, N'United Arab Emirates')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (201, N'United Kingdom')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (202, N'United States')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (203, N'Uruguay')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (204, N'Uzbekistan')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (205, N'Vanuatu')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (206, N'Venezuela')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (207, N'Vietnam')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (208, N'Western Sahara')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (209, N'Yemen')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (210, N'Zambia')
INSERT [dbo].[Country] ([Id], [Name]) VALUES (211, N'Zimbabwe')
SET IDENTITY_INSERT [dbo].[Country] OFF
GO
INSERT [dbo].[EmailTemplate] ([Id], [Subject], [Body], [Type]) VALUES (N'27D8409F-502D-4A01-8DA1-8D756EA00D0C', N'Password Reset For [WebsiteName] Account', N'<p>Hi [Username],<br><br>There was a request to reset your password on [WebsiteName].</p><p><a href="[Url]">Click Here</a> and follow the instructions to reset your password. Thank You.</p><p></p><p>If you did not make this request then please ignore this email.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>[WebsiteName]</p>', N'ForgotPassword')
INSERT [dbo].[EmailTemplate] ([Id], [Subject], [Body], [Type]) VALUES (N'37F6A753-2F8A-4808-AEDF-3512B474DA15', N'Confirm Your Email To Complete [WebsiteName] Account Registration', N'<p>Hi [Username],<br><br>Thanks for signning up an account on [WebsiteName].</p><p>Click <a href="[Url]">Here</a> to confirm your email in order to login. Thank You.</p><p>If you did not sign up an account on [WebsiteName], please ignore this email.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>[WebsiteName]</p>', N'ConfirmEmail')
INSERT [dbo].[EmailTemplate] ([Id], [Subject], [Body], [Type]) VALUES (N'809C6744-8632-4204-BB02-72EEBF748B84', N'Password Reset For [WebsiteName] Account', N'<p>Hi [Username],<br><br>Kindly be informed that your password for the [WebsiteName] account has been reset by [ResetByName].</p><p>Below is your temporary new password to log in:<br><b>New Password:</b> [NewPassword]</p><p><b>NOTE:</b> As a safety precaution, you are advised to change your password after you log in later. Thank you.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>[WebsiteName]</p>', N'PasswordResetByAdmin')
GO
INSERT [dbo].[GlobalOptionSet] ([Id], [Code], [DisplayName], [Type], [Status], [OptionOrder], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'1D0A8B2C-F5BF-44F4-B58E-04FE0A923DA0', N'ProfilePicture', N'Profile Picture', N'UserAttachment', N'Active', 1, NULL, NULL, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[GlobalOptionSet] ([Id], [Code], [DisplayName], [Type], [Status], [OptionOrder], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'2A538BDB-25AD-460F-A297-1D25503BC000', N'Other', N'Other', N'Gender', N'Active', 3, NULL, NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[GlobalOptionSet] ([Id], [Code], [DisplayName], [Type], [Status], [OptionOrder], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'2B6EB662-3F3F-45D4-9291-8088C7321D70', N'Male', N'Male', N'Gender', N'Active', 2, NULL, NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[GlobalOptionSet] ([Id], [Code], [DisplayName], [Type], [Status], [OptionOrder], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'4FEC0F55-03B0-4DC8-93B7-9099B2AFCAD6', N'Female', N'Female', N'Gender', N'Active', 1, NULL, NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[GlobalOptionSet] ([Id], [Code], [DisplayName], [Type], [Status], [OptionOrder], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'6A1672F3-4C0F-41F4-8D38-B25C97C0BCB2', N'NotValidated', N'Not Validated', N'UserStatus', N'Active', 3, NULL, NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[GlobalOptionSet] ([Id], [Code], [DisplayName], [Type], [Status], [OptionOrder], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'95848304-6BFB-4B79-B9D7-650103B1DE03', N'Registered', N'Registered', N'UserStatus', N'Active', 1, NULL, NULL, NULL, NULL, 1, NULL, NULL)
INSERT [dbo].[GlobalOptionSet] ([Id], [Code], [DisplayName], [Type], [Status], [OptionOrder], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'F213CC6E-09EB-419A-83D3-77A852FE6FEB', N'Banned', N'Banned', N'UserStatus', N'Active', 4, NULL, NULL, NULL, NULL, 0, NULL, NULL)
INSERT [dbo].[GlobalOptionSet] ([Id], [Code], [DisplayName], [Type], [Status], [OptionOrder], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [SystemDefault], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'F5ECBF7D-DCC2-4E4E-9755-AA1BF2E8B69F', N'Validated', N'Validated', N'UserStatus', N'Active', 2, NULL, NULL, NULL, NULL, 0, NULL, NULL)
GO
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'10A4FED3-D179-4E09-85A1-AEFDBAD46B89', N'UserStatus', N'User Status', N'/userstatus/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'1767BCEE-AE05-448D-8348-6EACAC4463DD', N'LoginHistory', N'Login History', N'/loginhistory/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'3113A195-9260-44FF-9138-1AB5C64983B4', N'RoleManagement', N'Role Management', N'/role/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'3E7B2F61-F4B5-4E34-BC11-98284C4D5191', N'DeviceType', N'Device Type Management', N'/devicetype/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'8F39E79A-E3A2-4957-92D6-F429C68F11A5', N'RepairManagement', N'Repair Management', N'/repair/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'954321C9-A0E8-42C5-A1A7-79F7B14A3D2E', N'Statistical', N'Statistical', N'/statistical/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'96DEF15B-7534-4485-84AD-476D97A14825', N'UserManagement', N'User Management', N'/user/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'A192F3CE-24B8-432A-AC02-F57DACE2FD1C', N'DeviceManagement', N'Device Management', N'/device/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'B2Y806DA-B561-4363-B10C-49E38ED1DF61', N'SupplierManagement', N'Supplier Management', N'/supplier/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'B4E801DA-B661-4923-B74C-42E38DD1DF68', N'Dashboard', N'Dashboard', N'/dashboard/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'BC12D46A-DB3A-4E74-AEF4-EC94DEFC23D8', N'RepairType', N'Repair Type Management', N'/repairtype/index', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Module] ([Id], [Code], [Name], [MainUrl], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'ED9A9D57-7917-4BEB-AAD2-9446A64532FF', N'UserAttachmentType', N'User Attachment Type', N'/userattachmenttype/index', NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'0F5CC3A6-29C3-453A-A91D-9A9002565A00', N'7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D', N'ED9A9D57-7917-4BEB-AAD2-9446A64532FF', 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'1DB24343-69A0-40E7-8EF0-8640E100434D', N'DCF4F5BC-D72C-453B-AC68-4CC7583F93B5', N'3113A195-9260-44FF-9138-1AB5C64983B4', 1, 1, 1, 1, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'30AE3AB0-DBBA-47B8-B16F-7F44F0A5F53B', N'7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D', N'10A4FED3-D179-4E09-85A1-AEFDBAD46B89', 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'41877B50-9811-4AA2-81C2-4B013D235084', N'DCF4F5BC-D72C-453B-AC68-4CC7583F93B5', N'1767BCEE-AE05-448D-8348-6EACAC4463DD', 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'44EAE7DF-8FBA-4152-A409-E4C05806AB9C', N'7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D', N'96DEF15B-7534-4485-84AD-476D97A14825', 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'49E06DDF-A5BC-49EE-897E-F3471A59B482', N'7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D', N'B4E801DA-B661-4923-B74C-42E38DD1DF68', 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'7C5E6F11-DAD8-4EC7-B95B-6E962F17E13A', N'7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D', N'3113A195-9260-44FF-9138-1AB5C64983B4', 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'A84E30B8-FC7E-4F7D-849F-4663DFF69205', N'7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D', N'1767BCEE-AE05-448D-8348-6EACAC4463DD', 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'ACB0E59E-8451-44F7-88E4-0616D3B0E9B1', N'DCF4F5BC-D72C-453B-AC68-4CC7583F93B5', N'B4E801DA-B661-4923-B74C-42E38DD1DF68', 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'BB1F9DE4-C626-401B-9E25-97676E2988AF', N'DCF4F5BC-D72C-453B-AC68-4CC7583F93B5', N'10A4FED3-D179-4E09-85A1-AEFDBAD46B89', 1, 1, 1, 1, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'C5967992-1EF3-4212-978C-E9F3257D237F', N'DCF4F5BC-D72C-453B-AC68-4CC7583F93B5', N'ED9A9D57-7917-4BEB-AAD2-9446A64532FF', 1, 1, 1, 1, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[RoleModulePermission] ([Id], [RoleId], [ModuleId], [ViewRight], [AddRight], [EditRight], [DeleteRight], [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn], [IsoUtcCreatedOn], [IsoUtcModifiedOn]) VALUES (N'E86F01CF-2E65-44CB-901A-839330BF7153', N'DCF4F5BC-D72C-453B-AC68-4CC7583F93B5', N'96DEF15B-7534-4485-84AD-476D97A14825', 1, 1, 1, 1, NULL, NULL, NULL, NULL, NULL, NULL)
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_GlobalOptionSetType]    Script Date: 3/13/2024 11:01:00 PM ******/
CREATE NONCLUSTERED INDEX [IX_GlobalOptionSetType] ON [dbo].[GlobalOptionSet]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RoleModulePermissionModuleId]    Script Date: 3/13/2024 11:01:01 PM ******/
CREATE NONCLUSTERED INDEX [IX_RoleModulePermissionModuleId] ON [dbo].[RoleModulePermission]
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_RoleModulePermissionRoleId]    Script Date: 3/13/2024 11:01:01 PM ******/
CREATE NONCLUSTERED INDEX [IX_RoleModulePermissionRoleId] ON [dbo].[RoleModulePermission]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserAttachmentUserProfileId]    Script Date: 3/13/2024 11:01:01 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserAttachmentUserProfileId] ON [dbo].[UserAttachment]
(
	[UserProfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserProfileUserStatusId]    Script Date: 3/13/2024 11:01:01 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserProfileUserStatusId] ON [dbo].[UserProfile]
(
	[UserStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GlobalOptionSet] ADD  DEFAULT ((0)) FOR [SystemDefault]
GO
ALTER TABLE [dbo].[RoleModulePermission] ADD  DEFAULT ((0)) FOR [ViewRight]
GO
ALTER TABLE [dbo].[RoleModulePermission] ADD  DEFAULT ((0)) FOR [AddRight]
GO
ALTER TABLE [dbo].[RoleModulePermission] ADD  DEFAULT ((0)) FOR [EditRight]
GO
ALTER TABLE [dbo].[RoleModulePermission] ADD  DEFAULT ((0)) FOR [DeleteRight]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceType] ([DeviceTypeId])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceType] ([DeviceTypeId])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[DeviceSoftware]  WITH CHECK ADD FOREIGN KEY([SoftwareId])
REFERENCES [dbo].[Software] ([SoftwareId])
GO
ALTER TABLE [dbo].[DeviceSoftware]  WITH CHECK ADD FOREIGN KEY([SoftwareId])
REFERENCES [dbo].[Device] ([DeviceId])
GO
ALTER TABLE [dbo].[DeviceSoftware]  WITH CHECK ADD FOREIGN KEY([SoftwareId])
REFERENCES [dbo].[Software] ([SoftwareId])
GO
ALTER TABLE [dbo].[DeviceSoftware]  WITH CHECK ADD FOREIGN KEY([SoftwareId])
REFERENCES [dbo].[Device] ([DeviceId])
GO
ALTER TABLE [dbo].[DraftCode]  WITH CHECK ADD FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceType] ([DeviceTypeId])
GO
ALTER TABLE [dbo].[DraftCode]  WITH CHECK ADD FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceType] ([DeviceTypeId])
GO
ALTER TABLE [dbo].[ErrorLog]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ErrorLog_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ErrorLog] CHECK CONSTRAINT [FK_dbo.ErrorLog_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[RepairDetail]  WITH CHECK ADD FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Device] ([DeviceId])
GO
ALTER TABLE [dbo].[RepairDetail]  WITH CHECK ADD FOREIGN KEY([RepairTypeId])
REFERENCES [dbo].[RepairType] ([RepairTypeId])
GO
ALTER TABLE [dbo].[SoftwareActivation]  WITH CHECK ADD FOREIGN KEY([ActivationKeyId])
REFERENCES [dbo].[ActivationKey] ([ActivationKeyId])
GO
ALTER TABLE [dbo].[SoftwareActivation]  WITH CHECK ADD FOREIGN KEY([ActivationKeyId])
REFERENCES [dbo].[ActivationKey] ([ActivationKeyId])
GO
ALTER TABLE [dbo].[SoftwareActivation]  WITH CHECK ADD FOREIGN KEY([SoftwareId])
REFERENCES [dbo].[Software] ([SoftwareId])
GO
ALTER TABLE [dbo].[SoftwareActivation]  WITH CHECK ADD FOREIGN KEY([SoftwareId])
REFERENCES [dbo].[Software] ([SoftwareId])
GO
ALTER TABLE [dbo].[Supplier]  WITH CHECK ADD FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[Supplier]  WITH CHECK ADD FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contact] ([ContactId])
GO
ALTER TABLE [dbo].[UserAttachment]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserAttachment_dbo.UserProfile_UserProfileId] FOREIGN KEY([UserProfileId])
REFERENCES [dbo].[UserProfile] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserAttachment] CHECK CONSTRAINT [FK_dbo.UserAttachment_dbo.UserProfile_UserProfileId]
GO
ALTER TABLE [dbo].[UserProfile]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserProfile_dbo.AspNetUsers_AspNetUserId] FOREIGN KEY([AspNetUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_dbo.UserProfile_dbo.AspNetUsers_AspNetUserId]
GO
USE [master]
GO
ALTER DATABASE [BDMPro] SET  READ_WRITE 
GO
