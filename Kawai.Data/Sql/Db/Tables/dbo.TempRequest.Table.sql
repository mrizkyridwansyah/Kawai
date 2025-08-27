SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempRequest](
	[Parent_ItemCode] [char](25) NULL,
	[SeqNo] [numeric](18, 0) NULL,
	[LotNo] [char](15) NULL,
	[Remarks] [char](25) NULL,
	[Item_Code] [char](25) NULL,
	[Unit_Cls] [char](2) NULL,
	[QtyBOM] [numeric](18, 5) NULL,
	[WH_Code] [char](15) NULL,
	[Material_cls] [char](2) NULL,
	[Data] [char](15) NULL,
	[ItemType] [char](15) NULL
) ON [PRIMARY]
GO
