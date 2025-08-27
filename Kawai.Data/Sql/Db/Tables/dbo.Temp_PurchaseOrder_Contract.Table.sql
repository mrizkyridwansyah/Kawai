SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Temp_PurchaseOrder_Contract](
	[PO_No] [char](25) NOT NULL,
	[Supplier_Code] [char](15) NOT NULL,
	[PO_Date] [datetime] NULL,
	[Delivery_Date] [datetime] NULL,
	[Item_Code] [char](25) NULL,
	[Qty] [numeric](9, 5) NULL,
	[QtyRemaining_PO] [numeric](9, 5) NULL,
	[Qty_Receipt] [numeric](9, 5) NULL,
	[QtyRemaining_Receipt] [numeric](9, 5) NULL,
	[Price] [numeric](18, 5) NULL,
	[Amount] [numeric](18, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL
) ON [PRIMARY]
GO
