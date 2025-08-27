SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PurchaseOrder_Detail](
	[Seq_No] [numeric](18, 0) NOT NULL,
	[PO_No] [char](25) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[Item_Name] [char](50) NULL,
	[PORequest_No] [char](20) NULL,
	[POReq_SeqNo] [numeric](18, 0) NULL,
	[Delivery_Date] [datetime] NULL,
	[Price] [numeric](18, 5) NULL,
	[Price_Service] [numeric](18, 5) NULL,
	[Currency_Code] [char](2) NULL,
	[Unit_Cls] [char](2) NULL,
	[Qty] [numeric](18, 5) NULL,
	[Amount] [numeric](22, 5) NULL,
	[Amount_Service] [numeric](22, 5) NULL,
	[Complete_Cls] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Price_Adj] [numeric](18, 5) NULL,
	[PriceContractCls_Detail] [char](1) NULL,
 CONSTRAINT [PK_PurchaseOrder_Detail] PRIMARY KEY CLUSTERED 
(
	[PO_No] ASC,
	[Item_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [PurchaseOrder_Detail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_Detail_PurchaseOrder_Detail] FOREIGN KEY([PO_No], [Item_Code])
REFERENCES [PurchaseOrder_Detail] ([PO_No], [Item_Code])
GO
ALTER TABLE [PurchaseOrder_Detail] CHECK CONSTRAINT [FK_PurchaseOrder_Detail_PurchaseOrder_Detail]
GO
