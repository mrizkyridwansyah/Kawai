SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Packing_Master](
	[Cust_Code] [char](15) NOT NULL,
	[Consignee] [char](15) NOT NULL,
	[Notify_Code] [char](15) NOT NULL,
	[Packing_No] [char](25) NOT NULL,
	[Packing_Date] [datetime] NULL,
	[Stuffing_Date] [datetime] NULL,
	[ETD] [datetime] NULL,
	[ETA] [datetime] NULL,
	[Amount] [numeric](22, 5) NULL,
	[Total_Qty] [numeric](18, 5) NULL,
	[TotalWeight_Netto] [numeric](18, 5) NULL,
	[TotalWeight_Gross] [numeric](18, 5) NULL,
	[Total_Volume] [numeric](18, 5) NULL,
	[Reissue_Cls] [char](1) NULL,
	[Fix_Cls] [char](1) NULL,
	[List_PO] [char](750) NULL,
	[List_PODate] [char](750) NULL,
	[Payment_Code] [char](2) NULL,
	[Payment_Days] [numeric](3, 0) NULL,
	[Payment_Terms] [char](2) NULL,
	[Payment] [char](50) NULL,
	[Transportation_Cls] [char](2) NULL,
	[Vessel] [char](25) NULL,
	[Mother_Vessel] [char](25) NULL,
	[From_Port] [char](25) NULL,
	[Country_Origin] [char](25) NULL,
	[Forwarder] [char](50) NULL,
	[To_Port] [char](25) NULL,
	[Final_Destination] [char](25) NULL,
	[POCaseMark1] [char](100) NULL,
	[POCaseMark2] [char](100) NULL,
	[POCaseMark3] [char](100) NULL,
	[POCaseMark4] [char](100) NULL,
	[POCaseMark5] [char](100) NULL,
	[PackingStyle_Cls] [char](2) NULL,
	[WHCode] [char](15) NULL,
	[Remarks] [char](50) NULL,
	[ConsigneeTitle] [char](50) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[List_DO] [char](750) NULL,
	[List_DODate] [char](750) NULL,
	[Register_Date] [datetime] NULL,
	[Final_Destination_Cls] [char](1) NULL,
	[FinalPlace_Destination_Cls] [char](1) NULL,
	[Company_Code] [char](5) NULL,
 CONSTRAINT [PK_Packing_Master] PRIMARY KEY CLUSTERED 
(
	[Packing_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Packing_Master]  WITH NOCHECK ADD  CONSTRAINT [FK_Packing_Master_Trade_Master] FOREIGN KEY([Cust_Code])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [Packing_Master] CHECK CONSTRAINT [FK_Packing_Master_Trade_Master]
GO
