SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PurchaseOrder_Master_History](
	[PO_Correction_no] [varchar](20) NOT NULL,
	[PO_No] [char](25) NOT NULL,
	[Supplier_Code] [char](15) NOT NULL,
	[Period] [char](6) NULL,
	[PO_Date] [datetime] NULL,
	[Delivery_Date] [datetime] NULL,
	[WHTo] [char](15) NULL,
	[Deliver_To] [char](15) NULL,
	[PriceCondition_Cls] [char](2) NULL,
	[PaymentTerm_Cls] [char](2) NULL,
	[POPacking_Cls] [char](2) NULL,
	[Insurance_Cls] [char](2) NULL,
	[PO_LOT] [char](35) NULL,
	[Transportation_Cls] [char](2) NULL,
	[POMarking1] [char](25) NULL,
	[POMarking2] [char](25) NULL,
	[POMarking3] [char](25) NULL,
	[POMarking4] [char](25) NULL,
	[POMarking5] [char](25) NULL,
	[POMarking6] [char](25) NULL,
	[Remarks] [char](250) NULL,
	[Amount] [numeric](22, 5) NULL,
	[PPN] [numeric](18, 5) NULL,
	[PPH] [numeric](18, 5) NULL,
	[Total_Amount] [numeric](22, 5) NULL,
	[Fix_Cls] [char](1) NULL,
	[SheetCoil_Cls] [char](1) NULL,
	[Revise_No] [char](2) NULL,
	[Others_Cls] [char](1) NULL,
	[Discount] [numeric](22, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[POSet_Code] [char](15) NULL,
	[POSet_SeqNo] [numeric](18, 0) NULL,
	[Reason] [varchar](500) NULL,
	[Approved_date] [datetime] NULL,
	[Approved_User] [char](15) NULL,
	[Approved_Cls] [char](1) NULL,
 CONSTRAINT [PK_PurchaseOrder_Master_History] PRIMARY KEY CLUSTERED 
(
	[PO_Correction_no] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
