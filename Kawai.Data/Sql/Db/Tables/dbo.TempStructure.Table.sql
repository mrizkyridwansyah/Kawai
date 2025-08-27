SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempStructure](
	[UTAMA] [char](25) NULL,
	[Parent_ItemCode] [char](25) NULL,
	[Item_Code] [char](25) NULL,
	[QtyBOM] [numeric](18, 5) NULL,
	[Control_cls] [char](2) NULL,
	[Unit_cls] [char](2) NULL,
	[ItemType] [char](15) NULL
) ON [PRIMARY]
GO
