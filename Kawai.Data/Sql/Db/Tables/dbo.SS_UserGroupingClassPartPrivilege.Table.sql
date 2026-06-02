CREATE TABLE [dbo].[SS_UserGroupingClassPartPrivilege](
	[UserID] [varchar](50) NOT NULL,
	[GroupingClassPartCode] [varchar](25) NOT NULL,
	[AllowAccess] [bit] NULL,
	[Last_User] [char](15) NULL,
	[Last_Update] [datetime] NULL,
 CONSTRAINT [PK_SS_UserGroupingClassPartPrivilege] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[GroupingClassPartCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
