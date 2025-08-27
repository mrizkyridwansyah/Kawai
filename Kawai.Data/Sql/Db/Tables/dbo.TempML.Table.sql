SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempML](
	[parentitem_code] [char](25) NULL,
	[level_DP] [char](15) NULL,
	[amount] [decimal](18, 2) NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]
GO
