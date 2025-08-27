SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempRequest2](
	[Parent_ItemCode] [char](25) NOT NULL,
	[SeqNo] [numeric](18, 0) NOT NULL,
	[LotNo] [char](15) NOT NULL,
	[Remarks] [char](25) NULL,
	[Item_Code] [char](25) NOT NULL,
	[Unit_Cls] [char](2) NULL,
	[QtyBOM] [numeric](18, 5) NULL,
	[WH_Code] [char](15) NULL,
	[Material_cls] [char](2) NULL,
	[Data] [char](15) NULL,
	[ItemType] [char](15) NOT NULL,
 CONSTRAINT [PK_TempRequest] PRIMARY KEY CLUSTERED 
(
	[Parent_ItemCode] ASC,
	[SeqNo] ASC,
	[LotNo] ASC,
	[Item_Code] ASC,
	[ItemType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
