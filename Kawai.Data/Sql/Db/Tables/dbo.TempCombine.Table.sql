SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempCombine](
	[Item_Code] [char](25) NULL,
	[Item_Name] [char](75) NULL,
	[Begining] [numeric](18, 5) NULL,
	[Requirement] [numeric](18, 5) NULL
) ON [PRIMARY]
GO
