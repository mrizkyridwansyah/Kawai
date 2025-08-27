SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Temp_Receipt](
	[Item_Code] [char](25) NULL,
	[SJ_No] [varchar](50) NULL,
	[PO_No] [varchar](50) NULL,
	[Price] [numeric](18, 5) NULL
) ON [PRIMARY]
GO
