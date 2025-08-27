SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [purchaseorder_detail_history1](
	[PO_Correction_no] [varchar](20) NOT NULL,
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
	[Price_Adj] [numeric](18, 5) NULL
) ON [PRIMARY]
GO
