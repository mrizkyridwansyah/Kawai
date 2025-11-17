SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Trade_Master](
	[Trade_Code] [char](15) NOT NULL,
	[Trade_Cls] [char](1) NULL,
	[Trade_Name] [char](70) NULL,
	[Trade_Abbr] [char](20) NULL,
	[Contact_Person] [char](50) NULL,
	[Address1] [char](100) NULL,
	[Address2] [char](100) NULL,
	[City] [char](50) NULL,
	[Country] [char](50) NULL,
	[Country_Cls] [char](1) NULL,
	[Epte_Cls] [char](1) NULL,
	[Region_Cls] [char](2) NULL,
	[Postal_Code] [char](10) NULL,
	[Telephone] [char](50) NULL,
	[Fax] [char](50) NULL,
	[Closing_Day] [char](2) NULL,
	[Pay_Day] [char](2) NULL,
	[InvoicePay_Days] [numeric](3, 0) NULL,
	[Affiliate_Cls] [char](1) NULL,
	[Insurance_Cls] [char](2) NULL,
	[NPWP_No] [char](20) NULL,
	[NPWP_Name] [char](100) NULL,
	[NPWP_Address] [char](200) NULL,
	[NPWP_City] [char](50) NULL,
	[NPPKP_No] [char](20) NULL,
	[Invoice_To] [char](15) NULL,
	[PO_Cls] [char](1) NULL,
	[Price_Condition] [char](2) NULL,
	[POPayment_Code] [char](2) NULL,
	[POPayment_Day] [numeric](3, 0) NULL,
	[POPayment_Terms] [char](2) NULL,
	[POPayment] [char](50) NULL,
	[Transportation_Cls] [char](2) NULL,
	[POCaseMark1] [char](25) NULL,
	[POCaseMark2] [char](25) NULL,
	[POCaseMark3] [char](25) NULL,
	[POCaseMark4] [char](25) NULL,
	[POCaseMark5] [char](25) NULL,
	[POMarking1] [char](25) NULL,
	[POMarking2] [char](25) NULL,
	[POMarking3] [char](25) NULL,
	[POMarking4] [char](25) NULL,
	[POMarking5] [char](25) NULL,
	[POMarking6] [char](25) NULL,
	[Subcon_WH_Code] [char](15) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[NG_Cls] [char](1) NULL,
	[SAP_Code] [char](15) NULL,
	[Type_BC] [varchar](15) NULL,
	[No_Izin] [varchar](50) NULL,
	[CODE_KPPBC] [varchar](6) NULL,
	[NoIzin_Date] [date] NULL,
	[NITKU] [varchar](30) NULL,
	[Company_Code] [char](25) NULL,
 CONSTRAINT [PK_Trade_Master] PRIMARY KEY CLUSTERED 
(
	[Trade_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Trade_Master] ADD  CONSTRAINT [DF_Trade_Master_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [Trade_Master]  WITH CHECK ADD  CONSTRAINT [FK_Trade_Master_PaymentCode_Cls] FOREIGN KEY([POPayment_Code])
REFERENCES [PaymentCode_Cls] ([PaymentCode_Cls])
GO
ALTER TABLE [Trade_Master] CHECK CONSTRAINT [FK_Trade_Master_PaymentCode_Cls]
GO
ALTER TABLE [Trade_Master]  WITH CHECK ADD  CONSTRAINT [FK_Trade_Master_PaymentTerm_Cls] FOREIGN KEY([POPayment_Terms])
REFERENCES [PaymentTerm_Cls] ([PaymentTerm_Cls])
GO
ALTER TABLE [Trade_Master] CHECK CONSTRAINT [FK_Trade_Master_PaymentTerm_Cls]
GO
ALTER TABLE [Trade_Master]  WITH CHECK ADD  CONSTRAINT [FK_Trade_Master_PriceCondition_Cls] FOREIGN KEY([Price_Condition])
REFERENCES [PriceCondition_Cls] ([PriceCondition_Cls])
GO
ALTER TABLE [Trade_Master] CHECK CONSTRAINT [FK_Trade_Master_PriceCondition_Cls]
GO
ALTER TABLE [Trade_Master]  WITH CHECK ADD  CONSTRAINT [FK_Trade_Master_PriceCondition_Cls1] FOREIGN KEY([Price_Condition])
REFERENCES [PriceCondition_Cls] ([PriceCondition_Cls])
GO
ALTER TABLE [Trade_Master] CHECK CONSTRAINT [FK_Trade_Master_PriceCondition_Cls1]
GO
ALTER TABLE [Trade_Master]  WITH CHECK ADD  CONSTRAINT [FK_Trade_Master_Region_Cls] FOREIGN KEY([Region_Cls])
REFERENCES [Region_Cls] ([Region_Cls])
GO
ALTER TABLE [Trade_Master] CHECK CONSTRAINT [FK_Trade_Master_Region_Cls]
GO
ALTER TABLE [Trade_Master]  WITH CHECK ADD  CONSTRAINT [FK_Trade_Master_Transportation_Cls] FOREIGN KEY([Transportation_Cls])
REFERENCES [Transportation_Cls] ([Transportation_Cls])
GO
ALTER TABLE [Trade_Master] CHECK CONSTRAINT [FK_Trade_Master_Transportation_Cls]
GO
