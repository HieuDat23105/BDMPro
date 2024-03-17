GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 12/24/2022 7:29:11 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 12/24/2022 7:29:12 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 12/24/2022 7:29:12 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 12/24/2022 7:29:12 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 12/24/2022 7:29:12 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 12/24/2022 7:29:12 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 12/24/2022 7:29:12 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
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


BEGIN
CREATE TABLE [dbo].[GlobalOptionSet] (
    [Id]            NVARCHAR (128)   NOT NULL,
    [Code]          NVARCHAR (256)    NULL,
    [DisplayName]   NVARCHAR (256)   NULL,
    [Type]          NVARCHAR (256)   NULL,
    [Status]        NVARCHAR (256)   NULL,
    [OptionOrder]   int NULL,
    [CreatedBy] NVARCHAR (128) NULL,
    [CreatedOn] datetime NULL,
    [ModifiedBy] NVARCHAR (128) NULL,
    [ModifiedOn] datetime NULL,
    [SystemDefault]   bit default(0) not null,
    [IsoUtcCreatedOn] NVARCHAR (128) NULL,
    [IsoUtcModifiedOn] NVARCHAR (128) NULL
    CONSTRAINT [PK_dbo.GlobalOptionSet] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END

BEGIN
CREATE NONCLUSTERED INDEX [IX_GlobalOptionSetType] ON [dbo].[GlobalOptionSet]([Type] ASC);
END

BEGIN
CREATE TABLE [dbo].[Module] (
    [Id]            NVARCHAR (128)   NOT NULL,
    [Code]          NVARCHAR (256)    NULL,
    [Name]          NVARCHAR (256)    NULL,
    [MainUrl]       NVARCHAR (max)   NULL,
    [CreatedBy]     NVARCHAR (128)   NULL,
    [CreatedOn]     datetime   NULL,
    [ModifiedBy]    NVARCHAR (128)   NULL,
    [ModifiedOn]    datetime   NULL,
    [IsoUtcCreatedOn] NVARCHAR (128) NULL,
    [IsoUtcModifiedOn] NVARCHAR (128) NULL
    CONSTRAINT [PK_dbo.Module] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END

BEGIN
CREATE TABLE [dbo].[RoleModulePermission] (
    [Id]            NVARCHAR (128)    NOT NULL,
    [RoleId]        NVARCHAR (128)  NOT NULL,
    [ModuleId]      NVARCHAR (128)    NOT NULL,
    [ViewRight]   bit  default(0) NOT NULL,
    [AddRight]     bit  default(0) NOT NULL,
    [EditRight]   bit  default(0) NOT NULL,
    [DeleteRight]   bit  default(0) NOT NULL,
    [CreatedBy]     NVARCHAR (128)   NULL,
    [CreatedOn]     datetime  NULL,
    [ModifiedBy]    NVARCHAR (128)   NULL,
    [ModifiedOn]    datetime   NULL,
    [IsoUtcCreatedOn] NVARCHAR (128) NULL,
    [IsoUtcModifiedOn] NVARCHAR (128) NULL
    CONSTRAINT [PK_dbo.RoleModulePermission] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END

BEGIN
CREATE NONCLUSTERED INDEX [IX_RoleModulePermissionRoleId] ON [dbo].[RoleModulePermission]([RoleId] ASC);
END

BEGIN
CREATE NONCLUSTERED INDEX [IX_RoleModulePermissionModuleId] ON [dbo].[RoleModulePermission]([ModuleId] ASC);
END

BEGIN
CREATE TABLE [dbo].[UserProfile] (
    [Id]                NVARCHAR (128)    NOT NULL,
    [AspNetUserId]     NVARCHAR (450)  NULL,
    [FirstName]           NVARCHAR (256)  NULL,
    [LastName]          NVARCHAR (256)  NULL,
    [FullName]          NVARCHAR (256)  NULL,
    [IDCardNumber]          NVARCHAR (256)  NULL,
    [DateOfBirth]          DateTime  NULL,
    [GenderId]     NVARCHAR (128)  NULL,
    [CountryName]     NVARCHAR(256)  NULL,
    [Address]     NVARCHAR (max)  NULL,
    [PostalCode]     NVARCHAR (128)  NULL,
    [PhoneNumber]     NVARCHAR (256)  NULL,
    [UserStatusId]     NVARCHAR (128)  NULL,
    [CreatedBy]         NVARCHAR (128)   NULL,
    [CreatedOn]         datetime  NULL,
    [ModifiedBy]        NVARCHAR (128)   NULL,
    [ModifiedOn]        datetime   NULL,
    [IsoUtcDateOfBirth] NVARCHAR (128) NULL,
    [IsoUtcCreatedOn] NVARCHAR (128) NULL,
    [IsoUtcModifiedOn] NVARCHAR (128) NULL
    CONSTRAINT [PK_dbo.UserProfile] PRIMARY KEY CLUSTERED ([Id] ASC),
    --When delete user from AspNetUsers, delete the record from UserProfile as well
    CONSTRAINT [FK_dbo.UserProfile_dbo.AspNetUsers_AspNetUserId] FOREIGN KEY ([AspNetUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
END

BEGIN
CREATE NONCLUSTERED INDEX [IX_UserProfileUserStatusId] ON [dbo].[UserProfile]([UserStatusId] ASC);
END

BEGIN
CREATE TABLE [dbo].[UserAttachment] (
    [Id]                NVARCHAR (128)    NOT NULL,
    [UserProfileId]     NVARCHAR (128)  NULL,
    [FileUrl]           NVARCHAR (max)  NULL,
    [FileName]          NVARCHAR (256)  NULL,
    [UniqueFileName]    NVARCHAR (256)  NULL,
    [AttachmentTypeId]  NVARCHAR (128)     NULL,
    [CreatedBy]         NVARCHAR (128)   NULL,
    [CreatedOn]         datetime  NULL,
    [ModifiedBy]        NVARCHAR (128)   NULL,
    [ModifiedOn]        datetime   NULL,
    [IsoUtcCreatedOn] NVARCHAR (128) NULL,
    [IsoUtcModifiedOn] NVARCHAR (128) NULL
    CONSTRAINT [PK_dbo.UserAttachment] PRIMARY KEY CLUSTERED ([Id] ASC),
    --When delete user from [UserProfile], delete the record from [UserAttachment] as well
    CONSTRAINT [FK_dbo.UserAttachment_dbo.UserProfile_UserProfileId] FOREIGN KEY ([UserProfileId]) REFERENCES [dbo].[UserProfile] ([Id]) ON DELETE CASCADE
);
END

BEGIN
CREATE NONCLUSTERED INDEX [IX_UserAttachmentUserProfileId] ON [dbo].[UserAttachment]([UserProfileId] ASC);
END

BEGIN
CREATE TABLE [dbo].[EmailTemplate] (
    [Id]            NVARCHAR (128)    NOT NULL,
    [Subject]        NVARCHAR (max)   NULL,
    [Body]        NVARCHAR (max)   NULL,
    [Type]        NVARCHAR (256)   NULL
    CONSTRAINT [PK_dbo.EmailTemplate] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END

BEGIN
CREATE TABLE [dbo].[LoginHistory] (
    [Id]            NVARCHAR (128)    NOT NULL,
    [AspNetUserId]  NVARCHAR (450)   NULL,
    [LoginDateTime] datetime   NULL,
    [IsoUtcLoginDateTime] NVARCHAR (128) NULL
    CONSTRAINT [PK_dbo.LoginHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END

BEGIN
insert into GlobalOptionSet values('1D0A8B2C-F5BF-44F4-B58E-04FE0A923DA0','ProfilePicture','Profile Picture','UserAttachment','Active',1,null,null,null,null,1,null,null)
END

BEGIN
insert into GlobalOptionSet values('4FEC0F55-03B0-4DC8-93B7-9099B2AFCAD6','Female','Female','Gender','Active',1,null,null,null,null,0,null,null)
insert into GlobalOptionSet values('2B6EB662-3F3F-45D4-9291-8088C7321D70','Male','Male','Gender','Active',2,null,null,null,null,0,null,null)
insert into GlobalOptionSet values('2A538BDB-25AD-460F-A297-1D25503BC000','Other','Other','Gender','Active',3,null,null,null,null,0,null,null)
END

BEGIN
insert into GlobalOptionSet values('95848304-6BFB-4B79-B9D7-650103B1DE03','Registered','Registered','UserStatus','Active',1,null,null,null,null,1,null,null)
insert into GlobalOptionSet values('F5ECBF7D-DCC2-4E4E-9755-AA1BF2E8B69F','Validated','Validated','UserStatus','Active',2,null,null,null,null,0,null,null)
insert into GlobalOptionSet values('6A1672F3-4C0F-41F4-8D38-B25C97C0BCB2','NotValidated','Not Validated','UserStatus','Active',3,null,null,null,null,0,null,null)
insert into GlobalOptionSet values('F213CC6E-09EB-419A-83D3-77A852FE6FEB','Banned','Banned','UserStatus','Active',4,null,null,null,null,0,null,null)
END

BEGIN
insert into Module values ('10A4FED3-D179-4E09-85A1-AEFDBAD46B89','UserStatus','User Status','/userstatus/index', null,null,null,null,null,null)
insert into Module values ('ED9A9D57-7917-4BEB-AAD2-9446A64532FF','UserAttachmentType','User Attachment Type','/userattachmenttype/index', null,null,null,null,null,null)
insert into Module values ('3113A195-9260-44FF-9138-1AB5C64983B4','RoleManagement','Role Management','/role/index', null,null,null,null,null,null)
insert into Module values ('96DEF15B-7534-4485-84AD-476D97A14825','UserManagement','User Management','/user/index', null,null,null,null,null,null)
insert into Module values ('1767BCEE-AE05-448D-8348-6EACAC4463DD','LoginHistory','Login History','/loginhistory/index', null,null,null,null,null,null)
insert into Module values ('B4E801DA-B661-4923-B74C-42E38DD1DF68','Dashboard','Dashboard','/dashboard/index', null,null,null,null,null,null)
END

BEGIN
insert into AspNetRoles values ('DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','System Admin','SYSTEM ADMIN',null,null,null,null,1,null,null,null)
insert into AspNetRoles values ('7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','Normal User','NORMAL USER',null,null,null,null,1,null,null,null)

--dashboard module
insert into RoleModulePermission values ('ACB0E59E-8451-44F7-88E4-0616D3B0E9B1','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','B4E801DA-B661-4923-B74C-42E38DD1DF68',1,0,0,0,null,null,null,null,null,null) --admin
insert into RoleModulePermission values ('49E06DDF-A5BC-49EE-897E-F3471A59B482','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','B4E801DA-B661-4923-B74C-42E38DD1DF68',1,0,0,0,null,null,null,null,null,null) --user
--userstatus module
insert into RoleModulePermission values ('BB1F9DE4-C626-401B-9E25-97676E2988AF','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','10A4FED3-D179-4E09-85A1-AEFDBAD46B89',1,1,1,1,null,null,null,null,null,null) --admin
insert into RoleModulePermission values ('30AE3AB0-DBBA-47B8-B16F-7F44F0A5F53B','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','10A4FED3-D179-4E09-85A1-AEFDBAD46B89',0,0,0,0,null,null,null,null,null,null) --user
--userattachmenttype module
insert into RoleModulePermission values ('C5967992-1EF3-4212-978C-E9F3257D237F','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','ED9A9D57-7917-4BEB-AAD2-9446A64532FF',1,1,1,1,null,null,null,null,null,null) --admin
insert into RoleModulePermission values ('0F5CC3A6-29C3-453A-A91D-9A9002565A00','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','ED9A9D57-7917-4BEB-AAD2-9446A64532FF',0,0,0,0,null,null,null,null,null,null) --user
--rolemanagement module
insert into RoleModulePermission values ('1DB24343-69A0-40E7-8EF0-8640E100434D','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','3113A195-9260-44FF-9138-1AB5C64983B4',1,1,1,1,null,null,null,null,null,null) --admin
insert into RoleModulePermission values ('7C5E6F11-DAD8-4EC7-B95B-6E962F17E13A','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','3113A195-9260-44FF-9138-1AB5C64983B4',0,0,0,0,null,null,null,null,null,null) --user
--usermanagement module
insert into RoleModulePermission values ('E86F01CF-2E65-44CB-901A-839330BF7153','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','96DEF15B-7534-4485-84AD-476D97A14825',1,1,1,1,null,null,null,null,null,null) --admin
insert into RoleModulePermission values ('44EAE7DF-8FBA-4152-A409-E4C05806AB9C','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','96DEF15B-7534-4485-84AD-476D97A14825',0,0,0,0,null,null,null,null,null,null) --user
--loginhistory module
insert into RoleModulePermission values ('41877B50-9811-4AA2-81C2-4B013D235084','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','1767BCEE-AE05-448D-8348-6EACAC4463DD',1,0,0,0,null,null,null,null,null,null) --admin
insert into RoleModulePermission values ('A84E30B8-FC7E-4F7D-849F-4663DFF69205','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','1767BCEE-AE05-448D-8348-6EACAC4463DD',1,0,0,0,null,null,null,null,null,null) --user
END

BEGIN
insert into EmailTemplate values('37F6A753-2F8A-4808-AEDF-3512B474DA15','Confirm Your Email To Complete [WebsiteName] Account Registration','<p>Hi [Username],<br><br>Thanks for signning up an account on [WebsiteName].</p><p>Click <a href="[Url]">Here</a> to confirm your email in order to login. Thank You.</p><p>If you did not sign up an account on [WebsiteName], please ignore this email.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>[WebsiteName]</p>','ConfirmEmail')
insert into EmailTemplate values('809C6744-8632-4204-BB02-72EEBF748B84','Password Reset For [WebsiteName] Account','<p>Hi [Username],<br><br>Kindly be informed that your password for the [WebsiteName] account has been reset by [ResetByName].</p><p>Below is your temporary new password to log in:<br><b>New Password:</b> [NewPassword]</p><p><b>NOTE:</b> As a safety precaution, you are advised to change your password after you log in later. Thank you.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>[WebsiteName]</p>','PasswordResetByAdmin')
insert into EmailTemplate values('27D8409F-502D-4A01-8DA1-8D756EA00D0C','Password Reset For [WebsiteName] Account','<p>Hi [Username],<br><br>There was a request to reset your password on [WebsiteName].</p><p><a href="[Url]">Click Here</a> and follow the instructions to reset your password. Thank You.</p><p></p><p>If you did not make this request then please ignore this email.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>[WebsiteName]</p>','ForgotPassword')
END
