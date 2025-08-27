SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Packing_Detail](
	[Packing_No] [char](25) NOT NULL,
	[PackingSeq_No] [numeric](4, 0) NOT NULL,
	[Container_No] [char](35) NOT NULL,
	[Seal_No] [char](25) NULL,
	[Container_Size] [char](25) NOT NULL,
	[Order_No] [char](35) NULL,
	[Order_SeqNo] [numeric](4, 0) NULL,
	[Item_Code] [char](25) NULL,
	[MakerItem_Code] [char](30) NULL,
	[Qty] [numeric](18, 5) NULL,
	[SerialNoFrom] [char](10) NULL,
	[SerialNoTo] [char](10) NULL,
	[QtyWeight_Netto] [numeric](18, 5) NULL,
	[QtyWeight_Gross] [numeric](18, 5) NULL,
	[Qty_Volume] [numeric](18, 5) NOT NULL,
	[Qty_Ctn] [numeric](18, 5) NULL,
	[Ctn_No] [char](15) NULL,
	[Detail_Cls] [char](1) NULL,
	[Unit_Cls] [char](2) NULL,
	[Currency_Code] [char](2) NULL,
	[Do_No] [char](25) NOT NULL,
	[DoSeq_No] [numeric](18, 0) NULL,
	[Do_Date] [datetime] NULL,
	[Price] [numeric](18, 5) NULL,
	[Service] [numeric](18, 5) NULL,
	[Amount] [numeric](22, 5) NULL,
	[Length] [numeric](9, 2) NULL,
	[Width] [numeric](9, 2) NULL,
	[Thickness] [numeric](9, 2) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Packing_Detail] PRIMARY KEY CLUSTERED 
(
	[Packing_No] ASC,
	[PackingSeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Packing_Detail]  WITH CHECK ADD  CONSTRAINT [FK_Packing_Detail_Packing_Master] FOREIGN KEY([Packing_No])
REFERENCES [Packing_Master] ([Packing_No])
GO
ALTER TABLE [Packing_Detail] CHECK CONSTRAINT [FK_Packing_Detail_Packing_Master]
GO
