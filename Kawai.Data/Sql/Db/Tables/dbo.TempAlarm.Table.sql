SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempAlarm](
	[Item_Code] [char](25) NULL,
	[Qty] [numeric](18, 5) NULL,
	[Closing_Date] [datetime] NULL
) ON [PRIMARY]
GO
