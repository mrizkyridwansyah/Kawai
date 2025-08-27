SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempBom](
	[parentitem_code] [char](25) NULL,
	[ChildItem_Code] [char](25) NULL,
	[Qty] [numeric](18, 5) NOT NULL,
	[RQty] [numeric](18, 5) NULL,
	[Unit_Cls] [char](2) NULL,
	[Start_Date] [char](8) NULL,
	[End_Date] [char](8) NULL,
	[SeqNo] [numeric](4, 0) NULL,
	[level] [char](2) NULL,
	[BomSeq_No] [numeric](5, 0) NULL,
	[Doc_No] [char](50) NULL,
	[Revision_No] [char](50) NULL
) ON [PRIMARY]
GO
