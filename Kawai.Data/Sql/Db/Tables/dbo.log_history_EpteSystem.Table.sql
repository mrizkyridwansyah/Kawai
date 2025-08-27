SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [log_history_EpteSystem](
	[Tanggal] [date] NOT NULL,
	[UserID] [char](15) NOT NULL,
	[MenuDesc] [char](50) NOT NULL,
	[Last_Update] [datetime] NOT NULL
) ON [PRIMARY]
GO
