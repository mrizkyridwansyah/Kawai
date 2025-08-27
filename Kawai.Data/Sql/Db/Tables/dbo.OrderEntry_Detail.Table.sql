SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [OrderEntry_Detail](
	[Cust_Code] [char](15) NOT NULL,
	[PO_No] [char](35) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[Delivery_Date] [datetime] NOT NULL,
	[Seq_No] [numeric](18, 0) NOT NULL,
	[MakerItem_Code] [char](30) NULL,
	[Delivery_Time] [char](5) NULL,
	[Price] [numeric](18, 5) NULL,
	[Service] [numeric](18, 0) NULL,
	[Currency_Code] [char](2) NULL,
	[Unit_Cls] [char](2) NULL,
	[Qty] [numeric](18, 5) NULL,
	[SerialNoFrom] [char](10) NULL,
	[SerialNoto] [char](10) NULL,
	[Amount] [numeric](22, 5) NULL,
	[Remarks] [char](35) NULL,
	[Lot_No] [char](7) NULL,
	[Calculate_Cls] [char](1) NULL,
	[Generate_Cls] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Edit_Price_cls] [char](1) NULL,
	[PlaceOfDestination_Cls] [char](1) NULL,
 CONSTRAINT [PK_OrderEntry_Detail] PRIMARY KEY CLUSTERED 
(
	[Cust_Code] ASC,
	[PO_No] ASC,
	[Seq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [OrderEntry_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_OrderEntry_Detail_OrderEntry_Master] FOREIGN KEY([Cust_Code], [PO_No])
REFERENCES [OrderEntry_Master] ([Cust_Code], [PO_No])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [OrderEntry_Detail] CHECK CONSTRAINT [FK_OrderEntry_Detail_OrderEntry_Master]
GO
