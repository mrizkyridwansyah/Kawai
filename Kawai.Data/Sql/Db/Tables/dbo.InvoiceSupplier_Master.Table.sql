SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [InvoiceSupplier_Master](
	[Supplier_Code] [char](15) NOT NULL,
	[Invoice_No] [char](25) NOT NULL,
	[Invoice_Date] [datetime] NULL,
	[InvoiceReceipt_Date] [datetime] NULL,
	[Due_Date] [datetime] NULL,
	[BL_Date] [datetime] NULL,
	[BL_No] [char](25) NULL,
	[AirFreight_Amount] [numeric](18, 5) NULL,
	[Total_Amount] [numeric](22, 5) NULL,
	[Exchange_Rate] [numeric](9, 2) NULL,
	[Exchange_Amount] [numeric](22, 5) NULL,
	[FakturPajak_No] [char](25) NULL,
	[PPN] [numeric](18, 0) NULL,
	[Bank_Code] [char](5) NULL,
	[Paid_Cls] [char](1) NULL,
	[Paid_Date] [datetime] NULL,
	[Fix_Cls] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Total_Amount_Mat] [numeric](22, 5) NULL,
	[Total_Amount_Service] [numeric](22, 5) NULL,
	[FakturPajak_Date] [datetime] NULL,
	[PaymentVoucher_No] [char](100) NULL,
	[VoucherDesc] [varchar](50) NULL,
	[Interface_Cls] [char](1) NULL,
	[Interface_Date] [datetime] NULL,
	[Interface_User] [char](15) NULL,
 CONSTRAINT [PK_InvoiceSupplier_Master] PRIMARY KEY CLUSTERED 
(
	[Supplier_Code] ASC,
	[Invoice_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [InvoiceSupplier_Master]  WITH NOCHECK ADD  CONSTRAINT [FK_InvoiceSupplier_Master_Trade_Master] FOREIGN KEY([Supplier_Code])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [InvoiceSupplier_Master] CHECK CONSTRAINT [FK_InvoiceSupplier_Master_Trade_Master]
GO
