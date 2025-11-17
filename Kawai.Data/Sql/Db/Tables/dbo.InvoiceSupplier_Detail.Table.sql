SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [InvoiceSupplier_Detail](
	[Supplier_Code] [char](15) NOT NULL,
	[Invoice_No] [char](25) NOT NULL,
	[PO_No] [char](25) NOT NULL,
	[DO_No] [char](25) NOT NULL,
	[BC_No] [char](25) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[ReceiptSeq_No] [numeric](18, 0) NOT NULL,
	[Qty] [numeric](18, 5) NULL,
	[Unit_Cls] [char](2) NULL,
	[Currency_Code] [char](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[Amount] [numeric](22, 5) NULL,
	[Exchange_Amount] [numeric](22, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Price_Mat] [numeric](18, 5) NULL,
	[Amount_Mat] [numeric](22, 5) NULL,
	[Price_Service] [numeric](18, 5) NULL,
	[Amount_Service] [numeric](22, 5) NULL,
 CONSTRAINT [PK_InvoiceSupplier_Detail] PRIMARY KEY CLUSTERED 
(
	[Supplier_Code] ASC,
	[Invoice_No] ASC,
	[PO_No] ASC,
	[DO_No] ASC,
	[Item_Code] ASC,
	[ReceiptSeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [InvoiceSupplier_Detail] ADD  CONSTRAINT [DF_InvoiceSupplier_Detail_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [InvoiceSupplier_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_InvoiceSupplier_Detail_InvoiceSupplier_Master] FOREIGN KEY([Supplier_Code], [Invoice_No])
REFERENCES [InvoiceSupplier_Master] ([Supplier_Code], [Invoice_No])
GO
ALTER TABLE [InvoiceSupplier_Detail] CHECK CONSTRAINT [FK_InvoiceSupplier_Detail_InvoiceSupplier_Master]
GO
ALTER TABLE [InvoiceSupplier_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_InvoiceSupplier_Detail_Trade_Master] FOREIGN KEY([Supplier_Code])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [InvoiceSupplier_Detail] CHECK CONSTRAINT [FK_InvoiceSupplier_Detail_Trade_Master]
GO
