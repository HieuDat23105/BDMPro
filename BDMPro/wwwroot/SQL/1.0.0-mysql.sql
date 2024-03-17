CREATE TABLE AspNetRoleClaims(
	Id int NOT NULL AUTO_INCREMENT,
	RoleId VARCHAR(128) NOT NULL,
	ClaimType TEXT NULL,
	ClaimValue TEXT NULL,
	PRIMARY KEY (Id)
 );

CREATE TABLE AspNetRoles (
    Id VARCHAR(128) NOT NULL,
    Name VARCHAR(256),
    NormalizedName VARCHAR(256),
    CreatedBy VARCHAR(128),
    CreatedOn datetime(6) NULL,
    ModifiedBy VARCHAR(128),
    ModifiedOn datetime(6) NULL,
    SystemDefault bit NOT NULL,
    IsoUtcCreatedOn VARCHAR(128) NULL,
    IsoUtcModifiedOn VARCHAR(128) NULL,
    ConcurrencyStamp TEXT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE AspNetUserClaims(
Id int NOT NULL AUTO_INCREMENT,
	UserId VARCHAR(128) NOT NULL,
	ClaimType TEXT NULL,
	ClaimValue TEXT NULL,
    PRIMARY KEY (Id)
 );

CREATE TABLE AspNetUserLogins(
	LoginProvider VARCHAR(128) NOT NULL,
	ProviderKey VARCHAR(128) NOT NULL,
	ProviderDisplayName TEXT NULL,
	UserId VARCHAR(128) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey)
 );

CREATE TABLE AspNetUserRoles(
	UserId VARCHAR(128) NOT NULL,
	RoleId VARCHAR(128) NOT NULL,
 PRIMARY KEY (UserId, RoleId)
 );

 CREATE TABLE AspNetUsers(
	Id VARCHAR(128) NOT NULL,
	LockoutEndDateUtc datetime(6) NULL,
	UserName VARCHAR(256) NULL,
	NormalizedUserName VARCHAR(256) NULL,
	Email VARCHAR(256) NULL,
	NormalizedEmail VARCHAR(256) NULL,
	EmailConfirmed bit NOT NULL,
	PasswordHash TEXT NULL,
	SecurityStamp TEXT NULL,
	ConcurrencyStamp TEXT NULL,
	PhoneNumber TEXT NULL,
	PhoneNumberConfirmed bit NOT NULL,
	TwoFactorEnabled bit NOT NULL,
	LockoutEnd TIMESTAMP NULL,
	LockoutEnabled bit NOT NULL,
	AccessFailedCount int NOT NULL,
    PRIMARY KEY (Id)
 );

CREATE TABLE AspNetUserTokens(
	UserId VARCHAR(128) NOT NULL,
	LoginProvider VARCHAR(128) NOT NULL,
	Name VARCHAR(128) NOT NULL,
	Value TEXT NULL,
    PRIMARY KEY (UserId, LoginProvider)
);

ALTER TABLE AspNetRoleClaims ADD CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY(RoleId)
REFERENCES AspNetRoles (Id)
ON DELETE CASCADE;

ALTER TABLE AspNetUserClaims ADD CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY(UserId)
REFERENCES AspNetUsers (Id)
ON DELETE CASCADE;

ALTER TABLE AspNetUserLogins ADD CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY(UserId)
REFERENCES AspNetUsers (Id)
ON DELETE CASCADE;

ALTER TABLE AspNetUserRoles ADD CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY(RoleId)
REFERENCES AspNetRoles (Id)
ON DELETE CASCADE;

ALTER TABLE AspNetUserRoles ADD CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY(UserId)
REFERENCES AspNetUsers (Id)
ON DELETE CASCADE;

ALTER TABLE AspNetUserTokens ADD CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY(UserId)
REFERENCES AspNetUsers (Id)
ON DELETE CASCADE;

CREATE TABLE GlobalOptionSet (
    Id            VARCHAR (128)   NOT NULL,
    Code          VARCHAR (256)    NULL,
    DisplayName   VARCHAR (256)   NULL,
    Type          VARCHAR (256)   NULL,
    Status        VARCHAR (256)   NULL,
    OptionOrder   int NULL,
    CreatedBy VARCHAR (128) NULL,
    CreatedOn datetime(6) NULL,
    ModifiedBy VARCHAR (128) NULL,
    ModifiedOn datetime(6) NULL,
    SystemDefault   bit default(0) not null,
    IsoUtcCreatedOn VARCHAR (128) NULL,
    IsoUtcModifiedOn VARCHAR (128) NULL,
    PRIMARY KEY (Id)
);

CREATE INDEX IX_GlobalOptionSetType ON GlobalOptionSet(Type);

CREATE TABLE Module (
    Id            VARCHAR (128)   NOT NULL,
    Code          VARCHAR (256)    NULL,
    Name          VARCHAR (256)    NULL,
    MainUrl       TEXT  NULL,
    CreatedBy     VARCHAR (128)   NULL,
    CreatedOn     datetime(6) NULL,
    ModifiedBy    VARCHAR (128)   NULL,
    ModifiedOn    datetime(6) NULL,
    IsoUtcCreatedOn VARCHAR (128) NULL,
    IsoUtcModifiedOn VARCHAR (128) NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE RoleModulePermission (
    Id            VARCHAR (128)    NOT NULL,
    RoleId        VARCHAR (128)  NOT NULL,
    ModuleId      VARCHAR (128)    NOT NULL,
    ViewRight   bit  default(0) NOT NULL,
    AddRight     bit  default(0) NOT NULL,
    EditRight   bit  default(0) NOT NULL,
    DeleteRight   bit  default(0) NOT NULL,
    CreatedBy     VARCHAR (128)   NULL,
    CreatedOn     datetime(6) NULL,
    ModifiedBy    VARCHAR (128)   NULL,
    ModifiedOn    datetime(6) NULL,
    IsoUtcCreatedOn VARCHAR (128) NULL,
    IsoUtcModifiedOn VARCHAR (128) NULL,
    PRIMARY KEY (Id)
);

CREATE INDEX IX_RoleModulePermissionRoleId ON RoleModulePermission(RoleId);
CREATE INDEX IX_RoleModulePermissionModuleId ON RoleModulePermission(ModuleId);

CREATE TABLE UserProfile (
    Id                VARCHAR (128)    NOT NULL,
    AspNetUserId     VARCHAR (128)  NULL,
    FirstName           VARCHAR (256)  NULL,
    LastName          VARCHAR (256)  NULL,
    FullName          VARCHAR (256)  NULL,
    IDCardNumber          VARCHAR (256)  NULL,
    DateOfBirth          datetime(6) NULL,
    GenderId     VARCHAR (128)  NULL,
    CountryName     VARCHAR(256)  NULL,
    Address     TEXT  NULL,
    PostalCode     VARCHAR (128)  NULL,
    PhoneNumber     VARCHAR (256)  NULL,
    UserStatusId     VARCHAR (128)  NULL,
    CreatedBy         VARCHAR (128)   NULL,
    CreatedOn         datetime(6) NULL,
    ModifiedBy        VARCHAR (128)   NULL,
    ModifiedOn        datetime(6) NULL,
    IsoUtcDateOfBirth VARCHAR (128) NULL,
    IsoUtcCreatedOn VARCHAR (128) NULL,
    IsoUtcModifiedOn VARCHAR (128) NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (AspNetUserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE INDEX IX_UserProfileUserStatusId ON UserProfile(UserStatusId);

CREATE TABLE UserAttachment (
    Id                VARCHAR (128)    NOT NULL,
    UserProfileId     VARCHAR (128)  NULL,
    FileUrl           TEXT  NULL,
    FileName          VARCHAR (256)  NULL,
    UniqueFileName    VARCHAR (256)  NULL,
    AttachmentTypeId  VARCHAR (128)     NULL,
    CreatedBy         VARCHAR (128)   NULL,
    CreatedOn         datetime(6) NULL,
    ModifiedBy        VARCHAR (128)   NULL,
    ModifiedOn        datetime(6) NULL,
    IsoUtcCreatedOn VARCHAR (128) NULL,
    IsoUtcModifiedOn VARCHAR (128) NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (UserProfileId) REFERENCES UserProfile(Id) ON DELETE CASCADE
);

CREATE INDEX IX_UserAttachmentUserProfileId ON UserAttachment(UserProfileId);

CREATE TABLE EmailTemplate (
    Id            VARCHAR (128)    NOT NULL,
    Subject        TEXT   NULL,
    Body        TEXT  NULL,
    Type        VARCHAR (256)   NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE LoginHistory (
    Id            VARCHAR (128)    NOT NULL,
    AspNetUserId  VARCHAR (128)   NULL,
    LoginDateTime datetime(6) NULL,
    IsoUtcLoginDateTime VARCHAR (128) NULL,
    PRIMARY KEY (Id)
);

insert into GlobalOptionSet values('1D0A8B2C-F5BF-44F4-B58E-04FE0A923DA0','ProfilePicture','Profile Picture','UserAttachment','Active',1,null,null,null,null,1,null,null);

insert into GlobalOptionSet values('4FEC0F55-03B0-4DC8-93B7-9099B2AFCAD6','Female','Female','Gender','Active',1,null,null,null,null,0,null,null);
insert into GlobalOptionSet values('2B6EB662-3F3F-45D4-9291-8088C7321D70','Male','Male','Gender','Active',2,null,null,null,null,0,null,null);
insert into GlobalOptionSet values('2A538BDB-25AD-460F-A297-1D25503BC000','Other','Other','Gender','Active',3,null,null,null,null,0,null,null);

insert into GlobalOptionSet values('95848304-6BFB-4B79-B9D7-650103B1DE03','Registered','Registered','UserStatus','Active',1,null,null,null,null,1,null,null);
insert into GlobalOptionSet values('F5ECBF7D-DCC2-4E4E-9755-AA1BF2E8B69F','Validated','Validated','UserStatus','Active',2,null,null,null,null,0,null,null);
insert into GlobalOptionSet values('6A1672F3-4C0F-41F4-8D38-B25C97C0BCB2','NotValidated','Not Validated','UserStatus','Active',3,null,null,null,null,0,null,null);
insert into GlobalOptionSet values('F213CC6E-09EB-419A-83D3-77A852FE6FEB','Banned','Banned','UserStatus','Active',4,null,null,null,null,0,null,null);

insert into Module values ('10A4FED3-D179-4E09-85A1-AEFDBAD46B89','UserStatus','User Status','/userstatus/index', null,null,null,null,null,null);
insert into Module values ('ED9A9D57-7917-4BEB-AAD2-9446A64532FF','UserAttachmentType','User Attachment Type','/userattachmenttype/index', null,null,null,null,null,null);
insert into Module values ('3113A195-9260-44FF-9138-1AB5C64983B4','RoleManagement','Role Management','/role/index', null,null,null,null,null,null);
insert into Module values ('96DEF15B-7534-4485-84AD-476D97A14825','UserManagement','User Management','/user/index', null,null,null,null,null,null);
insert into Module values ('1767BCEE-AE05-448D-8348-6EACAC4463DD','LoginHistory','Login History','/loginhistory/index', null,null,null,null,null,null);
insert into Module values ('B4E801DA-B661-4923-B74C-42E38DD1DF68','Dashboard','Dashboard','/dashboard/index', null,null,null,null,null,null);

insert into AspNetRoles values ('DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','System Admin','SYSTEM ADMIN',null,null,null,null,1,null,null,null);
insert into AspNetRoles values ('7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','Normal User','NORMAL USER',null,null,null,null,1,null,null,null);

insert into RoleModulePermission values ('ACB0E59E-8451-44F7-88E4-0616D3B0E9B1','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','B4E801DA-B661-4923-B74C-42E38DD1DF68',1,0,0,0,null,null,null,null,null,null);
insert into RoleModulePermission values ('49E06DDF-A5BC-49EE-897E-F3471A59B482','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','B4E801DA-B661-4923-B74C-42E38DD1DF68',1,0,0,0,null,null,null,null,null,null);

insert into RoleModulePermission values ('BB1F9DE4-C626-401B-9E25-97676E2988AF','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','10A4FED3-D179-4E09-85A1-AEFDBAD46B89',1,1,1,1,null,null,null,null,null,null);
insert into RoleModulePermission values ('30AE3AB0-DBBA-47B8-B16F-7F44F0A5F53B','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','10A4FED3-D179-4E09-85A1-AEFDBAD46B89',0,0,0,0,null,null,null,null,null,null);

insert into RoleModulePermission values ('C5967992-1EF3-4212-978C-E9F3257D237F','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','ED9A9D57-7917-4BEB-AAD2-9446A64532FF',1,1,1,1,null,null,null,null,null,null);
insert into RoleModulePermission values ('0F5CC3A6-29C3-453A-A91D-9A9002565A00','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','ED9A9D57-7917-4BEB-AAD2-9446A64532FF',0,0,0,0,null,null,null,null,null,null);

insert into RoleModulePermission values ('1DB24343-69A0-40E7-8EF0-8640E100434D','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','3113A195-9260-44FF-9138-1AB5C64983B4',1,1,1,1,null,null,null,null,null,null);
insert into RoleModulePermission values ('7C5E6F11-DAD8-4EC7-B95B-6E962F17E13A','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','3113A195-9260-44FF-9138-1AB5C64983B4',0,0,0,0,null,null,null,null,null,null);

insert into RoleModulePermission values ('E86F01CF-2E65-44CB-901A-839330BF7153','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','96DEF15B-7534-4485-84AD-476D97A14825',1,1,1,1,null,null,null,null,null,null);
insert into RoleModulePermission values ('44EAE7DF-8FBA-4152-A409-E4C05806AB9C','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','96DEF15B-7534-4485-84AD-476D97A14825',0,0,0,0,null,null,null,null,null,null);

insert into RoleModulePermission values ('41877B50-9811-4AA2-81C2-4B013D235084','DCF4F5BC-D72C-453B-AC68-4CC7583F93B5','1767BCEE-AE05-448D-8348-6EACAC4463DD',1,0,0,0,null,null,null,null,null,null);
insert into RoleModulePermission values ('A84E30B8-FC7E-4F7D-849F-4663DFF69205','7ABA3C40-F31F-4AB5-BF39-4EEA3CCDE82D','1767BCEE-AE05-448D-8348-6EACAC4463DD',1,0,0,0,null,null,null,null,null,null);

insert into EmailTemplate values('37F6A753-2F8A-4808-AEDF-3512B474DA15','Confirm Your Email To Complete WebsiteName Account Registration','<p>Hi Username,<br><br>Thanks for signning up an account on WebsiteName.</p><p>Click <a href="Url">Here</a> to confirm your email in order to login. Thank You.</p><p>If you did not sign up an account on WebsiteName, please ignore this email.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>WebsiteName</p>','ConfirmEmail');
insert into EmailTemplate values('809C6744-8632-4204-BB02-72EEBF748B84','Password Reset For WebsiteName Account','<p>Hi Username,<br><br>Kindly be informed that your password for the WebsiteName account has been reset by ResetByName.</p><p>Below is your temporary new password to log in:<br><b>New Password:</b> NewPassword</p><p><b>NOTE:</b> As a safety precaution, you are advised to change your password after you log in later. Thank you.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>WebsiteName</p>','PasswordResetByAdmin');
insert into EmailTemplate values('27D8409F-502D-4A01-8DA1-8D756EA00D0C','Password Reset For WebsiteName Account','<p>Hi Username,<br><br>There was a request to reset your password on WebsiteName.</p><p><a href="Url">Click Here</a> and follow the instructions to reset your password. Thank You.</p><p></p><p>If you did not make this request then please ignore this email.</p><p><i>Do not reply to this email.</i></p><p>Regards,<br>WebsiteName</p>','ForgotPassword');

