SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Delivery_Order](
	[DO_No] [char](25) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[MakerItem_Code] [char](30) NULL,
	[Delivery_Date] [datetime] NOT NULL,
	[Delivery_Time] [char](5) NULL,
	[PO_No] [char](35) NOT NULL,
	[Seq_No] [numeric](4, 0) NOT NULL,
	[DOSeq_No] [numeric](4, 0) NOT NULL,
	[Qty] [numeric](18, 5) NULL,
	[SerialNoFrom] [char](10) NULL,
	[SerialNoto] [char](10) NULL,
	[Unit_Cls] [char](2) NULL,
	[CtnQty] [numeric](9, 0) NULL,
	[NetWeight] [numeric](18, 5) NULL,
	[GrossWeight] [numeric](18, 5) NULL,
	[Currency_Code] [char](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[Service] [numeric](18, 5) NULL,
	[Amount] [numeric](22, 5) NULL,
	[Revised_Cls] [char](1) NULL,
	[Lot_No] [char](7) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Interface_Cls] [char](1) NULL,
	[Interface_Date] [datetime] NULL,
	[Interface_User] [char](15) NULL,
 CONSTRAINT [PK_Delivery_Order] PRIMARY KEY CLUSTERED 
(
	[DO_No] ASC,
	[PO_No] ASC,
	[Seq_No] ASC,
	[DOSeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
