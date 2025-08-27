SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Connection_Mysql](
	[ServerName] [char](25) NULL,
	[Port] [char](10) NULL,
	[DatabaseName] [char](30) NULL,
	[UserId] [char](30) NULL,
	[Password] [char](250) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [char](30) NULL,
	[RegisterDate] [datetime] NULL
) ON [PRIMARY]
GO
