SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SP_PackingList_Master](
	[Packing_No] [char](15) NULL,
	[Part_no] [char](15) NULL,
	[Model] [char](25) NULL,
	[Unit_Price] [numeric](18, 5) NULL,
	[Remarks] [char](25) NULL,
	[Qty] [numeric](18, 0) NULL,
	[Loading_Form_Status] [char](1) NULL,
	[Scanned_FG_Status] [char](1) NULL
) ON [PRIMARY]
GO
