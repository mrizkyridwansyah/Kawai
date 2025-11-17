SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Invoice_Master](
	[Cust_Code] [char](15) NULL,
	[Invoice_No] [char](25) NOT NULL,
	[Invoice_Date] [datetime] NULL,
	[Delivery_Date] [datetime] NULL,
	[Amount] [numeric](22, 5) NULL,
	[PPN] [numeric](22, 5) NULL,
	[Total_Amount] [numeric](22, 5) NULL,
	[Remarks] [char](100) NULL,
	[Reissue_Cls] [char](1) NULL,
	[Fix_Cls] [char](1) NULL,
	[List_DO] [char](750) NULL,
	[List_PO] [char](750) NULL,
	[List_PODate] [char](750) NULL,
	[Paid_Cls] [char](1) NULL,
	[Paid_Date] [datetime] NULL,
	[Due_Date] [datetime] NULL,
	[Exchange_Rate] [numeric](9, 2) NULL,
	[Exchange_Amount] [numeric](22, 5) NULL,
	[TradeTerms_Cls] [char](2) NULL,
	[Attn] [char](50) NULL,
	[AirFreightCharge] [numeric](18, 4) NULL,
	[PEBNo] [char](25) NULL,
	[PEBDate] [datetime] NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Interface_Cls] [char](1) NULL,
	[Interface_Date] [datetime] NULL,
	[Interface_User] [char](15) NULL,
	[Voucher_No] [char](16) NULL,
	[Voucher_Description] [char](50) NULL,
	[InterfaceDel_Cls] [char](1) NULL,
	[InterfaceDel_Date] [datetime] NULL,
	[InterfaceDel_User] [char](15) NULL,
 CONSTRAINT [PK_Invoice_Master] PRIMARY KEY CLUSTERED 
(
	[Invoice_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Invoice_Master] ADD  CONSTRAINT [DF_Invoice_Master_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
