SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PO_Contract_Master](
	[Contract_No] [varchar](20) NOT NULL,
	[PO_No] [varchar](20) NULL,
	[Supplier_Code] [varchar](20) NULL,
	[Period] [varchar](10) NULL,
	[Contract_Date] [date] NULL,
	[Delivery_Date] [date] NULL,
	[Currency_Code] [varchar](10) NULL,
	[Rate] [decimal](18, 4) NULL,
	[Total_Amount] [decimal](18, 2) NULL,
	[Remarks] [varchar](255) NULL,
	[Contract_Status] [varchar](20) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [varchar](50) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK__PO_Contr__5E2E9E756F76D9BF] PRIMARY KEY CLUSTERED 
(
	[Contract_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
