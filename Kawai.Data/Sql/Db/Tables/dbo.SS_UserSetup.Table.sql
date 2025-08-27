SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SS_UserSetup](
	[UserID] [varchar](30) NOT NULL,
	[FullName] [varchar](50) NULL,
	[Password] [varchar](max) NULL,
	[StatusAdmin] [bit] NULL,
	[UserGroup] [varchar](50) NULL,
	[LastLogin] [smalldatetime] NULL,
	[Status] [varchar](20) NULL,
	[StatusLogin] [char](1) NULL,
	[StatusJob] [char](1) NULL,
	[IPAddress] [varchar](15) NULL,
	[LastLogout] [datetime] NULL,
	[PasswordExpired] [date] NULL,
	[JobPosition] [varchar](10) NULL,
	[UserPhoto] [varchar](max) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [char](50) NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [char](50) NULL,
	[EmployeeID] [varchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
