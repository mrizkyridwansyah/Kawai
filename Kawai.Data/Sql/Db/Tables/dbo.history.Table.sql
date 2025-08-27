SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [history](
	[SeqNo] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Tanggal] [char](24) NULL,
	[UserID] [char](15) NOT NULL,
	[MenuDesc] [char](50) NOT NULL,
	[Last_Update] [char](25) NULL
) ON [PRIMARY]
GO
