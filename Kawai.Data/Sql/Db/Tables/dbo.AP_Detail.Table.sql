SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [AP_Detail](
	[Supplier_Code] [char](15) NOT NULL,
	[AP_No] [char](12) NOT NULL,
	[Invoice_No] [char](25) NOT NULL,
	[Currency_Code] [char](2) NULL,
	[Amount] [numeric](22, 5) NULL,
	[PPN] [numeric](18, 0) NULL,
	[Exchange_Rate] [numeric](9, 2) NULL,
	[Exchange_Amount] [numeric](22, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_AP_Detail] PRIMARY KEY CLUSTERED 
(
	[Supplier_Code] ASC,
	[AP_No] ASC,
	[Invoice_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [AP_Detail] ADD  CONSTRAINT [DF_AP_Detail_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [AP_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_AP_Detail_AP_Master] FOREIGN KEY([Supplier_Code], [AP_No])
REFERENCES [AP_Master] ([Supplier_Code], [AP_No])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [AP_Detail] CHECK CONSTRAINT [FK_AP_Detail_AP_Master]
GO
ALTER TABLE [AP_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_AP_Detail_InvoiceSupplier_Master] FOREIGN KEY([Supplier_Code], [Invoice_No])
REFERENCES [InvoiceSupplier_Master] ([Supplier_Code], [Invoice_No])
GO
ALTER TABLE [AP_Detail] CHECK CONSTRAINT [FK_AP_Detail_InvoiceSupplier_Master]
GO
ALTER TABLE [AP_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_AP_Detail_Trade_Master] FOREIGN KEY([Supplier_Code])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [AP_Detail] CHECK CONSTRAINT [FK_AP_Detail_Trade_Master]
GO
